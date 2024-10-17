using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Models;
using Npgsql;


namespace Repository
{
    public class DatabaseRepository<T>
    {
        private readonly string _connectionString;
        private readonly string _tableName;

        public DatabaseRepository(string connectionString, string tableName)
        {
            _connectionString = connectionString;
            _tableName = tableName;
        }

        // Метод для получения соединения с БД
        private async Task<NpgsqlConnection> GetConnectionAsync(CancellationToken token)
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(token);
            return connection;
        }

        // Метод для выполнения SQL-запросов типа SELECT
        public async Task<List<T>> ExecuteQueryAsync(string query, Dictionary<string, object> parameters, CancellationToken token)
        {
            var results = new List<T>();

            using (var connection = await GetConnectionAsync(token))
            {
                using (var command = new NpgsqlCommand(query, connection))
                {
                    // Добавляем параметры к запросу
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value);
                    }

                    // Выполняем запрос и считываем данные
                    using (var reader = await command.ExecuteReaderAsync(token))
                    {
                        while (await reader.ReadAsync(token))
                        {
                            // Преобразуем строки результата в объекты типа T
                            results.Add(TransformReaderToEntity(reader));
                        }
                    }
                }
            }

            return results;
        }

        // Метод для выполнения запросов INSERT, UPDATE и DELETE
        public async Task ExecuteNonQueryAsync(string query, Dictionary<string, object> parameters, CancellationToken token)
        {
            using (var connection = await GetConnectionAsync(token))
            {
                using (var command = new NpgsqlCommand(query, connection))
                {
                    // Добавляем параметры к запросу
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value);
                    }

                    // Выполняем запрос
                    await command.ExecuteNonQueryAsync(token);
                }
            }
        }

        // Метод для преобразования данных из строки результата в объект T
        private T TransformReaderToEntity(IDataRecord record)
        {
            if (typeof(T) == typeof(User))
            {
                return (T)(object)new User(
                    record.GetString(record.GetOrdinal("id")),
                    record.GetString(record.GetOrdinal("name")),
                    record.GetString(record.GetOrdinal("email")),
                    record.GetString(record.GetOrdinal("password_hash"))
                );
            }
                
            
            else if (typeof(T) == typeof(Present))
            {
                return (T)(object)new Present(
                    record.GetGuid(record.GetOrdinal("id")),
                    record.GetString(record.GetOrdinal("name")),
                    record.GetString(record.GetOrdinal("description")),
                    record.GetString(record.GetOrdinal("wishlistid")),
                    record.GetBoolean(record.GetOrdinal("isreserved")),
                    record.GetString(record.GetOrdinal("reserverid"))
                );
            }
            else if (typeof(T) == typeof(Wishlist))
            {
                return (T)(object)new Wishlist(
                    record.GetString(record.GetOrdinal("id")),
                    record.GetString(record.GetOrdinal("name")),
                    record.GetString(record.GetOrdinal("description")),
                    record.GetString(record.GetOrdinal("ownerid")),
                    record.GetString(record.GetOrdinal("presentsnumber"))
                );
            }

            throw new InvalidOperationException("Unsupported entity type");
        }
    }
}
