using System;
using System.Numerics;
using System.Timers;

namespace GameServer
{
    public class EnemySpawner
    {
        private static Timer timer1;
        private static Random random;
        private static int spawnTime = 3500;
        public static void InitTimer()
        {
            random = new Random();
            timer1 = new Timer();
            timer1.Elapsed += new ElapsedEventHandler(timer1_Tick);
            timer1.Interval = spawnTime; // in miliseconds
            timer1.Start();
        }

        private static void timer1_Tick(object sender, EventArgs e)
        {
            spawnTime -= 50;
            if (spawnTime < 350) spawnTime = 350; 
            int randomN = random.Next(0, 3);
            Enemy.Type type;
            if (randomN > 0.66f)
            {
                type = Enemy.Type.Nunchuck;
            }
            else if (randomN > 0.33f)
            {
                type = Enemy.Type.Kunai;
            }
            else
            {
                type = Enemy.Type.Shuriken;
            }
            Enemy enemy = new Enemy(1.5f, type);
            timer1.Interval = spawnTime; // in miliseconds
            timer1.Start();
        }
    }
}