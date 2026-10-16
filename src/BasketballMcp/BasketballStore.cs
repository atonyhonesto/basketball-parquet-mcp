using Parquet;
using Parquet.Schema;
using Parquet.Serialization;

namespace BasketballMcp;

public sealed class BasketballStore(string directory)
{
    public string[] ListDatasets()
    {
        if (!Directory.Exists(directory)) return [];
        if ((File.GetAttributes(directory) & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Linked data directories are not supported.");
        var files = Directory.GetFiles(directory, "*.parquet", SearchOption.TopDirectoryOnly).Order(StringComparer.Ordinal).ToArray();
        if (files.Length > 100) throw new InvalidDataException("This local demo supports at most 100 files.");
        foreach (var file in files)
        {
            if ((File.GetAttributes(file) & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Linked files are not supported.");
            if (new FileInfo(file).Length > 10 * 1024 * 1024) throw new InvalidDataException("File exceeds the 10 MiB demo limit.");
        }
        return files.Select(Path.GetFileName).Select(x => x!).ToArray();
    }

    public async Task<List<PlayerGame>> ReadAsync()
    {
        var rows = new List<PlayerGame>();
        var keys = new HashSet<(string, string)>();
        foreach (var file in ListDatasets())
        {
            var path = Path.Combine(directory, file);
            await using (var stream = File.OpenRead(path))
            using (var reader = await ParquetReader.CreateAsync(stream))
            {
                var required = typeof(PlayerGame).GetProperties().Select(x => x.Name).ToHashSet(StringComparer.Ordinal);
                if (reader.Schema.Fields.Any(f => f is not DataField) || !required.SetEquals(reader.Schema.GetDataFields().Select(x => x.Name)))
                    throw new InvalidDataException("Expected the flat PlayerGame schema. Normalize nested box scores explicitly before use.");
                long count = 0;
                for (var i = 0; i < reader.RowGroupCount; i++)
                {
                    using var group = reader.OpenRowGroupReader(i);
                    count += group.RowCount;
                }
                if (count + rows.Count > 20000) throw new InvalidDataException("Dataset exceeds the 20,000-row demo limit.");
            }
            foreach (var row in await ParquetSerializer.DeserializeAsync<PlayerGame>(path))
            {
                Validate(row);
                if (!keys.Add((row.GameId, row.PlayerId))) throw new InvalidDataException("Duplicate game/player record. No totals returned.");
                rows.Add(row);
            }
        }
        return rows;
    }

    public static void Validate(PlayerGame row)
    {
        if (string.IsNullOrWhiteSpace(row.GameId) || string.IsNullOrWhiteSpace(row.PlayerId) || string.IsNullOrWhiteSpace(row.Player) || string.IsNullOrWhiteSpace(row.Team) || row.GameNumber < 1)
            throw new InvalidDataException("Missing player/game identity.");
        var stats = new[] { row.Points, row.FieldGoalsMade, row.FieldGoalsAttempted, row.ThreePointersMade, row.ThreePointersAttempted, row.FreeThrowsMade, row.FreeThrowsAttempted, row.Rebounds, row.OffensiveRebounds, row.Assists, row.Turnovers, row.Steals, row.Blocks };
        if (stats.Any(x => x < 0) || row.FreeThrowsMade > row.FreeThrowsAttempted || row.FieldGoalsMade > row.FieldGoalsAttempted || row.ThreePointersMade > row.ThreePointersAttempted || row.ThreePointersMade > row.FieldGoalsMade || row.ThreePointersAttempted > row.FieldGoalsAttempted || row.OffensiveRebounds > row.Rebounds)
            throw new InvalidDataException("Invalid basketball counts.");
    }

    public async Task<List<ShootingResult>> FreeThrowsAsync(int minimumAttempts)
    {
        if (minimumAttempts < 1) throw new ArgumentOutOfRangeException(nameof(minimumAttempts));
        return (await ReadAsync()).GroupBy(x => x.PlayerId, StringComparer.Ordinal)
            .Select(g => new { Id = g.Key, Name = g.First().Player, Games = g.Count(), Made = g.Sum(x => (long)x.FreeThrowsMade), Attempts = g.Sum(x => (long)x.FreeThrowsAttempted) })
            .Where(x => x.Attempts >= minimumAttempts)
            .Select(x => new ShootingResult(x.Id, x.Name, x.Games, checked((int)x.Made), checked((int)x.Attempts), Math.Round(100m * x.Made / x.Attempts, 2)))
            .OrderByDescending(x => x.Percentage).ThenByDescending(x => x.Attempts).ThenBy(x => x.PlayerId, StringComparer.Ordinal).ToList();
    }
}
