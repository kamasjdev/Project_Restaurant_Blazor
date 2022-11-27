using System.Data.Common;
using System.Data;

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

        public static string? GetSafeString(this DbDataReader dbDataReader, string field)
        {
            if (dbDataReader.IsDBNull(field))
            {
                return null;
            }

            return dbDataReader.GetString(field);
        }

        public static decimal GetSafeDecimal(this DbDataReader dbDataReader, string field)
        {
            if (dbDataReader.IsDBNull(field))
            {
                return default;
            }

            return dbDataReader.GetDecimal(field);
        }

        public static Guid GetSafeGuid(this DbDataReader dbDataReader, string field)
        {
            if (dbDataReader.IsDBNull(field))
            {
                return default;
            }

            return dbDataReader.GetGuid(field);
        }

        public static DateTime GetSafeDateTime(this DbDataReader dbDataReader, string field)
        {
            if (dbDataReader.IsDBNull(field))
            {
                return default;
            }

            return dbDataReader.GetDateTime(field);
        }
    }
}
