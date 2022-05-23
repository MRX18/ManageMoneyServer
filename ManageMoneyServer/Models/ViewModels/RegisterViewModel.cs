using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ManageMoneyServer.Models.ViewModels
{
    public class RegisterViewModel : LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Properties.Messages), ErrorMessageResourceName = "RequiredError")]
        [StringLength(32, MinimumLength = 2, ErrorMessageResourceType = typeof(Properties.Messages), ErrorMessageResourceName = "StringLengthError")]
        public string FullName { get; set; }
        [Compare("Password", ErrorMessageResourceType = typeof(Properties.Messages), ErrorMessageResourceName = "CompareError")]
        public string ConfirmPassword { get; set; }
    }
}
