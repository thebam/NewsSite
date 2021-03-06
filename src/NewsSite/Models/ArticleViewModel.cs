﻿
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewsSite.Models
{
    public class ArticleViewModel
    {
        public ArticleViewModel()
        {
            this.ArticleMediaKitFiles = new HashSet<ArticleMediaKitFile>();
            this.ArticleTags = new HashSet<ArticleTag>();
        }
        public int ArticleId { get; set; }
        [Required]
        [MaxLength(256, ErrorMessage = "Title must be 256 characters or less.")]
        public string Title { get; set; }
        [Required]
        [MaxLength(256, ErrorMessage = "URL must be 256 characters or less.")]
        public string URL { get; set; }
        [Required]
        public string Body { get; set; }
        [Required]
        [Display(Name = "Social Media Title")]
        [MaxLength(140, ErrorMessage = "Open Graph Title must be 140 characters or less.")]
        public string OGTitle { get; set; }
        [Required]
        [Display(Name = "Social Media Description")]
        [MaxLength(140, ErrorMessage = "Open Graph Description must be 140 characters or less.")]
        public string OGDescription { get; set; }
        [Required]
        [Display(Name = "Social Media Image")]
        public string OGImage { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "0:MM/dd/yyyy h:mm t")]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "0:MM/dd/yyyy h:mm t")]
        public DateTime EndDate { get; set; }
        public virtual ICollection<ArticleMediaKitFile> ArticleMediaKitFiles { get; set; }
        public virtual ICollection<ArticleTag> ArticleTags { get; set; }
    }
}
