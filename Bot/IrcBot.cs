using System.Collections.Concurrent;
using System.ComponentModel;
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
using Topshelf;

namespace Bot
{
    public class IrcBot : IDisposable
    {
        public IrcBotConfiguration Configuration { get; private set; }

        private readonly string commandPrefix;
        private readonly List<IIrcTask> tasks;

        private readonly IrcClient client;
        private bool isRegistered;
        private HostControl hostControl;

        private ConcurrentDictionary<string, IrcBotUser> users;

        private static IrcCommandProcessorFactory commandProcessorFactory = 
            new IrcCommandProcessorFactory(
                typeof(IrcCommandProcessor).SubclassesWithAttribute<IrcCommandAttribute>()
        );

        public IrcBot(IrcBotConfiguration configuration)
        {
            this.tasks = new List<IIrcTask>();
            this.users = new ConcurrentDictionary<string, IrcBotUser>();

            configuration.UserName = configuration.UserName ?? configuration.NickName;
            configuration.RealName = configuration.RealName?? configuration.NickName;

            this.Configuration = configuration;

            this.commandPrefix = string.Format("{0}", this.Configuration.NickName.ToLower());

            this.client = new IrcClient();
        }

        public void Start(HostControl hostControl)
        {
            this.hostControl = hostControl;
            SubscribeToClientEvents();
            Connect();
            WaitForRegistration();
        }

        public void Stop()
        {
            this.StopTasks();
        }

        public void Pause()
        {
            this.tasks.ForEach(t => t.Pause());
        }

        public void Resume()
        {
            this.tasks.ForEach(t => t.Resume());
        }

        public void RegisterUser(IrcBotUser user)
        {
            IrcBotUser existingUser;
            if (this.users.TryGetValue(user.NickName, out existingUser))
                this.users.TryUpdate(user.NickName, user, existingUser);
            else
                this.users.TryAdd(user.NickName, user);
        }

        private IrcBotUser GetUser(string nickName)
        {
            IrcBotUser user;
            this.users.TryGetValue(nickName, out user);
            return user;
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
            this.client.Error += OnError;
            this.client.Registered += OnRegistered;
            this.client.Disconnected += OnDisconnected;
            this.client.RawMessageReceived += OnRawMessageReceived;
        }

        private void OnError(object sender, IrcErrorEventArgs e)
        {
            // HACK : log somewhere, console for now
            Console.WriteLine("Error occured in IRC client: {0}", e.Error.Message);
        }

        private void OnRawMessageReceived(object sender, IrcRawMessageEventArgs e)
        {
            if (e.Message.Command == "NICK")
            {
                var oldNick = e.Message.Source.Name;
                var newNick = e.Message.Parameters.FirstOrDefault();
                HandleNickChange(oldNick, newNick);
            }
        }

        private void HandleNickChange(string oldNick, string newNick)
        {
            IrcBotUser user;
            if (this.users.TryRemove(oldNick, out user))
            {
                user.NickName = newNick;
                if (this.users.TryAdd(user.NickName, user))
                {
                    SendMessage(newNick,
                        string.Format(
                            "I see you've chosen a new name, {0}. Your active user ({1}) has been updated.",
                            newNick,
                            user.UserName
                        )
                    );
                }
                else
                {
                    SendMessage(newNick, "Sorry, you've been logged out.");
                }
            }
        }

        private void SendMessage(string nick, string message)
        {
            this.client.LocalUser.SendMessage(nick, message);
        }

        private void OnDisconnected(object sender, EventArgs e)
        {
            StopTasks();
            this.hostControl.Restart();
        }

        private void SubscribeToChannelEvents(IrcChannel channel)
        {
            channel.MessageReceived += OnChannelMessageReceived;
            channel.UserLeft += OnUserLeft;
        }

        private void OnChannelMessageReceived(object sender, IrcMessageEventArgs e)
        {
            var channel = sender as IrcChannel;
            var parts = e.Text.Trim().Split(' ');

            if (IsValidChannelCommand(parts)) {
                var command = new IrcCommand(
                    this,
                    GetUser(e.Source.Name),
                    this.client,
                    parts,
                    channel,
                    e.Source
                );

                ProcessCommand(command);
            }
        }

        private void OnUserLeft(object sender, IrcChannelUserEventArgs e)
        {
            IrcBotUser user;
            this.users.TryRemove(e.ChannelUser.User.NickName, out user);
        }

        private bool IsValidChannelCommand(string[] commandParts)
        {
            return (commandParts.Length > 0 && commandParts[0].ToLower().StartsWith(this.commandPrefix));
        }

        private void ProcessCommand(IrcCommand command)
        {
            NotifyAdmins(command);
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

        private void NotifyAdmins(IrcCommand command)
        {
            var targets = this.users.Values
                .Where(u => u.IsBotAdmin)
                .Select(u => u.NickName)
                .ToArray();

            if (targets.Length > 0)
            {
                this.client.LocalUser.SendMessage(
                    targets,
                    string.Format(
                            "command - source: {0} target: {1} name: {2} params: {3}",
                            command.Source.Name,
                            command.Target.Name,
                            command.Name,
                            string.Join(" ", command.Parameters)
                        )
                    );
            }
        }

        private void HandleCommandException(IrcCommand command, Exception ex)
        {
            try
            {
                var message = string.Format(
                    "{0}, I ran into a bit of a problem with that last request: {1}",
                    command.Source.Name,
                    ex.Message
                );

                if (ex.InnerException != null)
                {
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
            catch (Exception secondException)
            {
                // HACK: log, console for now
                Console.WriteLine(
                    "Exception occured while handling exception from command ({0}): {1}",
                    command.Name,
                    secondException.Message
                );
            }
        }

        private void SubscribeToRegisteredClientEvents(IrcLocalUser user)
        {
            user.JoinedChannel += OnJoinedChannel;
            user.MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, IrcMessageEventArgs e)
        {
            var parts = e.Text.Trim().Split(' ');

            if (!e.Source.Name.Equals(this.Configuration.NickName, StringComparison.OrdinalIgnoreCase))
            {
                var command = new IrcMessageCommand(
                    this,
                    GetUser(e.Source.Name),
                    this.client,
                    parts,
                    e.Source as IIrcMessageTarget,
                    e.Source
                );

                ProcessCommand(command);
            }
        }

        private void OnJoinedChannel(object sender, IrcChannelEventArgs e)
        {
            StartTasks();
            SubscribeToChannelEvents(e.Channel);
        }

        private void OnRegistered(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            if (client == null) return;

            this.isRegistered = true;
            SubscribeToRegisteredClientEvents(client.LocalUser);
            JoinChannels();
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

        public void Dispose()
        {
            if (this.client != null)
            {
                this.client.Dispose();
            }
        }
    }
}
