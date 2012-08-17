using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using Griffin.MvcContrib.Localization.Types;

namespace Griffin.MvcContrib.Localization.ValidationMessages
{
    /// <summary>
    /// Loads the default DataAnnotation strings from the resource file System.ComponentModel.DataAnnotations.Resources.DataAnnotationsResources
    /// </summary>
    /// <remarks>Do note that resource files can fallback to default culture (and therefore return the incorrect language)</remarks>
    public class DataAnnotationDefaultStrings : IValidationMessageDataSource
    {
        private readonly ResourceManager _resourceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationAttributesStringProvider"/> class.
        /// </summary>
        public DataAnnotationDefaultStrings()
        {
            var resourceStringType =
                typeof(RequiredAttribute).Assembly.GetType(
                    "System.ComponentModel.DataAnnotations.Resources.DataAnnotationsResources");

            if (resourceStringType == null)
                return;

            _resourceManager = new ResourceManager(resourceStringType);
        }


        /// <summary>
        ///   Get all strings.
        /// </summary>
        /// <param name="culture">Culture to get prompts for</param>
        /// <returns>A colleciton of prompts (or an empty collection)</returns>
        public virtual IEnumerable<TypePrompt> GetPrompts(CultureInfo culture)
        {
            if (culture == null) throw new ArgumentNullException("culture");

            var prompts = new List<TypePrompt>();

            var baseAttribte = typeof(ValidationAttribute);
            var attributes =
                typeof(RequiredAttribute).Assembly.GetTypes().Where(
                    p => baseAttribte.IsAssignableFrom(p) && !p.IsAbstract).ToList();
            foreach (var type in attributes)
            {
                var key = new TypePromptKey(type.FullName, "class");
                var typePrompt = new TypePrompt
                                     {
                                         Key = key,
                                         LocaleId = CultureInfo.CurrentUICulture.LCID,
                                         TypeFullName = type.FullName,
                                         TextName = "class",
                                         UpdatedAt = DateTime.Now,
                                         UpdatedBy = Thread.CurrentPrincipal.Identity.Name
                                     };

                var value = GetString(type, culture);
                if (value != null)
                {
                    typePrompt.TranslatedText = DefaultUICulture.IsActive ? value : "";
                }

                prompts.Add(typePrompt);
            }

            return prompts;
        }

        /// <summary>
        /// Get the localized text
        /// </summary>
        /// <param name="type">Validation attribute type.</param>
        /// <param name="culture">Culture to get for </param>
        /// <returns>Text if found; otherwise null</returns>
        public virtual string GetString(Type type, CultureInfo culture)
        {
            var resourceName = string.Format("{0}_ValidationError", type.Name);

            if (culture.Name.StartsWith("en"))
                return _resourceManager.GetString(resourceName, culture);

            var rs = _resourceManager.GetResourceSet(culture, false, true);
            return rs == null ? null : rs.GetString(resourceName);
        }

        public string GetMessage(IGetMessageContext context)
        {
            
            return GetString(context.Attribute.GetType(), context.CultureInfo);
        }
    }
}
