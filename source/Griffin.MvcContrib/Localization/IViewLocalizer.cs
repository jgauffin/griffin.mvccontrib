// -----------------------------------------------------------------------
// <copyright file="IViewLocalizer.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Griffin.MvcContrib.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Used to localize strings in a view.
    /// </summary>
    public interface IViewLocalizer
    {
        string Translate(string controllerName, string actionName, string text);
    }
}
