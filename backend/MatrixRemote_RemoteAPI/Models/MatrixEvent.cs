using System.ComponentModel.DataAnnotations;

namespace MatrixRemote_RemoteAPI.Models
{
    public class MatrixEvent
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public GeoLocation? Location { get; set; }  //nullable to allow no location
        public EventType Type { get; set; }
        public RgbColor? Color { get; set; }       // Nullable RGB color for content

        public class GeoLocation
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        public enum EventType
        {
            Text,
            Image,  // for static images
            GIF     // for animated GIFs
        }

        public class RgbColor
        {
            public int R { get; set; }
            public int G { get; set; }
            public int B { get; set; }
        }


    }
}
