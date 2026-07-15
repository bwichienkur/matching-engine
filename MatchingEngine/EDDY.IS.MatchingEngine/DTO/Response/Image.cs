using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    public enum ImageType
    {
        Logo = 1,
        Carousel = 2,
        Video = 3,
        YouVisitVideo = 4,
        Hero = 5,
        Tile = 6
    }

    [DataContract]
    public class Image
    {
        [DataMember]
        public int? Length { get; set; }
        [DataMember]
        public int? Breadth { get; set; }
        [DataMember]
        public string FileUrl { get; set; }
        [DataMember]
        public ImageType? ImageType { get; set; }
    }
}
