namespace EplStats
{
    public record Team(int Id, string Name, bool IsActive);
    public record Player(int Id, string Name, int TeamId, string Position);
}