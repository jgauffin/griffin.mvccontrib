using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SqlServerLocalization.Models
{
    public class User
    {
        public string Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required, StringLength(20)]
        public string LastName { get; set; }

        public string Title { get; set; }

    }
}