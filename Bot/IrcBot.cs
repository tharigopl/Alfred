using IrcDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bot.Commands;
using Bot.Commands.Attributes;
using Bot.Extensions;
using Bot.Tasks;

namespace Bot
{
    public class IrcBot
    {
        public IrcBotConfiguration Configuration { get; private set; }

        private readonly string commandPrefix;
        private readonly List<IIrcTask> tasks;

        private readonly IrcClient client;
        private bool isRegistered;

        private static IrcCommandProcessorFactory commandProcessorFactory = 
            new IrcCommandProcessorFactory(
                typeof(IrcCommandProcessor).SubclassesWithAttribute<IrcCommandAttribute>()
            );

        public IrcBot(IrcBotConfiguration configuration)
        {
            this.tasks = new List<IIrcTask>();

            configuration.UserName = configuration.UserName ?? configuration.NickName;
            configuration.RealName = configuration.RealName?? configuration.NickName;

            this.Configuration = configuration;

            this.commandPrefix = string.Format("{0}", this.Configuration.NickName.ToLower());

            this.client = new IrcClient();
        }

        public async Task Run()
        {
            SubscribeToClientEvents();

            using (this.client)
            {
                Connect();
                WaitForRegistration();
                await Start();
            }
        }

        private async Task Start()
        {
            await Task.Run(() => {
                while (this.IsConnected || this.tasks.Any(t => t.IsRunning))
                {
                    Thread.Sleep(250);
                }
            });
        }

        private void WaitForRegistration()
        {
            while (!this.IsConnected && !this.isRegistered)
            {
                Thread.Sleep(200);
            }
        }

        private void Connect()
        {
            this.client.Connect(
                this.Configuration.HostName,
                this.Configuration.Port,
                false,
                new IrcUserRegistrationInfo {
                    NickName = this.Configuration.NickName,
                    UserName = this.Configuration.UserName,
                    RealName = this.Configuration.RealName
                }
            );
        }

        public bool IsConnected
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

            if (IsValidCommand(parts)) {
                ProcessCommand(parts, channel, e.Source);
            }
        }

        private bool IsValidCommand(string[] commandParts)
        {
            return (commandParts.Length > 0 && commandParts[0].ToLower().StartsWith(this.commandPrefix));
        }

        private void ProcessCommand(string[] commandParts, IIrcMessageTarget target, IIrcMessageSource source)
        {
            var command = new IrcCommand(
                this,
                this.client,
                commandParts,
                target,
                source
            );

            Task.Run(() =>
            {
                try
                {
                    var processor = commandProcessorFactory.CreateByCommand(command);
                    processor.Process(command);
                }
                catch (Exception ex)
                {
                    HandleCommandException(command, ex);
                }
            });
        }

        private void HandleCommandException(IrcCommand command, Exception ex)
        {
            var message = 
                string.Format(
                    "{0}, I ran into a bit of a problem with that last request: {1}",
                    command.Source.Name,
                    ex.Message
                );

            if (ex.InnerException != null) {
                message = string.Format(
                    "{0} {1}",
                    message,
                    ex.InnerException.Message
                );
            }

            this.client.LocalUser.SendMessage(
                command.Target,
                message.Replace(Environment.NewLine, " ")
            );
        }

        private void SubscribeToRegisteredClientEvents(IrcLocalUser user)
        {
            user.JoinedChannel += OnJoinedChannel;
        }

        private void OnJoinedChannel(object sender, IrcChannelEventArgs e)
        {
            StartTasks();
            SubscribeToChannelEvents(e.Channel);
        }

        private void OnRegistered(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            if (client != null)
            {
                this.isRegistered = true;
                SubscribeToRegisteredClientEvents(client.LocalUser);
                JoinChannels();
            }
        }

        private void JoinChannels()
        {
            this.client.Channels.Join(this.Configuration.Channel);
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
