using UnityEngine;

namespace Game.Networking
{
    public class ClientHandle : MonoBehaviour
    {
        public static void Welcome(Packet packet)
        {
            string msg = packet.ReadString();
            int myId = packet.ReadInt();
            
            Debug.Log($"Message from server: \n {msg}");
            Client.instance.myId = myId;
            ClientSend.WelcomeRecieved();
        }

        public static void SpawnPlayer(Packet packet)
        {
            int id = packet.ReadInt();
            string username = packet.ReadString();
            Vector3 position = packet.ReadVector3();
            Quaternion rotation = packet.ReadQuaternion();
            
            GameManager.Instance.SpawnPlayer(id, username, position, rotation);
        }
    }
}
