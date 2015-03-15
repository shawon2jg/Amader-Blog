using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectBlog.Models
{
    public class UserView
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "Enter user name", AllowEmptyStrings = false)]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "User name must be minimum 5 characters")]
        public string UserName { set; get; }

        [Required(ErrorMessage = "Enter email")]
        [StringLength(100)]
        [RegularExpression(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$",
        ErrorMessage = "Enter valid email")]
        public string Email { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Enter password", AllowEmptyStrings = false)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be minimum 8 characters")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Password does not match.")]
        public string ConfirmPassword { get; set; }
    }
}