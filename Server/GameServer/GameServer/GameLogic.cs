namespace GameServer
{
    class GameLogic
    {
        public static void Update()
        {
            ThreadManager.UpdateMain();
            Server.Update();
        }
    }
}