using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
namespace Repository;
public class FileRepository<T>
{
    private readonly string _filePath;
    private readonly string _tableName;

    public FileRepository(string filePath, string tableName)
    {
        _filePath = filePath;
        _tableName = tableName;
    }
    

    private async Task<List<T>> LoadDataAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<T>();
        }

        var lines = await File.ReadAllLinesAsync(_filePath);
        
        var tableData = lines.SkipWhile(line => line != _tableName + ":").Skip(1).TakeWhile(line => !string.IsNullOrWhiteSpace(line)).ToList();

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull // Игнорировать null поля при записи
        };

        return tableData.Select(line => JsonSerializer.Deserialize<T>(line, options)).ToList();
    }

    private async Task SaveDataAsync(List<T> data)
    {
        var lines = await File.ReadAllLinesAsync(_filePath);
        var newLines = lines.TakeWhile(line => line != _tableName + ":").ToList();
        newLines.Add($"{_tableName}:");

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull // Игнорировать null поля при записи
        };

        newLines.AddRange(data.Select(item => JsonSerializer.Serialize(item, options)));
        newLines.Add("");

        await File.WriteAllLinesAsync(_filePath, newLines);
    }


    public async Task<List<T>> GetAllAsync()
    {
        return await LoadDataAsync();
    }

    public async Task AddAsync(T entity)
    {
        var data = await LoadDataAsync();
        data.Add(entity);
        await SaveDataAsync(data);
    }

    public async Task DeleteAsync(Func<T, bool> predicate)
    {
        var data = await LoadDataAsync();
        data = data.Where(x => !predicate(x)).ToList();
        await SaveDataAsync(data);
    }

    public async Task UpdateAsync(Predicate<T> predicate, T updatedEntity)
    {
        var data = await LoadDataAsync();
        var index = data.FindIndex(predicate);
        if (index != -1)
        {
            data[index] = updatedEntity;
            await SaveDataAsync(data);
        }
    }
    
}
