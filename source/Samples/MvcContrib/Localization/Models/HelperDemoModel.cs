using System.Collections.Generic;
using System.Web.Mvc;

namespace Localization.Models
{
    public class HelperDemoModel
    {
        public InputType InputType { get; set; }
        public IEnumerable<UserViewModel> Users { get; set; }
        public UserViewModel User { get; set; }
        public IEnumerable<int> Ages { get; set; }
        public int Age { get; set; }
    }
}