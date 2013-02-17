using IrcDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bot.Commands;
using Bot.Tasks;

namespace Bot
{
    public class IrcBot
    {
        //private readonly IrcBotConfiguration configuration;

        private readonly string nickName;
        private readonly string userName;
        private readonly string realName;
        private readonly string commandPrefix;
        private readonly string channel;

        private readonly string hostName;
        private readonly int port;

        private readonly List<IIrcTask> tasks;

        private readonly IrcClient client;

        public IrcBot(IrcBotConfiguration configuration)
        {
            this.tasks = new List<IIrcTask>();

            this.nickName = configuration.NickName;
            this.userName = configuration.UserName ?? this.nickName;
            this.realName = configuration.RealName ?? this.nickName;

            this.hostName = configuration.HostName;
            this.port = configuration.Port;
            this.channel = configuration.Channel;

            this.commandPrefix = string.Format("!{0}", this.nickName.ToLower());

            this.client = new IrcClient();
        }

        public async Task Run()
        {
            SubscribeToClientEvents();

            using (this.client)
            {
                this.client.Connect(
                    this.hostName,
                    this.port,
                    false,
                    new IrcUserRegistrationInfo
                    {
                        NickName = this.nickName,
                        UserName = this.userName,
                        RealName = this.realName
                    }
                );

                while (!this.Connected)
                {
                    Thread.Sleep(200);
                }

                Thread.Sleep(1000);

                await Task.Run(() =>
                {
                    StartTasks();
                    while (this.Connected || this.tasks.Any(t => t.IsRunning))
                    {
                        Thread.Sleep(250);
                    };
                });
            }
        }

        public bool Connected
        {
            get { return this.client.IsConnected; }
        }

        private void StartTasks()
        {
            foreach (var task in this.tasks)
            {
                task.Start();
            }
        }

        private void StopTasks()
        {
            var tasks = this.tasks.Select(t => t.Task).ToArray();

            foreach (var task in this.tasks)
            {
                task.Stop();
            }

            Task.WaitAll(tasks);
        }

        private void SubscribeToClientEvents()
        {
            this.client.Registered += OnRegistered;
            this.client.Disconnected += OnDisconnected;
        }

        private void OnDisconnected(object sender, EventArgs e)
        {
            StopTasks();
        }

        private void SubscribeToChannelEvents(IrcChannel channel)
        {
            channel.MessageReceived += OnChannelMessageReceived;
        }

        private void OnChannelMessageReceived(object sender, IrcMessageEventArgs e)
        {
            var channel = sender as IrcChannel;
            var parts = e.Text.Split(' ');

            if (parts.Length > 0 && parts[0].ToLower().Equals(this.commandPrefix)) {
                ProcessCommand(parts, channel, e.Source);
            }
        }

        private void ProcessCommand(string[] commandParts, IIrcMessageTarget target, IIrcMessageSource source)
        {
            var command = new IrcCommand(
                this.client,
                commandParts,
                target,
                source
            );

            var factory = new IrcCommandProcessorFactory();
            var processor = factory.GetByCommand(command);
            processor.Process(command);
        }


        private void SubscribeToRegisteredClientEvents(IrcLocalUser user)
        {
            user.JoinedChannel += OnJoinedChannel;
        }

        void OnJoinedChannel(object sender, IrcChannelEventArgs e)
        {
            SubscribeToChannelEvents(e.Channel);
        }

        private void OnRegistered(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            SubscribeToRegisteredClientEvents(client.LocalUser);
            JoinChannels();
        }

        private void JoinChannels()
        {
            this.client.Channels.Join(this.channel);
        }

        public void AddTask(IrcTask task)
        {
            task.OnMessage += OnTaskMessage;
            this.tasks.Add(task);
        }

        private void OnTaskMessage(object sender, IrcTaskMessageEventArgs e)
        {
            foreach (var channel in this.client.Channels) {
                foreach (var message in e.Messages) {
                    if (this.client.IsRegistered)
                    {
                        this.client.LocalUser.SendMessage(channel, message);
                    }
                }
            }
        }
    }
}
