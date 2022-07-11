using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ManageMoneyServer.Models.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Email", ResourceType = typeof(Resources.Fields))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.Messages))]
        [EmailAddress(ErrorMessageResourceName = "EmailError", ErrorMessageResourceType = typeof(Resources.Messages))]
        public string Email { get; set; }
        [Display(Name = "Password", ResourceType = typeof(Resources.Fields))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.Messages))]
        [StringLength(32, MinimumLength = 8, ErrorMessageResourceName = "StringLengthError", ErrorMessageResourceType = typeof(Resources.Messages))]
        public string Password { get; set; }
    }
}
