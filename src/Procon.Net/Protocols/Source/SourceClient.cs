﻿using System;
using System.Collections.Generic;

namespace Procon.Net.Protocols.Source {
    using Procon.Net.Protocols.Source.Logging.BroadcastListener;
    using Procon.Net.Protocols.Source.Logging.BroadcastService;

    public class SourceClient : TcpClient<SourcePacket> {
        protected Dictionary<int?, SourcePacket> SentPackets;
        protected Queue<SourcePacket> QueuePackets;

        protected SourceBroadcastListener BroadcastListener { get; set; }

        public ushort SourceLogServicePort { get; set; }
        public ushort SourceLogListenPort { get; set; }

        protected readonly Object QueueUnqueuePacketLock = new Object();

        public SourceClient(string hostname, ushort port) : base(hostname, port) {
            this.SentPackets = new Dictionary<int?, SourcePacket>();
            this.QueuePackets = new Queue<SourcePacket>();

            this.PacketSerializer = new SourcePacketSerializer();

            this.ConnectionStateChanged += new ConnectionStateChangedHandler(SourceClient_ConnectionStateChanged);
        }

        private void SourceClient_ConnectionStateChanged(Client<SourcePacket> sender, ConnectionState newState) {
            if (newState == Net.ConnectionState.ConnectionLoggedIn) {
                // Start the broadcast service. This will work or exit if the port is already bound to.
                new SourceBroadcastService(this.SourceLogServicePort, this.SourceLogListenPort).Connect();

                // Now setup a listener to catch relevent console log packets and dispatch them
                this.BroadcastListener = new SourceBroadcastListener(this.SourceLogListenPort);
                this.BroadcastListener.PacketReceived += new Client<SourceBroadcastListenerPacket>.PacketDispatchHandler(BroadcastListener_PacketReceived);
                this.BroadcastListener.Connect();
            }
        }

        private void BroadcastListener_PacketReceived(Client<SourceBroadcastListenerPacket> sender, SourceBroadcastListenerPacket packet) {

            // TODO: don't compare by string. it's slow.
            if (this.ConnectionState == Net.ConnectionState.ConnectionLoggedIn && this.RemoteEndPoint.ToString().CompareTo(packet.RemoteEndPoint.ToString()) == 0) {

                this.OnPacketReceived(packet);
            }
        }

        private bool QueueUnqueuePacket(bool blSendingPacket, SourcePacket cpPacket, out SourcePacket cpNextPacket) {
            cpNextPacket = null;
            bool blResponse = false;

            lock (this.QueueUnqueuePacketLock) {

                // @todo look at moving the queue from Frostbite to a util location so it may be used here.

                if (blSendingPacket == true) {
                    if (this.SentPackets.Count > 0) {
                        this.QueuePackets.Enqueue(cpPacket);
                        //if (this.PacketQueued != null) {
                        //    FrostbiteConnection.RaiseEvent(this.PacketQueued.GetInvocationList(), this, cpPacket, Thread.CurrentThread.ManagedThreadId);
                        //    //this.PacketQueued(cpPacket, Thread.CurrentThread.ManagedThreadId);
                        //}
                        blResponse = true;
                    }
                    else {
                        if (this.SentPackets.Count == 0 && this.QueuePackets.Count > 0) {
                            // TODO: I've seen it slip in here once, but that was when I had
                            // combined the events and commands streams.  Have not seen it since, but need to make sure.

                            //throw new Exception();
                        }
                        else {
                            // No packets waiting for response, free to send the new packet.
                            blResponse = false;
                        }
                    }
                }
                else {
                    // Else it's being called from recv and cpPacket holds the processed RequestPacket.

                    // Remove the packet 
                    if (cpPacket != null) {
                        if (this.SentPackets.ContainsKey(cpPacket.RequestId) == true) {
                            this.SentPackets.Remove(cpPacket.RequestId);
                        }
                    }

                    if (this.QueuePackets.Count > 0) {
                        cpNextPacket = this.QueuePackets.Dequeue();
                        //if (this.PacketDequeued != null) {
                        //    FrostbiteConnection.RaiseEvent(this.PacketDequeued.GetInvocationList(), this, cpNextPacket, Thread.CurrentThread.ManagedThreadId);
                        //    //this.PacketDequeued(cpNextPacket, Thread.CurrentThread.ManagedThreadId);
                        //}
                        blResponse = true;
                    }
                    else {
                        blResponse = false;
                    }

                }

                return blResponse;
            }
        }

        public SourcePacket GetRequestPacket(SourcePacket recievedPacket) {

            SourcePacket requestPacket = null;

            if (this.SentPackets.ContainsKey(recievedPacket.RequestId) == true) {
                requestPacket = this.SentPackets[recievedPacket.RequestId];
            }

            return requestPacket;
        }

        protected override void OnPacketReceived(SourcePacket packet) {
            base.OnPacketReceived(packet);

            // Pop the next packet if a packet is waiting to be sent.
            SourcePacket nextPacket = null;
            if (this.QueueUnqueuePacket(false, packet, out nextPacket) == true) {
                this.Send(nextPacket);
            }
        }

        public override void Send(SourcePacket packet) {

            if (packet.RequestId == null) {
                packet.RequestId = (int)this.AcquireSequenceNumber;
            }

            // QueueUnqueuePacket
            SourcePacket nullPacket = null;

            if (packet.Origin == PacketOrigin.Server && packet.Type == PacketType.Response) {
                // I don't think this will ever be encountered since OnPacketReceived calls the base.Send.
                base.Send(packet);
            }
            else {
                // Null return because we're not popping a packet, just checking to see if this one needs to be queued.
                if (this.QueueUnqueuePacket(true, (SourcePacket)packet, out nullPacket) == false) {
                    // No need to queue, queue is empty.  Send away..
                    base.Send(packet);
                }
            }
        }

        protected override bool BeforePacketSend(SourcePacket packet) {

            if (packet.Origin == PacketOrigin.Client && packet.Type == PacketType.Request && this.SentPackets.ContainsKey(packet.RequestId) == false) {
                this.SentPackets.Add(packet.RequestId, packet);
            }

            return base.BeforePacketSend(packet);
        }


    }
}
