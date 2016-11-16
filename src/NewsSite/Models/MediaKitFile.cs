using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewsSite.Models
{
    public class MediaKitFile
    {
        public MediaKitFile() {
            this.Enabled = true;
            this.ArticleMediaKitFiles = new HashSet<ArticleMediaKitFile>();
            this.MediaKitFileTags = new HashSet<MediaKitFileTag>();
        }
        public int MediaKitFileId { get; set; }
        [Required]
        public string URL { get; set; }
        public string ThumbnailURL { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int OwnerId { get; set; }
        public virtual Owner Owner { get; set; }
        public string MediaType { get; set; }
        [Required]
        [Display(Name = "Date Created")]
        [DisplayFormat(DataFormatString = "0:MM/dd/yyyy h:mm t")]
        public DateTime DateCreated { get; set; }
        [Required]
        [Display(Name = "Last Modified")]
        [DisplayFormat(DataFormatString = "0:MM/dd/yyyy h:mm t")]
        public DateTime DateModified { get; set; }
        public bool Enabled { get; set; }
        public virtual ICollection<ArticleMediaKitFile> ArticleMediaKitFiles { get; set; }
        public virtual ICollection<MediaKitFileTag> MediaKitFileTags { get; set; }
    }
}