namespace SignalR.Hubs;

public interface IGameHub
{
    Task GameStarted(string player1, string player2, string gameName);
    Task WaitingForPlayer();
    Task Pending(string? WaitingFor);
    Task Drawn(string? explanation, string gameScores);
    Task Won(string? winner, string? explanation, string gameScores);
    Task<IEnumerable<string>> GetPlayers();
}
