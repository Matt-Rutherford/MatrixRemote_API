using System.ComponentModel.DataAnnotations;

namespace MatrixRemote_RemoteAPI.Models.Dto
{
    public class MessageDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
