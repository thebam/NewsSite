using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewsSite.Models
{
    public class TagIndexViewModel : Tag
    {
        public TagIndexViewModel() {
            this.UsageCnt = 0;
        }
        [Display(Name ="Usage Count")]
        public int UsageCnt { get; set; }
        public List<Article> Articles { get; set; }
        public List<MediaKitFile> MediaKitFiles { get; set; }
    }
}
