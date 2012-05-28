using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.MvcContrib.Localization.Types;
using Xunit;

namespace Griffin.MvcContrib.Tests.Localization
{
    public class TypePromptTests
    {
        [Fact]
        public void LoadWrongVersion()
        {
            TypePrompt prompt = new TypePrompt();

            Assert.NotNull(prompt.TypeFullName);
        }
    }
}
