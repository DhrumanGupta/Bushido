using Game.Networking;
using UnityEngine;

namespace Game.Networking
{
    public class ClientSend : MonoBehaviour
    {
        private static void SendTCPData(Packet _packet)
        {
            _packet.WriteLength();
            Client.Instance.tcp.SendData(_packet);
        }

        private static void SendUDPData(Packet _packet)
        {
            _packet.WriteLength();
            Client.Instance.udp.SendData(_packet);
        }

        #region Packets
        public static void WelcomeReceived()
        {
            using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
            {
                _packet.Write(Client.Instance.myId);
                _packet.Write("Berlm" + Client.Instance.myId);

                SendTCPData(_packet);
            }
        }

        public static void PlayerMovement(Vector3 position)
        {
            using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
            {
                _packet.Write(position);
                SendUDPData(_packet);
            }
        }

        public static void PlayerKnocked(bool isKnocked)
        {
            using (Packet _packet = new Packet((int) ClientPackets.playerKnocked))
            {
                _packet.Write(isKnocked);
                SendTCPData(_packet);
            }
        }

        public static void PlayerAttack(int[] targets, float damage)
        {
            using (Packet _packet = new Packet((int) ClientPackets.playerAttack))
            {
                _packet.Write(damage);
                _packet.Write(targets.Length);
                foreach (var target in targets)
                {
                    _packet.Write(target);
                }
                SendTCPData(_packet);
            }
        }
        #endregion
    }
}