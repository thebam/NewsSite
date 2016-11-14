using System;
using System.ComponentModel.DataAnnotations;

namespace NewsSite.Models
{
    public class Owner
    {
        public Owner() {
            this.Enabled = true;
        }
        public int OwnerId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string SocialMedia { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Enabled { get; set; }
    }
}