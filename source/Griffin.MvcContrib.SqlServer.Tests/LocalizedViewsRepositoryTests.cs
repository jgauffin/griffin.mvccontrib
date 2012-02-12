using System.Globalization;
using Griffin.MvcContrib.Localization.Views;
using Griffin.MvcContrib.SqlServer.Localization;
using Xunit;

namespace Griffin.MvcContrib.SqlServer.Tests
{
    public class LocalizedViewsRepositoryTests
    {
        private const string ViewPath = "/myarea/controller/index";

        private const string TextName =
            "This is a text that should be translated since it contains a lot of things and so.";

        private readonly ConnectionFactory _factory = new ConnectionFactory();
        private readonly ViewPromptKey _key = new ViewPromptKey(ViewPath, TextName);
        private readonly SqlLocalizedViewsRepository _repository;

        public LocalizedViewsRepositoryTests()
        {
            _repository = new SqlLocalizedViewsRepository(_factory);
        }

        [Fact]
        public void GetNonExistant()
        {
            var p = _repository.GetPrompt(new CultureInfo(1053), new ViewPromptKey("/some/action/", "forme"));
            Assert.Null(p);
        }

        [Fact]
        public void TestCreateLang()
        {
            using (var cmd = _factory.Connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM LocalizedViews WHERE LocaleId = 1044";
                cmd.ExecuteNonQuery();
            }

            _repository.CreateLanguage(new CultureInfo(1044), new CultureInfo(1033));
        }


        [Fact]
        public void GetExisting()
        {
            _repository.Save(new CultureInfo(1053), ViewPath, TextName, "Förnamn");

            var prompt = _repository.GetPrompt(new CultureInfo(1053), _key);
            Assert.NotNull(prompt);
            Assert.Equal("Förnamn", prompt.TranslatedText);
        }

        [Fact]
        public void Update()
        {
            _repository.Save(new CultureInfo(1053), ViewPath, TextName, "Förrenammne");
            var prompt = _repository.GetPrompt(new CultureInfo(1053), _key);
            Assert.NotNull(prompt);
            Assert.Equal("Förrenammne", prompt.TranslatedText);
        }

        [Fact]
        public void TwoLanguages()
        {
            _repository.Save(new CultureInfo(1033), ViewPath, TextName, "FirstName");
            _repository.Save(new CultureInfo(1053), ViewPath, TextName, "Förnamn");


            var enprompt = _repository.GetPrompt(new CultureInfo(1033), _key);
            var seprompt = _repository.GetPrompt(new CultureInfo(1053), _key);
            Assert.NotNull(enprompt);
            Assert.NotNull(seprompt);
            Assert.NotEqual(enprompt.TranslatedText, seprompt.TranslatedText);
        }
    }
}