using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Admin.TestProject.Models
{
    public class HomeViewModel
    {
        [Required()]
        [StringLength(40)]
        public string Name { get; set; }

        [Required, Compare("Name", ErrorMessage = "Ange messy")]
        public int Age { get; set; }
    }
}