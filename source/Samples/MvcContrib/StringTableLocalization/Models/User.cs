using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcApplication2.Models
{
    public class User
    {
        [Required]
        public string FirstName { get; set; }

        [StringLength(20), Required]
        public string LastName { get; set; }

    }
}