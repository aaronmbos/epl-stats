namespace EplStats
{
    public record Team(int Id, string Name, bool IsActive);
    public record PlayerStat(
        string Name, 
        string Team,
        string Position,
        double Form,
        int GameweekPoints,
        int TotalPoints,
        string Price,
        string SelectedPercent,
        string Influence,
        string Creativity,
        string Threat,
        string PositionalIct,
        string OverallIct
    );
}