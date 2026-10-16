namespace BasketballMcp;

public class PlayerGame
{
    public string GameId { get; set; } = "";
    public int GameNumber { get; set; }
    public string PlayerId { get; set; } = "";
    public string Player { get; set; } = "";
    public string Team { get; set; } = "";
    public string Position { get; set; } = "";
    public bool Starter { get; set; }
    public bool TeamWon { get; set; }
    public int Points { get; set; }
    public int FieldGoalsMade { get; set; }
    public int FieldGoalsAttempted { get; set; }
    public int ThreePointersMade { get; set; }
    public int ThreePointersAttempted { get; set; }
    public int FreeThrowsMade { get; set; }
    public int FreeThrowsAttempted { get; set; }
    public int Rebounds { get; set; }
    public int OffensiveRebounds { get; set; }
    public int Assists { get; set; }
    public int Turnovers { get; set; }
    public int Steals { get; set; }
    public int Blocks { get; set; }
    public int PlusMinus { get; set; }
}

public record ShootingResult(string PlayerId, string Player, int Games, int Made, int Attempts, decimal Percentage);
