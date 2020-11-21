using System;
using System.Numerics;

namespace GameServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }
            Server.clients[_fromClient].SendIntoGame(_username);
            EnemySpawner.InitTimer();
        }

        public static void PlayerMovement(int _fromClient, Packet _packet)
        {
            try
            {
                Vector3 position = _packet.ReadVector3();

                if (Server.clients[_fromClient] == null) return;
                Server.clients[_fromClient].player.Move(position);
            }
            catch
            {
            }
        }

        public static void PlayerKnocked(int _fromClient, Packet _packet)
        {
            bool isKnocked = _packet.ReadBool();

            if (Server.clients[_fromClient] == null) return;
            Server.clients[_fromClient].player.KnockDown(isKnocked);
        }

        public static void EnemyTakeDamage(int _fromClient, Packet _packet)
        {
            float damage = _packet.ReadFloat();
            int length = _packet.ReadInt();
            for (int i = 0; i < length; i++)
            {
                Enemy.enemies[_packet.ReadInt()].TakeDamage(damage);
            }
        }
    }
}