using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Griffin.MvcContrib.Admin.TestProject.Models
{
    public class HomeViewModel
    {
        [Required]
        [StringLength(40)]
        public string Name { get; set; }

        [Required]
        public int Age { get; set; }
    }
}