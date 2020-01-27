using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace CloudSite.Models.User
{
    public class UserModel
    {
        public ObjectId _id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Username")]
        public string UserName{ get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string UserEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string UserPassword { get; set; }

        public bool ConfirmedEmail { get; set; }
    }
}