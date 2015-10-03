using IrcDotNet;
using LiveSplit.Model.Input;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace LiveSplit.Web.Share
{

    public class TwitchChat : IDisposable
    {
        private IrcClient Client { get; set; }

        public EventHandlerT<Message> OnMessage;
        public EventHandlerT<User> OnNewSubscriber;

        public IEnumerable<User> Users 
            => Client.Channels.FirstOrDefault().Users.Select(x => new User(x.User, GetFlags(x.User.NickName), GetColor(x.User.NickName))); 

        protected Dictionary<string, ChatBadges> UserFlags { get; set; }
        protected Dictionary<string, Color> UserColors { get; set; }

        public string Channel { get; protected set; }

        public TwitchChat(string accessToken, string channel)
        {
            Client = new IrcClient();
            UserFlags = new Dictionary<string, ChatBadges>();
            UserColors = new Dictionary<string, Color>();
            var twitch = Twitch.Instance;
            Client.Connected += Client_Connected;
            Client.Registered += Client_Registered;
            Channel = channel;

            Client.Connect("irc.twitch.tv", 6667,
                new IrcUserRegistrationInfo()
                {
                    NickName = twitch.ChannelName,
                    Password = string.Format("oauth:{0}", accessToken)
                });
        }

        protected ChatBadges GetFlags(string username)
        {
            return UserFlags.ContainsKey(username)
                ? UserFlags[username]
                : ChatBadges.None;
        }

        protected Color GetColor(string username)
        {
            return UserColors.ContainsKey(username)
                ? UserColors[username]
                : Color.White;
        }

        protected void AddFlag(string username, ChatBadges flag)
        {
            if (UserFlags.ContainsKey(username))
                UserFlags[username] |= flag;
            else
                UserFlags.Add(username, flag);
        }

        protected void AddColor(string username, Color color)
        {
            if (UserColors.ContainsKey(username))
                UserColors[username] = color;
            else
                UserColors.Add(username, color);
        }

        void WaitForChannelJoin(object sender, IrcRawMessageEventArgs e)
        {
            if (e.Message.Command == "353")
            {
                Client.Channels.FirstOrDefault().MessageReceived += Channel_MessageReceived;
                Client.RawMessageReceived -= WaitForChannelJoin;
                AddFlag(Client.Channels.FirstOrDefault().Name.Substring(1), ChatBadges.Moderator | ChatBadges.Broadcaster);
                SendMessage("/mods");
            }
        }

        void Client_Registered(object sender, EventArgs e)
        {
        }

        void Client_Connected(object sender, EventArgs e)
        {
            Client.RawMessageReceived += WaitForChannelJoin;
            Client.LocalUser.MessageReceived += LocalUser_MessageReceived;
            Client.SendRawMessage("TWITCHCLIENT 1");
            Client.Channels.Join("#" + Channel);
        }

        void LocalUser_MessageReceived(object sender, IrcMessageEventArgs e)
        {
            try
            {
                if (e.Source.Name == "jtv")
                {
                    if (e.Text.StartsWith("The moderators of this room are:"))
                    {
                        var modsText = e.Text.Substring("The moderators of this room are: ".Length);
                        var mods = modsText.Replace(", ", ",").Split(',');
                        foreach (var mod in mods)
                        {
                            AddFlag(mod, ChatBadges.Moderator);
                        }
                        return;
                    }

                    var splits = e.Text.Split(' ');
                    var command = splits[0];

                    if (splits.Length < 2)
                        return;

                    var user = splits[1];

                    if (splits.Length < 3)
                        return;

                    var argument = splits[2];

                    if (command == "USERCOLOR")
                    {
                        var color = Color.FromArgb((0xFF << 24) + int.Parse(argument.Substring(1), NumberStyles.HexNumber));
                        AddColor(user, color);
                    }
                    else if (command == "SPECIALUSER")
                    {
                        dynamic chatFlags = Enum.Parse(typeof(ChatBadges), argument, true);
                        AddFlag(user, chatFlags);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public void Close()
        {
            Client.Disconnect();
        }

        void Channel_MessageReceived(object sender, IrcMessageEventArgs e)
        {
            if (e.Source.Name == "twitchnotify" && e.Text.Contains("subscribed"))
            {
                var subs = Twitch.Instance.Subscribers;
                var username = e.Text.Substring(0, e.Text.IndexOf(' '));
                Twitch.Instance._Subscribers.Add(username);

                OnNewSubscriber?.Invoke(this, new User(Client, username));
            }

            OnMessage?.Invoke(this, new Message(new User(e.Source as IrcUser, GetFlags(e.Source.Name), GetColor(e.Source.Name)), e.Text));
        }

        public void SendMessage(string message)
        {
            var channel = Client.Channels.FirstOrDefault();
            if (channel == null)
                return;

            Client.LocalUser.SendMessage(channel, message);
        }

        public void Dispose()
        {
            ((IDisposable)Client).Dispose();
        }

        #region Inner classes

        [Flags]
        public enum ChatBadges
        {
            None = 0,
            Broadcaster = 1,
            Staff = 2, 
            Admin = 4,
            Moderator = 8,
            Turbo = 16,
            Subscriber = 32
        }

        public class User
        {
            public Color Color { get; protected set; }
            public ChatBadges Badges { get; protected set; }

            public string Name { get; protected set; }

            public User(IrcUser user, ChatBadges badges, Color color)
            {
                Name = user.NickName;
                Badges = badges;
                Color = color;
            }

            public User(IrcClient client, string name)
            {
                Name = name;
                try
                {
                    client.Channels.FirstOrDefault().Users.FirstOrDefault(x => x.User.NickName == name);
                } 
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

        public class Message
        {
            public User User { get; protected set; }
            public string Text { get; protected set; }

            public Message(User user, string text)
            {
                User = user;
                Text = text;
            }
        }

        #endregion
    }
}
