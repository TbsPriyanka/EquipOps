using Dapper;
using Npgsql;
using System.Data;
using System.Dynamic;

namespace EquipOps.Common.Helper
{
    public class PgHelper(IDbConnectionFactory dbFactory)
    {
        public async Task<dynamic> CreateUpdateAsync(string procedureName, Dictionary<string, DbParam> Params)
        {
            using var conn = dbFactory.CreateConnection();

            var parameters = new DynamicParameters();

            foreach (var p in Params)
            {
                parameters.Add(
                    p.Key,
                    value: p.Value.Value ?? DBNull.Value,
                    dbType: p.Value.DbType,
                    direction: p.Value.Direction
                );
            }

            string query = BuildCallQuery(procedureName, Params);
            await conn.ExecuteAsync(query, parameters);

            var dynamicvalue = MapOutputAnonymous(parameters, Params);
            return dynamicvalue;
        }

        public async Task<dynamic> ListAsync(string procedureName, Dictionary<string, DbParam> Params)
        {
            await using var conn = dbFactory.CreateConnection() as NpgsqlConnection;
            if (conn == null)
                throw new InvalidOperationException("Failed To Create a Database Connection.");

            await conn.OpenAsync();
            await using var tran = await conn.BeginTransactionAsync();

            var paramNames = string.Join(", ", Params.Keys.Select(x => "@" + x));
            var sql = $"CALL {procedureName}({paramNames})";

            await using var cmd = new NpgsqlCommand(sql, conn, tran);

            foreach (var p in Params)
            {
                var param = new NpgsqlParameter
                {
                    ParameterName = p.Key,
                    Direction = p.Value.Direction,
                    Value = p.Value.Value ?? DBNull.Value,
                    NpgsqlDbType = ConvertDbTypeToNpgsql(p.Value.DbType, p.Key, p.Value.Direction)
                };
                cmd.Parameters.Add(param);
            }

            await cmd.ExecuteNonQueryAsync();

            dynamic result = new ExpandoObject();
            var resultDict = (IDictionary<string, object?>)result;

            foreach (var key in Params
                         .Where(p => p.Value.Direction == ParameterDirection.Output
                                  || p.Value.Direction == ParameterDirection.InputOutput)
                         .Select(p => p.Key))
            {
                var value = cmd.Parameters.Contains(key) ? cmd.Parameters[key].Value : null;
                resultDict[key] = value is DBNull ? null : value;
            }

            foreach (var key in Params
                         .Where(p => (p.Key.ToLower().Contains("ref") || p.Key.ToLower().Contains("cursor"))
                                  && (p.Value.Direction == ParameterDirection.Output || p.Value.Direction == ParameterDirection.InputOutput))
                         .Select(p => p.Key))
            {
                var curName = cmd.Parameters[key].Value?.ToString() ?? "";
                var rows = new List<dynamic>();

                await using (var fetchCmd = new NpgsqlCommand($"FETCH ALL FROM \"{curName}\";", conn, tran))
                await using (var reader = await fetchCmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dynamic row = new ExpandoObject();
                        var rowDict = (IDictionary<string, object?>)row;

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            rowDict[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        }

                        rows.Add(row);
                    }
                }

                resultDict[key] = rows;
            }

            await tran.CommitAsync();
            return result;
        }

        private static NpgsqlTypes.NpgsqlDbType ConvertDbTypeToNpgsql(DbType type, string paramName, ParameterDirection direction)
        {
            if ((paramName.ToLower().Contains("ref") || paramName.ToLower().Contains("cursor"))
                && (direction == ParameterDirection.Output || direction == ParameterDirection.InputOutput))
            {
                return NpgsqlTypes.NpgsqlDbType.Refcursor;
            }

            return type switch
            {
                DbType.Guid => NpgsqlTypes.NpgsqlDbType.Uuid,
                DbType.String => NpgsqlTypes.NpgsqlDbType.Text,
                DbType.Boolean => NpgsqlTypes.NpgsqlDbType.Boolean,
                DbType.Int32 => NpgsqlTypes.NpgsqlDbType.Integer,
                DbType.DateTime => NpgsqlTypes.NpgsqlDbType.TimestampTz,
                _ => NpgsqlTypes.NpgsqlDbType.Text 
            };
        }

        private string BuildCallQuery(string procedureName, Dictionary<string, DbParam> inputParams)
        {
            var paramList = inputParams.Keys.Select(k => "@" + k);

            var joinedParams = string.Join(", ", paramList);

            return $"CALL {procedureName}({joinedParams})";
        }

        public static dynamic MapOutputAnonymous(DynamicParameters? parameters, Dictionary<string, DbParam>? paramDict)
        {
            IDictionary<string, object?> expando = new ExpandoObject();

            if (parameters == null || paramDict == null)
                return expando;

            foreach (var kvp in paramDict)
            {
                var paramKey = kvp.Key;
                var paramValue = kvp.Value;

                if (paramValue == null)
                    continue;

                if (paramValue.Direction != ParameterDirection.Output &&
                    paramValue.Direction != ParameterDirection.InputOutput)
                    continue;

                object? value = null;
                if (!string.IsNullOrWhiteSpace(paramKey) && parameters.ParameterNames.Contains(paramKey))
                {
                    value = parameters.Get<object>(paramKey);
                }

                expando[paramKey] = value == DBNull.Value ? null : value;
            }

            return expando;
        }

        public static dynamic ConvertDictionaryToDynamic(IDictionary<string, object?> dict)
        {
            dynamic expando = new ExpandoObject();
            var expandoDict = (IDictionary<string, object?>)expando;

            foreach (var kv in dict)
            {
                expandoDict[kv.Key] = kv.Value;
            }
            return expando;
        }
    }
    public class DbParam
    {
        public object? Value { get; set; }
        public DbType DbType { get; set; }
        public ParameterDirection Direction { get; set; } = ParameterDirection.Input;
    }
}
