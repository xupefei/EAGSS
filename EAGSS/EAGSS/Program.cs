namespace EAGSS
{
#if WINDOWS || XBOX

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            using (var game = new EAGSS())
            {
                game.Run();
            }
        }
    }

#endif
}