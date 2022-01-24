namespace ServerCore
{
    class UserManager
    {
        static object _lock = new object();

        public static void TestUser()
        {
            lock (_lock)
            {
            }
        }

        public static void Test()
        {
            lock (_lock)
            {
                SessionManager.TestSession();
            }
        }
    }
}
