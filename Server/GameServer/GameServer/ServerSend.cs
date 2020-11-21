using System;

namespace GameServer
{
    class ServerSend
    {
        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        private static void SendUDPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].udp.SendData(_packet);
        }

        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
        private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].tcp.SendData(_packet);
                }
            }
        }

        private static void SendUDPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
        private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i == _exceptClient) continue;
                Server.clients[i].udp.SendData(_packet);
            }
        }

        #region Packets
        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void SpawnPlayer(int _toClient, Player _player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
            {
                _packet.Write(_player.id);
                _packet.Write(_player.username);
                _packet.Write(_player.position);
                _packet.Write(_player.rotation);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void PlayerPosition(Player _player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
            {
                _packet.Write(_player.id);
                _packet.Write(_player.position);

                SendUDPDataToAll(_player.id, _packet);
            }
        }

        public static void PlayerLeft(Player _player)
        {
            using (Packet _packet = new Packet((int) ServerPackets.playerLeft))
            {
                _packet.Write(_player.id);
                SendTCPDataToAll(_player.id, _packet);
            }
        }

        public static void SpawnEnemy(Enemy enemy)
        {
            using (Packet _packet = new Packet((int) ServerPackets.enemySpawn))
            {
                SendTCPDataToAll(SpawnEnemy_Data(enemy, _packet));
            }
        }
        
        public static void SpawnEnemy(int toClient, Enemy enemy)
        {
            using (Packet _packet = new Packet((int) ServerPackets.enemySpawn))
            {
                SendTCPData(toClient, SpawnEnemy_Data(enemy, _packet));
            }
        }

        private static Packet SpawnEnemy_Data(Enemy enemy, Packet _packet)
        {
            _packet.Write(enemy.id);
            _packet.Write(enemy.position);
            _packet.Write(enemy.rotation);
            _packet.Write(enemy.type);
            _packet.Write(enemy.health);

            return _packet;
        }

        public static void EnemyMovement(Enemy enemy)
        {
            using (Packet _packet = new Packet((int) ServerPackets.enemyMove))
            {
                _packet.Write(enemy.id);
                _packet.Write(enemy.position);
                
                SendUDPDataToAll(_packet);
            } 
        }

        public static void EnemyAttack(Enemy enemy, Player toAttack)
        {
            using (Packet _packet = new Packet((int) ServerPackets.enemyAttack))
            {
                _packet.Write(enemy.id);
                _packet.Write(toAttack.id);

                SendTCPDataToAll(_packet);
            } 
        }
        
        public static void EnemyHealth(Enemy enemy)
        {
            using (Packet _packet = new Packet((int) ServerPackets.enemyHealth))
            {
                _packet.Write(enemy.id);
                _packet.Write(enemy.health);
                
                SendTCPDataToAll(_packet);
            }
        }
        #endregion

        public static void PlayerKnocked(Player player)
        {
            using (Packet _packet = new Packet((int) ServerPackets.playerKnocked))
            {
                _packet.Write(player.id);
                _packet.Write(player.isKnocked);

                SendTCPDataToAll(player.id, _packet);
            }
        }

        public static void PlayerHeal(Player player)
        {
            Console.WriteLine("Healing Player");
            using (Packet _packet = new Packet((int) ServerPackets.playerKnocked))
            {
                _packet.Write(player.id);

                SendTCPDataToAll(_packet);
            }
        }
    }
}    