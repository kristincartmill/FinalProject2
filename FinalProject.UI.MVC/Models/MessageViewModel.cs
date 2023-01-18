using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;//for attributes (validation/formatting)

namespace FinalProject.UI.MVC.Models
{
    public class MessageViewModel//represents a message user sends thru a form - we will email it 
    {
        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "A functional email address must be provided")]
        [Display(Name = "Email Address")]//used for dynamically genned labels
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required]
        [UIHint("MultilineText")]//changes the input control to a multi-line text box AKA <textarea>
        public string Message { get; set; }


    }
}
