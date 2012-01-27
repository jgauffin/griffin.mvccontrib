using System.Data;

namespace Griffin.MvcContrib.SqlServer.Localization
{
	public interface ILocalizationDbContext
	{
		IDbConnection Connection { get; }
		char ParameterPrefix { get; }
		string ChangePrefix(string sql);
	}
}