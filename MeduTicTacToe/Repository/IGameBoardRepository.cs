using MeduTicTacToe.Domain;


namespace MeduTicTacToe.Repository
{
    public interface IGameBoardRepository<T>
    {
        T GetGame(string gameName);
        T DeleteGame(string gameName);
        Player GetPlayer(string gameName, string playerName);
        GameBoard UpdateGameState(GameBoard game);
        bool GameFull(string gameName);
        bool AddPlayerToGame(string gameName, Player player, out GameBoard success);
        bool GameNameExists(string gameName);
    }
}