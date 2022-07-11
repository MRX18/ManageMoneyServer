using System.ComponentModel.DataAnnotations;

namespace ManageMoneyServer.Models.ViewModels
{
    public class RegisterViewModel : LoginViewModel
    {
        [Display(Name = "FullName", ResourceType = typeof(Resources.Fields))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.Messages))]
        [StringLength(32, MinimumLength = 2, ErrorMessageResourceName = "StringLengthError", ErrorMessageResourceType = typeof(Resources.Messages))]
        public string FullName { get; set; }
        [Display(Name = "ConfirmPassword", ResourceType = typeof(Resources.Fields))]
        [Compare("Password", ErrorMessageResourceName = "CompareError", ErrorMessageResourceType = typeof(Resources.Messages))]
        public string ConfirmPassword { get; set; }
    }
}
