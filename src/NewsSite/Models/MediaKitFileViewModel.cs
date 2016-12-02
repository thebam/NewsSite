using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsSite.Models
{
    public class MediaKitFileViewModel
    {
        public int MediaKitFileId { get; set; }
        public string URL { get; set; }
        public string Description { get; set; }
        public DateTime CopyrightDate { get; set; }
        public List<TagName> TagNames { get; set; }
    }

    public class TagName
    {
        public string Name { get; set; }
    }
}
