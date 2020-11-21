using System.Numerics;

namespace GameServer
{
    class Player
    {
        public int id;
        public string username;

        public Vector3 position;
        public Quaternion rotation;

        public bool isKnocked = false;

        public Player(int _id, string _username, Vector3 _spawnPosition)
        {
            id = _id;
            username = _username;
            position = _spawnPosition;
            rotation = Quaternion.Identity;
        }

        public void Move(Vector3 newPos)
        {
            position = newPos;
            ServerSend.PlayerPosition(this);
        }

        public void KnockDown(bool _isKnocked)
        {
            isKnocked = _isKnocked;
            ServerSend.PlayerKnocked(this);
        }
    }
}