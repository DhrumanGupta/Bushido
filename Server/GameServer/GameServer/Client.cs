using System;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Numerics;

namespace GameServer
{
    public class Client
    {
        public static int dataBufferSize = 4096;
        
        public int id;
        public Player player;
        public TCP tcp;

        public Client(int _clientId)
        {
            id = _clientId;
            tcp = new TCP(id);
        }

        public class TCP
        {
            public TcpClient socket;

            private readonly int id;
            private NetworkStream stream;
            private Packet receivedData;
            public byte[] receiveBuffer;

            public TCP(int _id)
            {
                id = _id;
            }

            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();
                receivedData = new Packet();
                
                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                
                ServerSend.Welcome(id, "Connected to the server! \n Welcome!");
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
                    Console.WriteLine($"Error sending data to player {id} via TCP: {e}");
                }
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
                            Server.packetHandlers[_packetId](id, _packet);
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
        }

        public void SendIntoGame(string _playerName)
        {
            player = new Player(id, _playerName, Vector3.Zero);

            foreach (Client _client in Server.clients.Values)
            {
                if (_client.player != null && _client.id != id)
                {
                    ServerSend.SpawnPlayer(id, _client.player);
                }
            }

            foreach (Client _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    ServerSend.SpawnPlayer(_client.id, player);
                }
            }
        }
    }
}