using System.Globalization;
using Griffin.MvcContrib.Localization.Types;
using Griffin.MvcContrib.SqlServer.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Assert = Xunit.Assert;

namespace Griffin.MvcContrib.SqlServer.Tests
{
    [TestClass]
    public class LocalizedTypesRepositoryTests
    {
        public const string Schema =
            @"CREATE TABLE LocalizedTypes(
	Id int IDENTITY(1,1) NOT NULL,
	LocaleId int NOT NULL,
	[Key] nvarchar(250) NOT NULL,
	TypeName nvarchar(255) NOT NULL,
	TextName nvarchar(250) NOT NULL,
	UpdatedAt datetime NOT NULL,
	UpdatedBy nvarchar(50) NOT NULL,
	Value nvarchar(2000) NOT NULL
);


CREATE TABLE LocalizedViews(
	Id int IDENTITY(1,1) NOT NULL,
	LocaleId int NOT NULL,
	[Key] nvarchar(50) NOT NULL,
	ViewPath nvarchar(255) NOT NULL,
	TextName nvarchar(2000) NOT NULL,
	Value nvarchar(2000) NOT NULL,
	UpdatedAt datetime NOT NULL,
	UpdatedBy nvarchar(50) NOT NULL
);";
        private readonly SqlExpressConnectionFactory _factory = new SqlExpressConnectionFactory(Schema);
        private SqlLocalizedTypesRepository _repository;

        [TestInitialize]
        public void Init()
        {
            _repository = new SqlLocalizedTypesRepository(_factory);
        }


        [TestCleanup]
        public void Cleanup()
        {
            _factory.Dispose();
        }

        [TestMethod]
        public void GetNonExistant()
        {
            var p = _repository.GetPrompt(new CultureInfo(1053),
                                          new TypePromptKey(typeof (TestType), "FirstNameOrSomething"));
            Assert.Null(p);
        }

        [TestMethod]
        public void TestCreateLang()
        {
            using (var cmd = _factory.Connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM LocalizedTypes WHERE LocaleId = 1044";
                cmd.ExecuteNonQuery();
            }

            _repository.CreateLanguage(new CultureInfo(1044), new CultureInfo(1033));
        }

        [TestMethod]
        public void GetExisting()
        {
            var key = new TypePromptKey(typeof (TestType), "FirstName");
            _repository.Save(new CultureInfo(1053), typeof (TestType), "FirstName", "Förnamn");

            var prompt = _repository.GetPrompt(new CultureInfo(1053), key);
            Assert.NotNull(prompt);
            Assert.Equal("Förnamn", prompt.TranslatedText);
        }

        [TestMethod]
        public void Update()
        {
            var key = new TypePromptKey(typeof (TestType), "FirstName");
            _repository.Update(new CultureInfo(1053), key, "Förrenammne");
            var prompt = _repository.GetPrompt(new CultureInfo(1053), key);
            Assert.Null(prompt);
        }

        [TestMethod]
        public void Save()
        {
            var key = new TypePromptKey(typeof(TestType), "FirstName");
            _repository.Save(new CultureInfo(1053), typeof(TestType), "FirstName", "Förrenammne");
            var prompt = _repository.GetPrompt(new CultureInfo(1053), key);
            Assert.NotNull(prompt);
            Assert.Equal("Förrenammne", prompt.TranslatedText);
        }

        [TestMethod]
        public void TwoLanguages()
        {
            var key = new TypePromptKey(typeof (TestType), "FirstName");
            _repository.Save(new CultureInfo(1033), typeof (TestType), "FirstName", "FirstName");
            _repository.Save(new CultureInfo(1053), typeof (TestType), "FirstName", "Förnamn");


            var enprompt = _repository.GetPrompt(new CultureInfo(1033), key);
            var seprompt = _repository.GetPrompt(new CultureInfo(1053), key);
            Assert.NotNull(enprompt);
            Assert.NotNull(seprompt);
            Assert.NotEqual(enprompt.TranslatedText, seprompt.TranslatedText);
        }
    }
}