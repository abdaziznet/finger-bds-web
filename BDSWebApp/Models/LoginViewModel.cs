using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BDSWebApp.Models
{
    public class LoginViewModel
    {
        [Display(Name = "Teller Number")]
        public string TellerNumber { get; set; }
    }
}
