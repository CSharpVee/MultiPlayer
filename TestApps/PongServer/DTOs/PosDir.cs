using PongServer.DTOs.Duplicates;

namespace PongServer.DTOs
{
    public class PosDir
    {
        public Vector2 Position;
        public short Dir;

        public PosDir()
        {
        }

        public PosDir(Vector2 inPos, short inDir)
        {
            Position = inPos;
            Dir = inDir;
        }
    }
}
