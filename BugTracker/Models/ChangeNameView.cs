using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class ChangeNameView
    {
        

        [Required]
        [StringLength(20, ErrorMessage = "Enter your First Name", MinimumLength = 1)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        //Last Name
        [Required]
        [StringLength(20, ErrorMessage = "Enter your Last Name", MinimumLength = 1)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
    }
}