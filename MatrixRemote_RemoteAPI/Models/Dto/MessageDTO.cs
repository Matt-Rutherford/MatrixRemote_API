using System.ComponentModel.DataAnnotations;

namespace MatrixRemote_RemoteAPI.Models.Dto
{
    public class MessageDTO
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Message { get; set; }
        [Required]
        [MaxLength(15)]
        public string Font { get; set; }
        public string ImageUrl {  get; set; } // delete


    }
}
