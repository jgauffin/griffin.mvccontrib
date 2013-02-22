using Griffin.MvcContrib.Localization.Types;
using Griffin.MvcContrib.Localization.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.MvcContrib.EF
{
    public class TranslationDbContext : DbContext, ITranslationDbContext
    {
        public IDbSet<LocalizedType> LocalizedTypes { get; set; }
        public IDbSet<LocalizedView> LocalizedViews { get; set; }

        public void Save()
        {
            this.SaveChanges();
        }
    }
}
