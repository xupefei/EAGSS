using Microsoft.Xna.Framework;

namespace EAGSS
{
    internal class GameSettings
    {
        public static int WindowWidth = 800;
        public static int WindowHeight = 600;

        public static Vector2 MessageWindowLocation = new Vector2(50, 450);
        public static Vector2 MessageWindowSize = new Vector2(500, 150);

        public static string DataFolderName = @"GameData";
        public static string DataPackageParameter = @"*.pkg";
    }
}