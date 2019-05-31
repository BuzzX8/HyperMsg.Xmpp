using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    public class XmlTransceiverFake : ITransceiver<XmlElement, XmlElement>
    {
        private readonly Queue<XmlElement> responses = new Queue<XmlElement>();
        private readonly List<XmlElement> requests = new List<XmlElement>();
        private readonly ManualResetEventSlim syncEvent = new ManualResetEventSlim();
        private readonly ManualResetEventSlim receiveLock = new ManualResetEventSlim();

        public IEnumerable<XmlElement> Requests => requests;

        public void AddResponse(XmlElement response) => responses.Enqueue(response);

        public XmlElement Receive()
        {
            if (responses.Count == 0)
            {
                receiveLock.Wait();
            }

            return responses.Dequeue();
        }

        public Task<XmlElement> ReceiveAsync(CancellationToken cancellationToken) => Task.FromResult(Receive());

        public void Send(XmlElement message)
        {
            requests.Add(message);
            syncEvent.Set();
        }

        public Task SendAsync(XmlElement message, CancellationToken cancellationToken)
        {
            Send(message);
            return Task.CompletedTask;
        }

        public void WaitSendCompleted(TimeSpan timeout)
        {
            syncEvent.Wait(timeout);
            syncEvent.Reset();
        }
    }
}
