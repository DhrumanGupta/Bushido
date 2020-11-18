using System.Numerics;

namespace GameServer
{
    public class Player
    {
        public int id;
        public string usermame;

        public Vector3 position;
        public Quaternion rotation;

        public Player(int id, string usermame, Vector3 spawnPosition)
        {
            this.id = id;
            this.usermame = usermame;
            position = spawnPosition;
            rotation = Quaternion.Identity;
        }
    }
}