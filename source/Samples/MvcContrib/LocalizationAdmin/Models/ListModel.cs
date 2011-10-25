using System.Collections.Generic;
using System.Web.Mvc;

namespace LocalizationAdmin.Models
{
	public class ListModel
	{
		public IEnumerable<TypePrompt> Prompts { get; set; }

		public IEnumerable<SelectListItem> Languages { get; set; }
		public bool ShowMetadata { get; set; }
	}
}