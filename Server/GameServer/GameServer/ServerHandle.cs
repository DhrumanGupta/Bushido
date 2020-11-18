using System;

namespace GameServer
{
    public class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();
            
            Console.Write($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully, and is now player {_username}.");
            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Player '{_username}' (ID: {_fromClient} has assumed the wrong client ID ({_clientIdCheck}))");
            }
            Server.clients[_fromClient].SendIntoGame(_username);
        }
    }
}