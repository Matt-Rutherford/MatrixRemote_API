using MatrixRemote_RemoteAPI.Models.Dto;

namespace MatrixRemote_RemoteAPI.Data
{
    public static class RemoteStore
    {
        public static List<MessageDTO> remoteList = new List<MessageDTO>
        {
            new MessageDTO { Id = 1, Name = "Hello"},
            new MessageDTO { Id = 2, Name = "Goodbye"}
        };
    }
}
