namespace EplStats
{
    public record Team(int Id, string Name, bool IsActive);
    public record Player(
        string Name, 
        string Team,
        string Position,
        double Form,
        int GameweekPoints,
        int TotalPoints,
        double Price,
        string Influence,
        string Creativity,
        string Threat,
        string OverallIct
    );
}