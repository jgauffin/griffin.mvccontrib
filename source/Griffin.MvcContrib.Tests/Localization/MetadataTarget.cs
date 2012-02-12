using System.ComponentModel.DataAnnotations;

namespace Griffin.MvcContrib.Tests.Localization
{
    public class MetadataTarget
    {
        [Required]
        public string UserName { get; set; }

        [Required, StringLength(40)]
        public string LastName { get; set; }
    }
}