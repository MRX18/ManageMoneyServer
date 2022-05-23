using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ManageMoneyServer.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Properties.Messages), ErrorMessageResourceName = "RequiredError")]
        [EmailAddress(ErrorMessageResourceType = typeof(Properties.Messages), ErrorMessageResourceName = "EmailError")]
        public string Email { get; set; }
        [Required(ErrorMessageResourceType = typeof(Properties.Messages), ErrorMessageResourceName = "RequiredError")]
        [StringLength(32, MinimumLength = 8, ErrorMessageResourceType = typeof(Properties.Messages), ErrorMessageResourceName = "StringLengthError")]
        public string Password { get; set; }
    }
}
