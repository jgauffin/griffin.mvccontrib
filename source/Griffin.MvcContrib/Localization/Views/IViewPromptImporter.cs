using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.MvcContrib.Localization.Views
{
    /// <summary>
    /// Can import view prompts from an external source
    /// </summary>
    public interface IViewPromptImporter
    {
        /// <summary>
        /// Imports a collection of view prompts into the data source
        /// </summary>
        /// <param name="viewPrompts">Collection of prompts to import</param>
        /// <remarks>All existing view prompts should be replaced with those that are being defined
        /// in the list.</remarks>
        void Import(IEnumerable<ViewPrompt> viewPrompts);
    }
}
