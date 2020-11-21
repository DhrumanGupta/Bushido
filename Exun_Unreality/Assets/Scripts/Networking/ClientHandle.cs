using System;
using System.ComponentModel;
using System.Net;
using Game.Combat;
using UnityEngine;

namespace Game.Networking
{
    public class ClientHandle : MonoBehaviour
    {
        public static void Welcome(Packet _packet)
        {
            string _msg = _packet.ReadString();
            int _myId = _packet.ReadInt();

            Debug.Log($"Message from server: {_msg}");
            Client.Instance.myId = _myId;
            ClientSend.WelcomeReceived();

            Client.Instance.udp.Connect(((IPEndPoint)Client.Instance.tcp.socket.Client.LocalEndPoint).Port);
        }

        public static void SpawnPlayer(Packet _packet)
        {
            int _id = _packet.ReadInt();
            string _username = _packet.ReadString();
            Vector3 _position = _packet.ReadVector3();
            Quaternion _rotation = _packet.ReadQuaternion();

            GameManager.Instance.SpawnPlayer(_id, _username, _position, _rotation);
        }

        public static void PlayerPosition(Packet _packet)
        {
            try
            {
                int _id = _packet.ReadInt();
                Vector3 _position = _packet.ReadVector3();

                GameManager.players[_id].MoveTo(_position);
            }
            catch
            {}
        }

        public static void PlayerLeft(Packet _packet)
        {
            int _id = _packet.ReadInt();

            GameManager.PlayerLeft(_id);
        }

        public static void EnemySpawn(Packet _packet)
        {
            int id = _packet.ReadInt();
            Vector3 position = _packet.ReadVector3();
            Quaternion rotation = _packet.ReadQuaternion();
            int type = _packet.ReadInt();
            float health = _packet.ReadFloat();
            
            GameManager.Instance.SpawnEnemy(id, position, rotation, type, health);
        }

        public static void EnemyMovement(Packet _packet)
        {
            try
            {
                int _id = _packet.ReadInt();
                Vector3 _position = _packet.ReadVector3();

                GameManager.enemies[_id].MoveTo(_position);
            }
            catch
            {
            }
        }

        public static void PlayerKnocked(Packet _packet)
        {
            int _id = _packet.ReadInt();
            bool isKnocked = _packet.ReadBool();
            
            GameManager.players[_id].KnockDownStatus(isKnocked);
        }

        public static void EnemyHealth(Packet _packet)
        {
            int id = _packet.ReadInt();
            float health = _packet.ReadFloat();
            
            GameManager.enemies[id].GetComponent<Health>().SetHealth(health);
        }

        public static void EnemyAttack(Packet _packet)
        {
            int enemyId = _packet.ReadInt();
            int playerId = _packet.ReadInt();

            GameManager.enemies[enemyId].GetComponent<Fighter>().Attack(GameManager.players?[playerId].gameObject);
        }
    }
}
