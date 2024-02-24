using System;
using System.IO;
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
        protected Thread ReaderThread { get; private set; }

        public event MessageEventHandler MessageReceived;
        public event EventHandler Disconnected;

        public Connection(Stream stream)
        {
            Stream = stream;
            Reader = new StreamReader(Stream);

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
            var buffer = Encoding.UTF8.GetBytes(message + Environment.NewLine);
            Stream.Write(buffer, 0, buffer.Length);
        }

        public void Dispose()
        {
            Stream.Dispose();
            ReaderThread.Abort();
        }
    }
}
