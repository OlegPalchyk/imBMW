using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using imBMW.Tools;

namespace imBMW.iBus
{
    public static class Manager
    {
        static ISerialPort iBus;
        public static bool Inited { get; private set; }

        static QueueThreadWorker messageWriteQueue;
        //static QueueThreadWorker messageReadQueue;

        static DateTime lastMessage = DateTime.Now;
        static byte[] messageBuffer = new byte[Message.PacketLengthMax];
        static int messageBufferLength = 0;
        static object bufferSync = new object();

        public static void Init(ISerialPort port)
        {
            messageWriteQueue = new QueueThreadWorker(SendMessage);
            //messageReadQueue = new QueueThreadWorker(ProcessMessage);

            iBus = port;
            iBus.DataReceived += new SerialDataReceivedEventHandler(iBus_DataReceived);

            Inited = true;
        }

        #region Message reading and processing

        static void iBus_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            ISerialPort port = (ISerialPort)sender;
            if (port.AvailableBytes == 0)
            {
                Logger.Warning("Available bytes lost! " + port.ToString());
                return;
            }
            lock (bufferSync)
            {
                byte[] data = port.ReadAvailable();
                if (messageBufferLength + data.Length > messageBuffer.Length)
                {
                    Logger.Info("Buffer overflow. Extending it. " + port.ToString());
                    byte[] newBuffer = new byte[messageBuffer.Length * 2];
                    Array.Copy(messageBuffer, newBuffer, messageBufferLength);
                    messageBuffer = newBuffer;
                }
                if (data.Length == 1)
                {
                    messageBuffer[messageBufferLength++] = data[0];
                }
                else
                {
                    Array.Copy(data, 0, messageBuffer, messageBufferLength, data.Length);
                    messageBufferLength += data.Length;
                }
                while (messageBufferLength >= Message.PacketLengthMin)
                {
                    Message m = Message.TryCreate(messageBuffer, messageBufferLength);
                    if (m == null)
                    {
                        if (!Message.CanStartWith(messageBuffer, messageBufferLength))
                        {
                            Logger.Warning("Buffer skip: non-iBus data detected: " + messageBuffer[0].ToHex());
                            SkipBuffer(1);
                            continue;
                        }
                        return;
                    }
                    ProcessMessage(m);
                    //#if DEBUG
                    //m.PerformanceInfo.TimeEnqueued = DateTime.Now;
                    //#endif
                    //messageReadQueue.Enqueue(m);
                    SkipBuffer(m.PacketLength);
                }
                lastMessage = DateTime.Now;
            }
        }

        static void SkipBuffer(int count)
        {
            messageBufferLength -= count;
            if (messageBufferLength > 0)
            {
                Array.Copy(messageBuffer, count, messageBuffer, 0, messageBufferLength);
            }
        }

        public static void ProcessMessage(Message m)
        {
            #if DEBUG
            m.PerformanceInfo.TimeStartedProcessing = DateTime.Now;
            #endif

            MessageEventArgs args = null;
            try
            {
                var e = BeforeMessageReceived;
                if (e != null)
                {
                    args = new MessageEventArgs(m);
                    e(args);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "on before message received " + m.ToPrettyString());
            }

            if (args != null && args.Cancel)
            {
                return;
            }

            foreach (MessageReceiverRegistration receiver in MessageReceiverList.ToArray())
            {
                try
                {
                    receiver.Process(m);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "while processing message: " + m.ToPrettyString());
                }
            }

            #if DEBUG
            m.PerformanceInfo.TimeEndedProcessing = DateTime.Now;
            #endif

            try
            {
                var e = AfterMessageReceived;
                if (e != null)
                {
                    if (args == null)
                    {
                        args = new MessageEventArgs(m);
                    }
                    e(args);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "on after message received " + m.ToPrettyString());
            }
        }

        #endregion

        #region Message writing and queue

        static void SendMessage(object o)
        {
            if (o is byte[])
            {
                iBus.Write((byte[])o);
                Thread.Sleep(iBus.AfterWriteDelay);
                return;
            }

            Message m = (Message)o;

            #if DEBUG
            m.PerformanceInfo.TimeStartedProcessing = DateTime.Now;
            #endif

            MessageEventArgs args = null;
            var e = BeforeMessageSent;
            if (e != null)
            {
                args = new MessageEventArgs(m);
                e(args);
                if (args.Cancel)
                {
                    return;
                }
            }

            iBus.Write(m.Packet);

            #if DEBUG
            m.PerformanceInfo.TimeEndedProcessing = DateTime.Now;
            #endif

            e = AfterMessageSent;
            if (e != null)
            {
                if (args == null)
                {
                    args = new MessageEventArgs(m);
                }
                e(args);
            }

            Thread.Sleep(m.AfterSendDelay > 0 ? m.AfterSendDelay : iBus.AfterWriteDelay); // Don't flood iBus
        }

        public static void EnqueueMessage(Message m)
        {
            #if DEBUG
            m.PerformanceInfo.TimeEnqueued = DateTime.Now;
            #endif
            try
            {
                messageWriteQueue.Enqueue(m);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void EnqueueMessage(params Message[] messages)
        {
            #if DEBUG
            var now = DateTime.Now;
            foreach (Message m in messages)
            {
                if (m != null)
                {
                    m.PerformanceInfo.TimeEnqueued = now;
                }
            }
            #endif
            messageWriteQueue.EnqueueArray(messages);
        }

        #endregion

        #region Message receiver registration

        /// <summary>
        /// Fired before processing the message by registered receivers.
        /// Message processing could be cancelled in this event
        /// </summary>
        public static event MessageEventHandler BeforeMessageReceived;

        /// <summary>
        /// Fired after processing the message by registered receivers
        /// </summary>
        public static event MessageEventHandler AfterMessageReceived;

        /// <summary>
        /// Fired before sending the message.
        /// Message processing could be cancelled in this event
        /// </summary>
        public static event MessageEventHandler BeforeMessageSent;

        /// <summary>
        /// Fired after sending the message
        /// </summary>
        public static event MessageEventHandler AfterMessageSent;

        static ArrayList messageReceiverList;

        static ArrayList MessageReceiverList
        {
            get
            {
                if (messageReceiverList == null)
                {
                    messageReceiverList = new ArrayList();
                }
                return messageReceiverList;
            }
        }

        public static void AddMessageReceiverForSourceDevice(DeviceAddress source, MessageReceiver callback)
        {
            MessageReceiverList.Add(new MessageReceiverRegistration(source, DeviceAddress.Unset, callback, MessageReceiverRegistration.MatchType.Source));
        }

        public static void AddMessageReceiverForDestinationDevice(DeviceAddress destination, MessageReceiver callback)
        {
            MessageReceiverList.Add(new MessageReceiverRegistration(DeviceAddress.Unset, destination, callback, MessageReceiverRegistration.MatchType.Destination));
        }

        public static void AddMessageReceiverForSourceAndDestinationDevice(DeviceAddress source, DeviceAddress destination, MessageReceiver callback)
        {
            MessageReceiverList.Add(new MessageReceiverRegistration(source, destination, callback, MessageReceiverRegistration.MatchType.SourceAndDestination));
        }

        public static void AddMessageReceiverForSourceOrDestinationDevice(DeviceAddress source, DeviceAddress destination, MessageReceiver callback)
        {
            MessageReceiverList.Add(new MessageReceiverRegistration(source, destination, callback, MessageReceiverRegistration.MatchType.SourceOrDestination));
        }

        #endregion

        #region Device searching on iBus

        const int findDeviceTimeout = 2000;

        static DeviceAddress findDevice;
        static ManualResetEvent findDeviceSync = new ManualResetEvent(false);
        static ArrayList foundDevices = new ArrayList();

        public static bool FindDevice(DeviceAddress device)
        {
            return FindDevice(device, findDeviceTimeout);
        }

        public static bool FindDevice(DeviceAddress device, int timeout)
        {
            if (foundDevices.Contains(device))
            {
                return true;
            }
            lock (foundDevices)
            {
                findDevice = device;
                findDeviceSync.Reset(); 
                AfterMessageReceived += SaveFoundDevice;
                EnqueueMessage(new Message(DeviceAddress.Diagnostic, device, MessageRegistry.DataPollRequest));
                findDeviceSync.WaitOne(timeout, true);
                AfterMessageReceived -= SaveFoundDevice;
                return foundDevices.Contains(device);
            }
        }

        static void SaveFoundDevice(MessageEventArgs e)
        {
            if (!foundDevices.Contains(e.Message.SourceDevice))
            {
                foundDevices.Add(e.Message.SourceDevice);
            }
            if (findDevice == e.Message.SourceDevice)
            {
                findDeviceSync.Set();
            }
        }

        #endregion
    }
}
