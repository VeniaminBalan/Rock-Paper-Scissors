using Microsoft.AspNetCore.SignalR;
using SignalR.GameLogic;

namespace SignalR.Hubs;

public class GameHub : Hub<IGameHub>
{
    public static GameManager _manager = new();

    public static Dictionary<string, string> _players = new();

    public async Task Register(string name)
    {
        var group = _manager.Register(name);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, group.Name);
        _players.TryAdd(Context.ConnectionId, name);

        if (group.Full)
            await Clients.Group(group.Name).GameStarted(group.Game.Player1.Name,
                                                                     group.Game.Player2.Name,
                                                                     group.Name);
        else
            await Clients.Caller.WaitingForPlayer();
    }

    public async Task Throw(string groupName, string player, string selection)
    {
        var game = _manager.Throw(groupName, player, Enum.Parse<Sign>(selection, true));

        if (game.Pending)
            await Clients.Group(groupName).Pending(game.WaitingFor);
        else
        {
            var winner = game.Winner;
            var explanation = game.Explanation;

            game.Reset();

            if (winner == null)
                await Clients.Group(groupName).Drawn(explanation, game.Scores);
            else
                await Clients.Group(groupName).Won(winner, explanation, game.Scores);
        }
    }

    public Dictionary<string, string> GetAllPLayers()
    {
        return _players;
    }


    public override Task OnDisconnectedAsync(Exception exception)
    {
        _players.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
