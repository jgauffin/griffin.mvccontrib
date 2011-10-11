using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Griffin.MvcContrib.Html;

namespace HtmlHelpersDemo.Models
{
    public class ListModel
    {
        [Display(Description = "This will be shown as a tooltip thanks to the TooltipAdapter")]
        public User CurrentUser { get; set; }
        public IEnumerable<User> Users { get; set; }
    }

    public class UserCustomFormatter : ISelectItemFormatter
    {
        public SelectListItem Generate(object itm)
        {
            var item = (User)itm;
            return new SelectListItem
            {
                Text = string.Format("{0} {1}", item.FirstName, item.LastName),
                Value = item.Id.ToString()
            };
        }
    }

    public class UserFormatter : ReflectiveSelectItemFormatter
    {
        public UserFormatter()
            : base("Id", "FirstName")
        {

        }
    }
}