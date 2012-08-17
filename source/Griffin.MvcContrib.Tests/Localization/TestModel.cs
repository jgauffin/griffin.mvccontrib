using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Tests.Localization
{
    public class TestModel : IValidatableObject
    {
        [Required]
        public string Required { get; set; }

        [Required, StringLength(40)]
        public string RequiredStringLength10 { get; set; }


        [Compare("Compare2")]
        public string Compare1 { get; set; }
        public string Compare2 { get; set; }

        [Required(ErrorMessage = "Custom message")]
        public string CustomMessage { get; set; }

        [Required(ErrorMessageResourceName = "ResourceFile", ErrorMessageResourceType = typeof(Messages))]
        public string ResourceFile { get; set; }

        [RegularExpression(@"[\d]+")]
        public string RegEx { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new [] { new ValidationResult("King size bed"), };
        }
    }
}