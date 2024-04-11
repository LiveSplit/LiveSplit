using LiveSplit.Options;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LiveSplit.Server
{
    public class MessageEventArgs : EventArgs
    {
        public Connection Connection { get; }
        public string Message { get; }

        public MessageEventArgs(Connection connection, string message)
        {
            Connection = connection;
            Message = message;
        }
    }

    public delegate void MessageEventHandler(object sender, MessageEventArgs e);

    public class Connection : IDisposable
    {
        protected Stream Stream { get; private set; }
        protected StreamReader Reader { get; private set; }
        protected StreamWriter Writer { get; private set; }
        protected Thread ReaderThread { get; private set; }

        public event MessageEventHandler MessageReceived;
        public event EventHandler Disconnected;

        public Connection(Stream stream)
        {
            Stream = stream;
            Reader = new StreamReader(Stream, Encoding.UTF8, false);

            Writer = new StreamWriter(Stream, new UTF8Encoding(false))
            {
                NewLine = "\n"
            };

            ReaderThread = new Thread(new ThreadStart(ReadCommands));
            ReaderThread.Start();
        }

        public void ReadCommands()
        {
            while (true)
            {
                string command = null;
                try
                {
                    command = Reader.ReadLine();
                }
                catch { }
                if (command != null)
                {
                    MessageReceived?.Invoke(this, new MessageEventArgs(this, command));
                }
                else break;
            }

            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        public void SendMessage(string message)
        {
            try
            {
                Writer.WriteLine(message);
                Writer.Flush();
            }
            catch (Exception e)
            {
                Log.Error(e);
                Disconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        public virtual void Dispose()
        {
            ReaderThread.Abort();
            Stream.Dispose();
            Reader.Dispose();
            Writer.Dispose();
        }
    }

    public class TcpConnection : Connection
    {
        protected TcpClient client { get; private set; }

        public TcpConnection(TcpClient client) : base(client.GetStream())
        {
            this.client = client;
        }

        public override void Dispose()
        {
            base.Dispose();
            client.Close();
        }
    }
}
