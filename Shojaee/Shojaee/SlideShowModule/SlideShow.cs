using MongoDB.Entities;

namespace Shojaee.SlideShowModule
{
    public class SlideShow : Entity
    {
        public string ImageAddress { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string ButtonTitle { get; set; }
        public string ButtonLink { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
