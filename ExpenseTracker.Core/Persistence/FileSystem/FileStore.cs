using System.Text.Json;

namespace ExpenseTracker.Core.Persistence.FileSystem;

internal static class FileStore
{
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public static async Task<T> LoadOrInitAsync<T>(string path, Func<T> init)
    {
        if (!File.Exists(path))
        {
            var seed = init();
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var jsonSeed = JsonSerializer.Serialize(seed, JsonOpts);
            await File.WriteAllTextAsync(path, jsonSeed);
            return seed;
        }

        var json = await File.ReadAllTextAsync(path);
        var data = JsonSerializer.Deserialize<T>(json, JsonOpts);
        return data!;
    }

    public static Task SaveAsync<T>(string path, T data)
    {
        var json = JsonSerializer.Serialize(data, JsonOpts);
        return File.WriteAllTextAsync(path, json);
    }

}