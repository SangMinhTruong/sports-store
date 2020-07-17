namespace SportsStore.Models.Photos
{
    public class Photo
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public Product product { get; set; }
        public int ProductId { get; set; }
    }
}