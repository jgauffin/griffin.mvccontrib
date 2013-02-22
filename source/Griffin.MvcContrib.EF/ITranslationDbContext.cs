using Griffin.MvcContrib.Localization.Types;
using Griffin.MvcContrib.Localization.Views;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Griffin.MvcContrib.EF
{
    public interface ITranslationDbContext
    {
        IDbSet<LocalizedType> LocalizedTypes { get; set; }
        IDbSet<LocalizedView> LocalizedViews { get; set; }

        void Save();
    }
}
