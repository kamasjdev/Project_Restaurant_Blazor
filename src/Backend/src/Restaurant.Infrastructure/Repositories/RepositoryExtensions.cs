using System.Data.Common;

namespace Restaurant.Infrastructure.Repositories
{
    internal static class RepositoryExtensions
    {
        public static DbParameter CreateParameter(this DbCommand dbCommand, string name, object value)
        {
            var dbParameter = dbCommand.CreateParameter();
            dbParameter.ParameterName = name;
            dbParameter.Value = value;
            return dbParameter;
        }
        
        public static void AddParameter(this DbCommand dbCommand, string name, object value)
        {
            var dbParameter = dbCommand.CreateParameter(name, value);
            dbCommand.Parameters.Add(dbParameter);
        }
    }
}
