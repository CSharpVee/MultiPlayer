using MPClientBase.DTO;

namespace PongServer.DTOs
{
    public class PlayerInputDTO : IDataPackTyped<PlayerInputDTO>
    {
        public bool UseSupershot { get; set; }
        public short Move { get; set; }

        public byte[] GetBytes()
        {
            //     ↓↓  movement: 0 - neutral, 1 - up, 2 - down, 3 - unused
            //       ↓ charging: 0 - neutral, 1 - charging
            //||||||||

            byte movement = 0;
            if (Move > 0)
                movement = 2;// (+y)
            else if (Move < 0)
                movement = 1;// (-y)

            byte charging = (byte)(UseSupershot ? 1 : 0);

            return new byte[] { (byte)(movement << 1 | charging) };
        }

        public PlayerInputDTO ParseIn(byte[] data)
        {
            UseSupershot = (data[0] & 0b1) != 0;
            var move = data[0] >> 1;
            if (move == 1)
                Move = -1;
            if (move == 2)
                Move = 1;

            return this;
        }

        public static PlayerInputDTO Empty => new PlayerInputDTO() { Move = 0, UseSupershot = false };
    }
}
