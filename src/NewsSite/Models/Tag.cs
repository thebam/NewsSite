using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewsSite.Models
{
    public class Tag
    {
        public Tag() {
            this.Enabled = true;
            this.ArticleTags = new HashSet<ArticleTag>();
            this.MediaKitFileTags = new HashSet<MediaKitFileTag>();
        }
        public int TagId { get; set; }
        [Required]
        [Display(Name ="Tag")]
        public string TagName { get; set; }
        [Display(Name ="Date Created")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy h:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime DateCreated { get; set; }
        public bool Enabled { get; set; }
        public virtual ICollection<ArticleTag> ArticleTags { get; set; }
        public virtual ICollection<MediaKitFileTag> MediaKitFileTags { get; set; }
    }
}