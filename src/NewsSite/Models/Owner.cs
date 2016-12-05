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
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Please enter a valid phone number")]
        public string Phone { get; set; }
        public string Address { get; set; }
        [DataType(DataType.Url,ErrorMessage = "Please enter a valid website URL")]
        public string Website { get; set; }
        [Display(Name ="Social Media Account")]
        public string SocialMedia { get; set; }
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        public bool Enabled { get; set; }
    }
}