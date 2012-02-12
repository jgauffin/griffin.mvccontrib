using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        #region ISelectItemFormatter Members

        public SelectListItem Generate(object itm)
        {
            var item = (User) itm;
            return new SelectListItem
                       {
                           Text = string.Format("{0} {1}", item.FirstName, item.LastName),
                           Value = item.Id.ToString()
                       };
        }

        #endregion
    }

    public class UserFormatter : ReflectiveSelectItemFormatter
    {
        public UserFormatter()
            : base("Id", "FirstName")
        {
        }
    }
}