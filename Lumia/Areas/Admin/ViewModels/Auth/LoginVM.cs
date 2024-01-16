using System.ComponentModel.DataAnnotations;

namespace Lumia.Areas.Admin.ViewModels
{
    public class LoginVM
    {
        public string UserNameorEmail { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
