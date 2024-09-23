using System.ComponentModel.DataAnnotations;

namespace MatrixRemote_RemoteAPI.Models
{
    public class Remote
    {
        public int Id { get; set; }
        
        public string Message { get; set; }

        public string Font { get; set; }

        public string ImageUrl { get; set; } // delete

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
