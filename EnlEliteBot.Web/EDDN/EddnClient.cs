using NetMQ;
using NetMQ.Sockets;
using Org.BouncyCastle.Utilities.Zlib;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace EnlEliteBot.Web.EDDN
{
    public class EddnClient
    {
        //we'll reuse these buffers, to reduce allocs of (possible large) objects
        //they'll grow to the largest seen message, but will only do that growth once.
        private readonly MemoryStream _outStream = new MemoryStream();
        private readonly UTF8Encoding _utf8 = new UTF8Encoding();

        private readonly byte[] _buffer = new byte[4096];

        public event EventHandler<string> OnMessageReceived;

        public void StartOnBackgroundThread()
        {
            ThreadPool.QueueUserWorkItem(Run);
        }

        private void Run(object _)
        {
            using (var client = new SubscriberSocket())
            {
                client.Options.ReceiveHighWatermark = 1000;
                client.Connect("tcp://eddn.edcd.io:9500");
                client.SubscribeToAnyTopic();

                while (true)
                {
                    var bytes = client.ReceiveFrameBytes();
                    var result = Decompress(bytes);

                    OnMessageReceived?.Invoke(this, result);
                }
            }
        }

        private string Decompress(byte[] bytes)
        {
            using (var inputStream = new MemoryStream(bytes))
            using (var zipStream = new ZInputStream(inputStream)) //this uses the byte[] provided, no array copies involved.
            {
                zipStream.CopyTo(_outStream);

                var result = _utf8.GetString(_outStream.ToArray());

                _outStream.SetLength(0); //reset buffers for next message

                return result;
            }
        }

        public byte[] ToArray(ZInputStream input)
        {

            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(_buffer, 0, _buffer.Length)) > 0)
                {
                    ms.Write(_buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
