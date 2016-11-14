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
        }
        public int TagId { get; set; }
        [Required]
        [Display(Name ="Tag")]
        public string TagName { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Enabled { get; set; }
        public virtual ICollection<ArticleTag> ArticleTags { get; set; }
    }
}