using Parquet.Serialization;

namespace BasketballMcp;

public static class SampleData
{
    public static async Task WriteAsync(string directory)
    {
        Directory.CreateDirectory(directory);
        for (var game = 1; game <= 3; game++)
        {
            var path = Path.Combine(directory, $"synthetic-game-{game}.parquet");
            if (File.Exists(path)) throw new IOException("Sample file already exists; use a new empty directory.");
            await ParquetSerializer.SerializeAsync(Rows(game), path);
        }
    }

    public static PlayerGame[] Rows(int game) =>
    [
        Make(game, "p1", "Morgan Lane", "Comets", true, game == 1 ? 1 : 9, game == 1 ? 2 : 10),
        Make(game, "p2", "Jordan Reed", "Comets", false, 4, 5),
        Make(game, "p3", "Alex Rivers", "Owls", true, 3, 4),
        Make(game, "p4", "Casey Brooks", "Owls", false, 0, 0)
    ];

    private static PlayerGame Make(int game, string id, string name, string team, bool starter, int made, int attempts) => new()
    {
        GameId = $"synthetic-g{game}", GameNumber = game, PlayerId = id, Player = name, Team = team,
        Position = starter ? "G" : "F", Starter = starter, TeamWon = (game % 2 == 1) == (team == "Comets"),
        FieldGoalsMade = 5, FieldGoalsAttempted = 12, ThreePointersMade = 2, ThreePointersAttempted = 5,
        Points = 12 + made, FreeThrowsMade = made, FreeThrowsAttempted = attempts,
        Rebounds = 6, OffensiveRebounds = 1, Assists = starter ? 7 : 2, Turnovers = 2,
        Steals = 1, Blocks = 0, PlusMinus = team == "Comets" ? 4 : -4
    };
}
