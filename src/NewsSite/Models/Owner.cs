using System;
using System.ComponentModel.DataAnnotations;

namespace NewsSite.Models
{
    public class Owner
    {
        public Owner() {
            this.Enabled = true;
            this.DateCreated = DateTime.Now;
        }
        public int OwnerId { get; set; }
        [Required]
        public string Name { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        public string Phone { get; set; }
        public string Address { get; set; }
        [DataType(DataType.Url)]
        public string Website { get; set; }
        public string SocialMedia { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Enabled { get; set; }
    }
}