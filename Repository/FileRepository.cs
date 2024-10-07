using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Repository
{
    public class FileRepository<T> : IFileRepository<T>
    {
        private readonly string _filePath;
        private readonly string _tableName;

        public FileRepository(string filePath, string tableName)
        {
            _filePath = filePath;
            _tableName = tableName;
        }

        private async Task<List<T>> LoadDataAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested(); // Проверяем, не был ли отменен запрос

            if (!File.Exists(_filePath))
            {
                return new List<T>();
            }

            var lines = await File.ReadAllLinesAsync(_filePath, cancellationToken); // Передаем токен отмены
            cancellationToken.ThrowIfCancellationRequested();

            var tableData = lines
                .SkipWhile(line => line != _tableName + ":")
                .Skip(1)
                .TakeWhile(line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            return tableData
                .Select(line => JsonSerializer.Deserialize<T>(line, options))
                .ToList();
        }

        private async Task SaveDataAsync(List<T> data, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var lines = await File.ReadAllLinesAsync(_filePath, cancellationToken); // Используем токен отмены
            cancellationToken.ThrowIfCancellationRequested();

            var newLines = lines
                .TakeWhile(line => line != _tableName + ":")
                .ToList();

            newLines.Add($"{_tableName}:");

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            newLines.AddRange(data.Select(item => JsonSerializer.Serialize(item, options)));
            newLines.Add("");

            await File.WriteAllLinesAsync(_filePath, newLines, cancellationToken); // Токен для записи данных
        }

        public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await LoadDataAsync(cancellationToken);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            var data = await LoadDataAsync(cancellationToken);
            data.Add(entity);
            await SaveDataAsync(data, cancellationToken);
        }

        public async Task DeleteAsync(Func<T, bool> predicate, CancellationToken cancellationToken)
        {
            var data = await LoadDataAsync(cancellationToken);
            data = data.Where(x => !predicate(x)).ToList();
            await SaveDataAsync(data, cancellationToken);
        }

        public async Task UpdateAsync(Predicate<T> predicate, T updatedEntity, CancellationToken cancellationToken)
        {
            var data = await LoadDataAsync(cancellationToken);
            var index = data.FindIndex(predicate);
            if (index != -1)
            {
                data[index] = updatedEntity;
                await SaveDataAsync(data, cancellationToken);
            }
        }
    }
}
