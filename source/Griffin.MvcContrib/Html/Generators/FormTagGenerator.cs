/*
 * Copyright (c) 2011, Jonas Gauffin. All rights reserved.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
 * MA 02110-1301 USA
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Types;

namespace Griffin.MvcContrib.Html.Generators
{
    /// <summary>
    /// Base class for all FORM tag generators
    /// </summary>
    /// <remarks>
    /// Tag generators are used in MVC views to generate HTML tags with the help of html helpers.
    /// </remarks>
    public abstract class FormTagGenerator
    {
        private readonly ViewContext _viewContext;
        private ILocalizedStringProvider _languageProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormTagGenerator"/> class.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        protected FormTagGenerator(ViewContext viewContext)
        {
            _viewContext = viewContext;
        }

        /// <summary>
        /// Gets generator context
        /// </summary>
        /// <remarks>Contains information used to generate tags such as ModelMetaData.</remarks>
        protected GeneratorContext Context { get; private set; }

        /// <summary>
        /// Gets provider used to load localized strings from any source
        /// </summary>
        protected ILocalizedStringProvider LocalizedStringProvider
        {
            get
            {
                return _languageProvider ??
                       (_languageProvider = DependencyResolver.Current.GetService<ILocalizedStringProvider>() ??
                                            new MetadataLanguageProvider());
            }
        }

        /// <summary>
        /// Generate options
        /// </summary>
        /// <param name="items"></param>
        /// <param name="selectedValue"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        protected IEnumerable<NestedTagBuilder> GenerateOptions(IEnumerable items, string selectedValue,
                                                                ISelectItemFormatter formatter)
        {
            if (formatter == null)
                return GenerateOptions(items, selectedValue);

            var listItems = new List<NestedTagBuilder>();
            foreach (var item in items)
            {
                var tag = new NestedTagBuilder("option");
                var listItem = formatter.Generate(item);
                tag.MergeAttribute("value", listItem.Value);
                if (listItem.Value == selectedValue || listItem.Selected)
                    tag.MergeAttribute("selected", "selected");
                tag.SetInnerText(listItem.Text);
                listItems.Add(tag);
            }

            return listItems;
        }

        protected IEnumerable<NestedTagBuilder> GenerateOptions(IEnumerable items, string selectedValue)
        {
            var listItems = new List<NestedTagBuilder>();
            foreach (SelectListItem listItem in items)
            {
                var tag = new NestedTagBuilder("option");
                tag.MergeAttribute("value", listItem.Value);
                if (listItem.Value == selectedValue || listItem.Selected)
                    tag.MergeAttribute("selected", "selected");
                tag.SetInnerText(listItem.Text);
                listItems.Add(tag);
            }

            return listItems;
        }

        /// <summary>
        /// I know, really. Setup/Init methods are a pain and spawn of satan and all that. But I couldn't figure out a  better solution.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void Setup(GeneratorContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Generate HTML tags for a property
        /// </summary>
        /// <param name="context">Context specific information</param>
        /// <returns>Generated HTML tags</returns>
        public IEnumerable<NestedTagBuilder> Generate(GeneratorContext context)
        {
            Setup(context);
            return GenerateTags();
        }

        protected abstract IEnumerable<NestedTagBuilder> GenerateTags();


        protected NestedTagBuilder CreatePrimaryTag(string tagName)
        {
            var tagBuilder = new NestedTagBuilder(tagName);
            tagBuilder.MergeAttributes(Context.HtmlAttributes);
            tagBuilder.MergeAttribute("name", Context.FullName, true);
            tagBuilder.GenerateId(Context.FullName);
            SetValidationState(tagBuilder, Context.FullName);
            return tagBuilder;
        }

        /// <summary>
        /// Get value for the model
        /// </summary>
        /// <returns>Model value</returns>
        /// <remarks>Value will either be the one from the previous POST or the one assigned in the model.</remarks>
        protected string GetValue()
        {
            ModelState modelState;
            if (_viewContext.ViewData.ModelState.TryGetValue(Context.Name, out modelState) && modelState.Value != null)
            {
                return Convert.ToString(modelState.Value, CultureInfo.CurrentUICulture);
            }

            return !string.IsNullOrEmpty(Context.Metadata.EditFormatString)
                       ? string.Format(Context.Metadata.EditFormatString, Context.Metadata.Model)
                       : Convert.ToString(Context.Metadata.Model, CultureInfo.CurrentUICulture);
        }

        protected string GetFullHtmlFieldName(string name)
        {
            return _viewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
        }

        private void SetValidationState(NestedTagBuilder tagBuilder, string fullName)
        {
            ModelState modelState;
            if (_viewContext.ViewData.ModelState.TryGetValue(fullName, out modelState) && modelState.Errors.Count > 0)
            {
                tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            }
        }
    }
}