using System.Reflection.Metadata;

namespace GameServer
{
    public class ServerSend
    {
        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        private static void SendTCPDataToAll(Packet _packet)
        {
            for (int i = 0; i < Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
        
        private static void SendTCPDataToAll(int exception, Packet _packet)
        {
            for (int i = 0; i < Server.MaxPlayers; i++)
            {
                if (i == exception) continue;
                Server.clients[i].tcp.SendData(_packet);
            }
        }
        
        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int) ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void SpawnPlayer(int _toClient, Player player)
        {
            using (Packet _packet = new Packet((int) ServerPackets.spawnPlayer))
            {
                _packet.Write(player.id);
                _packet.Write(player.usermame);
                _packet.Write(player.position);
                _packet.Write(player.rotation);
                
                SendTCPData(_toClient, _packet);
            }
        }
    }
}