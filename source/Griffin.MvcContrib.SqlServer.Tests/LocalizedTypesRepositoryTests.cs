using System.Globalization;
using Griffin.MvcContrib.Localization.Types;
using Griffin.MvcContrib.SqlServer.Localization;
using Xunit;

namespace Griffin.MvcContrib.SqlServer.Tests
{
    public class LocalizedTypesRepositoryTests
    {
        private ConnectionFactory _factory = new ConnectionFactory();
        private SqlLocalizedTypesRepository _repository ;

        public LocalizedTypesRepositoryTests()
        {
            _repository = new SqlLocalizedTypesRepository(_factory);
        }

        [Fact]
        public void GetNonExistant()
        {
            var p = _repository.GetPrompt(new CultureInfo(1053), new TypePromptKey(typeof (TestType), "FirstNameOrSomething"));
            Assert.Null(p);
        }

        [Fact]
        public void TestCreateLang()
        {
            using (var cmd = _factory.Connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM LocalizedTypes WHERE LocaleId = 1044";
                cmd.ExecuteNonQuery();
            }

            _repository.CreateLanguage(new CultureInfo(1044), new CultureInfo(1033));
        }

        [Fact]
        public void GetExisting()
        {
            var key = new TypePromptKey(typeof (TestType), "FirstName");
            _repository.Save(new CultureInfo(1053),  typeof(TestType), "FirstName", "Förnamn");

            var prompt = _repository.GetPrompt(new CultureInfo(1053), key);
            Assert.NotNull(prompt);
            Assert.Equal("Förnamn", prompt.TranslatedText);
        }

        [Fact]
        public void Update()
        {
            var key = new TypePromptKey(typeof(TestType), "FirstName");
            _repository.Update(new CultureInfo(1053), key, "Förrenammne");
            var prompt = _repository.GetPrompt(new CultureInfo(1053), key);
            Assert.NotNull(prompt);
            Assert.Equal("Förrenammne", prompt.TranslatedText);
        }

        [Fact]
        public void TwoLanguages()
        {
            var key = new TypePromptKey(typeof(TestType), "FirstName");
            _repository.Save(new CultureInfo(1033), typeof(TestType), "FirstName", "FirstName");
            _repository.Save(new CultureInfo(1053), typeof(TestType), "FirstName", "Förnamn");


            var enprompt = _repository.GetPrompt(new CultureInfo(1033), key);
            var seprompt = _repository.GetPrompt(new CultureInfo(1053), key);
            Assert.NotNull(enprompt);
            Assert.NotNull(seprompt);
            Assert.NotEqual(enprompt.TranslatedText, seprompt.TranslatedText);
        }

    }
}