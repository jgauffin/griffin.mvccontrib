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
            prompt.SubjectTypeName = GetType().AssemblyQualifiedName.Replace("1.0.0.0", "3.0.0.0");

            Assert.NotNull(prompt.TypeFullName);
        }
    }
}
