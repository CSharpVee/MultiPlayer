namespace PongServer.DTOs
{
    internal class Player
    {
        public int ID { get; set; } = -1;
        public bool ReadySentIn { get; set; } = false;
        public PlayerInputDTO LastInput { get; set; } = PlayerInputDTO.Empty;
    }
}
