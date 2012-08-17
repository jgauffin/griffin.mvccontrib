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
        protected virtual ILocalizedStringProvider GetStringProvider()
        {
            var provider = HttpContext.Current == null ? null : HttpContext.Current.Items["ILocalizedStringProvider"] as ILocalizedStringProvider;
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
        public string GetMessage(IGetMessageContext context)
        {
            var provider = GetStringProvider();

            return provider.GetValidationString(context.Attribute.GetType(), context.ContainerType, context.PropertyName) ??
                        provider.GetValidationString(context.Attribute.GetType());
        }
    }
}
