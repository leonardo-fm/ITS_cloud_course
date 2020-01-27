using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudSite.Models.Photos
{
    public class PhotoShare
    {
        public Photo PhotoToShare { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of expire")]
        public DateTime DateOfExpire { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Time of expire")]
        public TimeSpan TimeOfExpire { get; set; }
    }
}