using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.MvcContrib.Localization.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Griffin.MvcContrib.Tests.Localization
{
    [TestClass]
    public class TypePromptTests
    {
        [TestMethod]
        public void LoadWrongVersion()
        {
            TypePrompt prompt = new TypePrompt();

            Assert.IsNull(prompt.TypeFullName);
        }
    }
}
