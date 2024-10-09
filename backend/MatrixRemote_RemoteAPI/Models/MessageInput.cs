namespace MatrixRemote_RemoteAPI.Models
{
    public class MessageInput
    {
        public required string Message { get; set; }
        public required RgbColor Color { get; set; }
    }
}
