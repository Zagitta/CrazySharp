using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CrazySharpLib.Radio.Packets;

namespace CrazySharpLib.Radio
{
    public interface ICrazyRadio
    {
        void EnqueueSend(CRTPPacket packet);
    }

    public class CrazyRadio<T> : ICrazyRadio where T : ICrazyDispatcher
    {
        //crazy flie fields
        private readonly CrazyFlieDongle _dongle;
        public readonly T Dispatcher;
        
        //threading fields
        private Thread _sendThread;
        private Thread _recieveThread;
        private CancellationTokenSource _cancellation;
        private ConcurrentQueue<CRTPPacket> _packets;
        private ConcurrentQueue<CRTPResponse> _responses; 

        public CrazyRadio(CrazyFlieDongle dongle, T dispatcher)
        {
            _dongle = dongle;
            Dispatcher = dispatcher;
        }

        public void Start()
        {
            _sendThread = new Thread(SendLoop);
            _recieveThread = new Thread(ReceiveLoop);
            _cancellation = new CancellationTokenSource();
            //clear out old packets
            _packets = new ConcurrentQueue<CRTPPacket>();
            _responses = new ConcurrentQueue<CRTPResponse>();

            _dongle.Open();
            _sendThread.Start();
            _recieveThread.Start();
        }

        public void Stop()
        {
            _cancellation.Cancel();

            _dongle.Close();
            _sendThread.Join();
            _recieveThread.Join();
        }

        public void EnqueueSend(CRTPPacket packet)
        {
            _packets.Enqueue(packet);
        }

        /// <summary>
        /// Thread loop that sends enqueued packets and saves responses to another queue
        /// </summary>
        private void SendLoop()
        {
            int sleepCount = 0;
            while (_cancellation.IsCancellationRequested == false)
            {
                CRTPPacket pk;

                if (_packets.TryDequeue(out pk) == false)
                {
                    if(sleepCount > 10)
                        _packets.Enqueue(new CRTPIdlePacket());

                    Thread.Sleep(1);
                    sleepCount++;
                    continue;
                }

                sleepCount = 0;

                var response = _dongle.SendPacket(pk);

                _responses.Enqueue(response);
            }
        }

        /// <summary>
        /// Thread loop that waits for responses and executes callbacks for them
        /// </summary>
        private void ReceiveLoop()
        {
            while (_cancellation.IsCancellationRequested == false)
            {
                CRTPResponse response;

                if (_responses.TryDequeue(out response) == false)
                {
                    Thread.Sleep(0);
                    continue;
                }

                Dispatcher.Callback(response);
            }
        }
    }
}
