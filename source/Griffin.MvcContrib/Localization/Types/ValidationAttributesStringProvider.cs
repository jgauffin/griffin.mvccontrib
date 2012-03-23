using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;

namespace Griffin.MvcContrib.Localization.Types
{
    /// <summary>
    ///   Used to get default validation messages
    /// </summary>
    /// <remarks>
    ///   Current implementation haxx0r ASP.NET MVC3 to get the hidden resource strings. this also means that it might not work with other implementations like Mono
    /// </remarks>
    public class ValidationAttributesStringProvider
    {
        private static ValidationAttributesStringProvider _provider = new ValidationAttributesStringProvider();
        private readonly ResourceManager _resourceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationAttributesStringProvider"/> class.
        /// </summary>
        public ValidationAttributesStringProvider()
        {
            var resourceStringType =
                typeof (RequiredAttribute).Assembly.GetType(
                    "System.ComponentModel.DataAnnotations.Resources.DataAnnotationsResources");

            if (resourceStringType == null)
                return;

            _resourceManager = new ResourceManager(resourceStringType);
        }


        /// <summary>
        ///   Gets current implementation
        /// </summary>
        public static ValidationAttributesStringProvider Current
        {
            get { return _provider; }
        }

        /// <summary>
        ///   Assign a new custom provider.
        /// </summary>
        /// <param name="provider"> </param>
        public static void Assign(ValidationAttributesStringProvider provider)
        {
            if (provider == null) throw new ArgumentNullException("provider");
            _provider = provider;
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

            var baseAttribte = typeof (ValidationAttribute);
            var attributes =
                typeof (RequiredAttribute).Assembly.GetTypes().Where(
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
            return _resourceManager == null
                       ? null
                       : _resourceManager.GetString(string.Format("{0}_ValidationError", type.Name), culture);
        }
    }
}