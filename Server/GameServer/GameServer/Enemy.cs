using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Metadata;

namespace GameServer
{
    public class Enemy
    {
        public enum Type
        {
            Nunchuck = 1,
            Kunai,
            Shuriken
        }
        
        public int id;
        private static int nextId = 1;
        public int type;

        public static Dictionary<int, Enemy> enemies = new Dictionary<int, Enemy>();

        public Vector3 position;
        public Quaternion rotation;

        private Vector3 min = new Vector3(-9.56f, -3.95f, 0);
        private Vector3 max = new Vector3(9.38f, 3.84f, 0);

        public float speed;
        
        public float health;
        public float damage;
        public float range;

        public Enemy(float _speed, Enum _type)
        {
            id = nextId;
            enemies.Add(id, this);
            nextId++;
            speed = _speed;
            type = Convert.ToInt32(_type);
            rotation = Quaternion.Identity;

            switch (_type)
            {
                case Type.Nunchuck:
                    health = 50;
                    range = 0.7f;
                    damage = 5f;
                    break;
                case Type.Kunai:
                    health = 50;
                    range = 0.8f;
                    damage = 7f;
                    break;
                case Type.Shuriken:
                    health = 50;
                    range = 0.9f;
                    damage = 9f;
                    break;
            }
            
            Spawn();
        }

        private void UpdatePos(Vector3 newPos)
        {
            position = newPos;
            ServerSend.EnemyMovement(this);
        }

        private float attackCooldown = 1f;
        private float lastAttacked = 0;
        public void Update()
        {
            lastAttacked += 1f/Constants.TICKS_PER_SEC;
            Player closestEnemy = null;
            float greatestDist = float.PositiveInfinity;
            for (int i = 1; i <= Server.clients.Count; i++)
            {
                Player player = Server.clients[i].player;
                if (player == null) continue;
                if (player.isKnocked) continue;
                float distance = (player.position - position).LengthSquared();

                if (greatestDist * greatestDist > distance)
                {
                    greatestDist = distance;
                    closestEnemy = player;
                }
            }

            if (closestEnemy == null) return;
            if (greatestDist < range)
            {
                if (attackCooldown < lastAttacked)
                {
                    ServerSend.EnemyAttack(this, closestEnemy);
                    lastAttacked = 0;
                }
            }
            
            Vector3 dir = Vector3.Normalize(closestEnemy.position - position);
            Vector3 toMove = (speed * 1f / Constants.TICKS_PER_SEC * dir) + position;
            UpdatePos(toMove);
        }

        public void Spawn()
        {
            position = GetRandomPosition();
            ServerSend.SpawnEnemy(this);
        }

        private Vector3 GetRandomPosition()
        {
            Random random = new Random();
            Vector3 spawnPos = new Vector3(GetRandomNumber(min.X, max.X), GetRandomNumber(min.X, max.X), 0f);
            return spawnPos;
        }
        
        public float GetRandomNumber(double minimum, double maximum)
        { 
            return (float)(Server.random.NextDouble() * (maximum - minimum) + minimum);
        }

        public void TakeDamage(float damage, int fromId)
        {
            health -= damage;
            if (health >= 0)
            {
                ServerSend.EnemyHealth(this);
            }
            
            if (health <= 0) enemies.Remove(id);
        }
    }
}