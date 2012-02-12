using System;
using System.Globalization;

namespace Griffin.MvcContrib.Localization
{
    /// <summary>
    ///   Sets the default culture (UICulture) which is used in the localization process
    /// </summary>
    /// <remarks>
    ///   The default culture will not get translation missing tags etc.
    /// </remarks>
    public static class DefaultUICulture
    {
        private static CultureInfo _culture = new CultureInfo(1033);

        /// <summary>
        ///   Gets current default culture
        /// </summary>
        public static CultureInfo Value
        {
            get { return _culture; }
        }

        /// <summary>
        /// Gets if english is used as default language.
        /// </summary>
        public static bool IsEnglish
        {
            get { return _culture.Name.StartsWith("en"); }
        }

        /// <summary>
        /// Gets if <see cref="CultureInfo.CurrentUICulture"/>  is the default culture
        /// </summary>
        public static bool IsActive
        {
            get { return _culture.LCID == CultureInfo.CurrentUICulture.LCID; }
        }


        /// <summary>
        /// Gets locale id
        /// </summary>
        public static int LCID
        {
            get { return _culture.LCID; }
        }

        /// <summary>
        /// Reset to framework default culture (1033)
        /// </summary>
        internal static void Reset()
        {
            _culture = new CultureInfo(1033);
        }

        /// <summary>
        ///   Sets the specified culture.
        /// </summary>
        /// <param name="culture"> The culture. </param>
        public static void Set(CultureInfo culture)
        {
            if (culture == null) throw new ArgumentNullException("culture");

            _culture = culture;
        }

        /// <summary>
        /// Gets if the default culture is the specified one.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static bool Is(CultureInfo culture)
        {
            if (culture == null) throw new ArgumentNullException("culture");

            if (culture.Name.Length > _culture.Name.Length)
                return culture.Name.StartsWith(_culture.Name);
            return _culture.Name.StartsWith(culture.Name);
        }
    }
}