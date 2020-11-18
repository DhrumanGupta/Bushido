using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace Game.Networking
{
    public class Client : MonoBehaviour
    {
        public static Client instance;
        public static int dataBufferSize = 4096;

        public string ip = "127.0.0.1";
        public int port = 6969;
        public int myId = 0;
        public TCP tcp;

        private delegate void PacketHandler(Packet _packet);
        private static Dictionary<int, PacketHandler> packetHandlers;
        
        private void Awake()
        {
            if (instance != null) Destroy(this);
            instance = this;
        }

        private void Start()
        {
            tcp = new TCP();
            ConnectToServer();
        }

        public void ConnectToServer()
        {
            InitializeClientData();
            tcp.Connect();
        }

        public class TCP
        {
            public TcpClient socket;

            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public void Connect()
            {
                socket = new TcpClient()
                {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize
                };
                
                receiveBuffer = new byte[dataBufferSize];
                socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
            }

            private void ConnectCallback(IAsyncResult _result)
            {
                socket.EndConnect(_result);

                if (!socket.Connected) return;

                stream = socket.GetStream();
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                
                receivedData = new Packet();
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    // Tell the stream to stop reading and give an output
                    // The int represents the amount of bytes written
                    int _byteLength = stream.EndRead(_result);

                    if (_byteLength <= 0) // True: no bytes were written (disconnect)
                    {
                        return;
                    }
                        
                    byte[] data = new byte[_byteLength];
                        
                    // Copy only the bytes written in from the receive buffer to the newly made array
                    Array.Copy(receiveBuffer, data, _byteLength);
                        
                    receivedData.Reset(HandleData(data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error receiving TCP data: {e}");
                }
            }

            private bool HandleData(byte[] data)
            {
                int _packetLength = 0;

                receivedData.SetBytes(data);

                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    byte[] _packedBytes = receivedData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packedBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            packetHandlers[_packetId](_packet);
                        }
                    });

                    _packetLength = 0;
                    if (receivedData.UnreadLength() >= 4)
                    {
                        _packetLength = receivedData.ReadInt();
                        if (_packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (_packetLength <= 1)
                {
                    return true;
                }
                return false;
            }

            public void SendData(Packet _packet)
            {
                try
                {
                    if (socket == null) return;
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        
        private void InitializeClientData()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ServerPackets.welcome, ClientHandle.Welcome }
            };
            Debug.Log("Initialized packets.");
        }
    }
}
