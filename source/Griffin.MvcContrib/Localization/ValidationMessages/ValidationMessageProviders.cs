using System;
using System.Collections.Generic;

namespace Griffin.MvcContrib.Localization.ValidationMessages
{
    /// <summary>
    /// Used to supply custom validation messages for attributes.
    /// </summary>
    /// <remarks>
    /// <para>This class is called by <see cref="LocalizedModelValidatorProvider"/>
    /// for each attribute that a property have. It's responsibility is to use all of the language providers to get the correct translation.
    /// The returned message should have formatters in them.</para>
    /// <para>
    /// Default setup is using <see cref="GriffinStringsProvider"/> as default and <see cref="MvcDataSource"/> + <see cref="DataAnnotationDefaultStrings"/> as fallback.
    /// </para>
    /// </remarks>
    public class ValidationMessageProviders
    {
        private static readonly List<IValidationMessageDataSource> _dataSources = new List<IValidationMessageDataSource>();

        static ValidationMessageProviders()
        {
           Reset();
        }

        /// <summary>
        /// reset to the default providers.
        /// </summary>
        public static void Reset()
        {
            Clear();
            Add(new GriffinStringsProvider());
            Add(new MvcDataSource());
            Add(new DataAnnotationDefaultStrings());
        }
        /// <summary>
        /// Add another provider (last)
        /// </summary>
        /// <param name="provider">Provider to add</param>
        public static void Add(IValidationMessageDataSource provider)
        {
            if (provider == null) throw new ArgumentNullException("provider");
            _dataSources.Add(provider);
        }

        /// <summary>
        /// Remove all providers
        /// </summary>
        public static void Clear()
        {
            _dataSources.Clear();
        }

        /// <summary>
        /// Get a message
        /// </summary>
        /// <param name="context">Context information</param>
        /// <returns>String if found; otherwise <c>null</c>.</returns>
        public static string GetMessage(IGetMessageContext context)
        {
            foreach (var dataSource in _dataSources)
            {
                var msg = dataSource.GetMessage(context);
                if (msg != null)
                    return msg;
            }

            return null;
        }
    }
}