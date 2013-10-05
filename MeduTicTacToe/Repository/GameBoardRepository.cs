using MeduTicTacToe.Domain;


namespace MeduTicTacToe.Repository
{
    public class GameBoardRepository : IGameBoardRepository<GameBoard>
    {
        protected readonly IRepository<GameBoard> RepositoryBase;

        public GameBoardRepository() 
            : this(new RepositoryBase<GameBoard>())
        { }

        public GameBoardRepository(IRepository<GameBoard> repository)
        {
            RepositoryBase = repository;
        }

        public GameBoard UpdateGameState(GameBoard game)
        {
            if (!GameNameExists(game.GameName))
                return null;

            DeleteGame(game.GameName);
            RepositoryBase.Save(game.GameName, game);

            return game;
        }

        public GameBoard GetGame(string gameName)
        {
            return RepositoryBase.Get(gameName);
        }

        public GameBoard DeleteGame(string gameName)
        {
            return RepositoryBase.Delete(gameName);
        }

        public Player GetPlayer(string gameName, string playerName)
        {
            var board = GetGame(gameName);

            if (board == null)
                return null;

            if (board.Player1.PlayerName == playerName)
                return board.Player1;
            else if (board.Player2.PlayerName == playerName)
                return board.Player2;

            return null;
        }

        public bool GameFull(string gameName)
        {
            var board = GetGame(gameName);

            return ((board.Player1 != null) && (board.Player2 != null));
        }

        public bool AddPlayerToGame(string gameName, Player player, out GameBoard board)
        {
            if(!GameNameExists(gameName))
            {
                var newBoard = GameBoard.CreateBoard(gameName);

                newBoard.Player1 = player;

                RepositoryBase.Save(gameName, newBoard);

                board = newBoard;

                return true;
            }
            else if(!GameFull(gameName))
            {
                var oldBoard = GetGame(gameName);
                oldBoard.Player2 = player;
                RepositoryBase.Update(gameName, oldBoard);

                board = oldBoard;

                return true;
            }

            board = null;

            return false;
        }

        public bool GameNameExists(string gameName)
        {
            return RepositoryBase.Any(gameName);
        }
    }
}