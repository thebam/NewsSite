namespace NewsSite.Models
{
    public class ArticleMediaKitFile
    {
        public int ArticleMediaKitFileId { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; }
        public int MediaKitFileId { get; set; }
        public MediaKitFile MediaKitFile { get; set; }
    }
}
