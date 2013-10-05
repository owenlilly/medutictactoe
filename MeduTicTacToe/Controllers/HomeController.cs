using System.Web.Mvc;

using MeduTicTacToe.Domain;
using MeduTicTacToe.Models;
using MeduTicTacToe.Repository;


namespace MeduTicTacToe.Controllers
{
    public class HomeController : Controller
    {
        private readonly GameBoardRepository GameBoardRepo;

        public HomeController()
        {
            GameBoardRepo = new GameBoardRepository();
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCurrentState()
        {
            var gameName = Request.QueryString.Get("gameName");
            var playerName = Request.QueryString.Get("playerName");

            if (string.IsNullOrWhiteSpace(gameName))
                return null;

            var game = GameBoardRepo.GetGame(gameName);

            if (game == null)
                return null;

            var isTurn = false;

            if(game.Player1.PlayerName == playerName)
            {
                isTurn = game.Player1.IsPlayerTurn;
            }
            else if (game.Player2.PlayerName == playerName)
            {
                isTurn = game.Player2.IsPlayerTurn;
            }

            var player = GameBoardRepo.GetPlayer(gameName, playerName);

            var state = new UserState
                            {
                                winnerName = game.WinnerName,
                                gameOver   = game.GameOver,
                                winCount   = player.WinCount,
                                playerName = player.PlayerName,
                                isTurn     = isTurn,
                                gameName   = game.GameName,
                                rows       = game.Rows.rows
                            };

            return new JsonResult
                       {
                           JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                           ContentType = "application/json",
                           Data = state
                       };
        }

        public JsonResult UpdateGameState(UserState state)
        {
            if (GameBoardRepo.GameNameExists(state.gameName))
            {
                var game   = GameBoardRepo.GetGame(state.gameName);
                var player = GameBoardRepo.GetPlayer(state.gameName, state.playerName);

                if (game.Player2 == null)
                {
                    Response.StatusCode = 400;
                    Response.Write("Waiting for second player...");

                    return null;
                }

                game.Rows = new RowCollection
                                {
                                    rows = state.rows
                                };

                if (CheckWin(game, player))
                {
                    state.winCount   = (player.WinCount += 1);
                    state.winnerName = player.WinnerName = player.PlayerName;
                    player.IsWinner  = true;

                    state.gameOver  = game.GameOver = true;
                    game.WinnerName = player.PlayerName;
                }

                if (game.GameOver)
                {
                    if (state.winnerName == game.Player1.PlayerName)
                    {
                        game.Player1.IsWinner = true;
                        game.GameOver         = true;
                    }
                    else if (state.winnerName == game.Player2.PlayerName)
                    {
                        game.Player2.IsWinner = true;
                        game.GameOver         = true;
                    }
                }
                else
                {
                    game.Player1.IsPlayerTurn = !game.Player1.IsPlayerTurn;
                    game.Player2.IsPlayerTurn = !game.Player2.IsPlayerTurn;

                    state.isTurn = player.IsPlayerTurn;
                }

                GameBoardRepo.UpdateGameState(game);

                return new JsonResult
                       {
                           ContentType = "application/json",
                           Data = state
                       };
            }

            Response.StatusCode = 404;
            Response.StatusDescription = "Game not found";

            return new JsonResult
                       {
                           ContentType = "application/json",
                           Data = state
                       };
        }

        public JsonResult InitGame(RequestData data)
        {
            if (!GameBoardRepo.GameNameExists(data.gameName))
            {
                return NewGame(data.gameName, data.playerName, data.rows);
            }
            
            return AddSecondPlayer(data.gameName, data.playerName);
        }

        protected JsonResult NewGame(string gameName, string playerName, Cell[][] rows)
        {
            GameBoard board = null;

            GameBoardRepo.AddPlayerToGame(gameName, Player.CreatePlayer(playerName, "X", true), out board);
            
            return new JsonResult
                       {
                           ContentType = "application/json",
                           Data = new
                                      {
                                          isNewGame = true,
                                          session = board
                                      }
                       };
        }

        protected JsonResult AddSecondPlayer(string gameName, string playerName)
        {
            GameBoard board = null;

            var playerAdded = GameBoardRepo.AddPlayerToGame(gameName, Player.CreatePlayer(playerName, "O", false), out board);

            if(!playerAdded)
            {
                Response.StatusCode = 400;
                Response.Write("A player with this name is already on the board...");

                return null;
            }

            return new JsonResult
                       {
                           ContentType = "application/json",
                           Data = new
                                      {
                                          isNewGame = false,
                                          session = board
                                      }
                       };;
        }

        private static bool CheckWin(GameBoard game, Player player)
        {
            var letter = player.PlayerLetter;
            var rows = game.Rows;

            var textToLookFor = letter + letter + letter;
            var j = rows.rows.Length;
            for (var i = 0; i < j; i++)
            {
                if (rows[i, 0].letter + rows[i, 1].letter + rows[i, 2].letter == textToLookFor)
                {
                    return true;
                }

                if (rows[0, i].letter + rows[1, i].letter + rows[2, i].letter == textToLookFor)
                {
                    return true;
                }
            }
            if (rows[0, 0].letter + rows[1, 1].letter + rows[2, 2].letter == textToLookFor)
            {
                return true;
            }
            if (rows[2, 0].letter + rows[1, 1].letter + rows[0, 2].letter == textToLookFor)
            {
                return true;
            }

            return false;
        }

        #region Hidden
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        #endregion
    }
}