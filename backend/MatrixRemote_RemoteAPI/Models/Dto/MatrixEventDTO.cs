using System.ComponentModel.DataAnnotations;

namespace MatrixRemote_RemoteAPI.Models.Dto
{
    public class MatrixEventDTO
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        public GeoLocationDTO? Location { get; set; }
        public EventTypeDTO Type { get; set; }
        public RgbColorDTO? Color { get; set; }       // Nullable RGB color for content

    }

    public enum EventTypeDTO
    {
        Text,
        Image,  // for static images
        GIF     // for animated GIFs
    }
    public class GeoLocationDTO
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class RgbColorDTO
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
    }


}
