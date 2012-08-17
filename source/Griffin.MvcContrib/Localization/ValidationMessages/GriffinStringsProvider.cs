using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Griffin.MvcContrib.Localization.Types;

namespace Griffin.MvcContrib.Localization.ValidationMessages
{
    /// <summary>
    /// Uses <see cref="ILocalizedStringProvider"/> to find attribute translations.
    /// </summary>
    /// <remarks>Uses the <see cref="DependencyResolver"/> to find the localized string provider.</remarks>
    public class GriffinStringsProvider : IValidationMessageDataSource
    {
        private readonly ILocalizedStringProvider _stringProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="GriffinStringsProvider"/> class.
        /// </summary>
        /// <remarks>Use this constructor if you are using IoC (it will fetch the provider using <c>DependencyResolver</c>)</remarks>
        public GriffinStringsProvider()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GriffinStringsProvider"/> class.
        /// </summary>
        /// <param name="stringProvider">The string provider.</param>
        public GriffinStringsProvider(ILocalizedStringProvider stringProvider)
        {
            if (stringProvider == null) throw new ArgumentNullException("stringProvider");
            _stringProvider = stringProvider;
        }

        /// <summary>
        /// Gets the string provider.
        /// </summary>
        /// <returns></returns>
        protected virtual ILocalizedStringProvider GetStringProvider()
        {
            var provider = _stringProvider ??
                           (HttpContext.Current == null
                                ? null
                                : HttpContext.Current.Items["ILocalizedStringProvider"] as ILocalizedStringProvider);
            if (provider == null)
            {
                provider = DependencyResolver.Current.GetService<ILocalizedStringProvider>();
                if (HttpContext.Current != null)
                    HttpContext.Current.Items["ILocalizedStringProvider"] = provider;
            }


            if (provider == null)
                throw new InvalidOperationException(
                    "Failed to find an 'ILocalizedStringProvider' implementation. Either include one in the LocalizedModelMetadataProvider constructor, or register an implementation in your Inversion Of Control container.");

            return provider;

        }

        /// <summary>
        /// Get a validation message
        /// </summary>
        /// <param name="context"></param>
        /// <returns>
        /// String if found; otherwise <c>null</c>.
        /// </returns>
        public string GetMessage(IGetMessageContext context)
        {
            var provider = GetStringProvider();

            return provider.GetValidationString(context.Attribute.GetType(), context.ContainerType, context.PropertyName) ??
                        provider.GetValidationString(context.Attribute.GetType());
        }
    }
}
