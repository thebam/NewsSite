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
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTime DateCreated { get; set; }
        [Required]
        [Display(Name = "Last Modified")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTime DateModified { get; set; }
        [Required]
        [Display(Name = "Copyright Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime CopyrightDate { get; set; }
        [Required]
        public String AltText { get; set; }
        public bool Enabled { get; set; }
        public Int32 DownloadCnt { get; set; }
        public virtual ICollection<ArticleMediaKitFile> ArticleMediaKitFiles { get; set; }
        public virtual ICollection<MediaKitFileTag> MediaKitFileTags { get; set; }
    }
}