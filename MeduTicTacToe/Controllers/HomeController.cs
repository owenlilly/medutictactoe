using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using System.Web.Mvc;

using MeduTicTacToe.Models;


namespace MeduTicTacToe.Controllers
{
    public class HomeController : Controller
    {
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

            var session = GetFromCache(gameName);

            if (session == null)
                return null;

            var isTurn = false;

            if(session.Player1.PlayerName == playerName)
            {
                isTurn = session.Player1.IsPlayerTurn;
            }
            else if (session.Player2.PlayerName == playerName)
            {
                isTurn = session.Player2.IsPlayerTurn;
            }

            var state = new GetStateModel
                            {
                                isTurn = isTurn,
                                gameName = session.GameName,
                                rows = session.GameBoard.rows
                            };

            return new JsonResult
                       {
                           JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                           ContentType = "application/json",
                           Data = state
                       };
        }

        public JsonResult UpdateGameState(NewState state)
        {
            var gameName = state.gameName;
            var session  = GetFromCache(state.gameName);

            if (SessionIsValid(gameName, session))
            {
                if(session.Player2 == null)
                {
                    Response.StatusCode = 400;
                    Response.Write("Waiting for second player...");

                    return null;
                }

                session.Player1.IsPlayerTurn = !session.Player1.IsPlayerTurn;
                session.Player2.IsPlayerTurn = !session.Player2.IsPlayerTurn;

                session.GameBoard = new GameBoard
                                        {
                                            rows = state.rows
                                        };

                UpdateSession(state.gameName, session);
            }

            return new JsonResult
                       {
                           ContentType = "application/json",
                           Data = state
                       };
        }

        public JsonResult InitGame(RequestData data)
        {
            var session = GetFromCache(data.gameName);

            if(session == null)
            {
                return NewGame(data.gameName, data.playerName, data.rows);
            }
            
            return AddSecondPlayer(data.gameName, data.playerName);
        }

        protected JsonResult NewGame(string gameName, string playerName, Cell[][] rows)
        {
            var session = new SessionModel
                              {
                                  GameName = gameName,
                                  Player1 = new Player
                                                  {
                                                      PlayerId = Guid.NewGuid().ToString(),
                                                      PlayerName = playerName,
                                                      PlayerLetter = "X",
                                                      IsPlayerTurn = true
                                                  },
                                  GameBoard = new GameBoard
                                                  {
                                                      rows = new Cell[][]
                                                                 {
                                                                     new Cell[]{new Cell(), new Cell(), new Cell(), },
                                                                     new Cell[]{new Cell(), new Cell(), new Cell(), },
                                                                     new Cell[]{new Cell(), new Cell(), new Cell(), }
                                                                 }
                                                  }
                              };

            AddToCache(session.GameName, session);
            
            return new JsonResult
                       {
                           ContentType = "application/json",
                           Data = new
                                      {
                                          isNewGame = true,
                                          session = session
                                      }
                       };
        }

        protected JsonResult AddSecondPlayer(string gameName, string playerName)
        {
            var session = GetFromCache(gameName);
            
            if (SessionIsValid(gameName, session))
            {
                if(session.Player1.PlayerName == playerName)
                {
                    Response.StatusCode = 400;
                    Response.Write("A player with this name is already on the board...");

                    return null;
                }

                session.Player2 = new Player
                                        {
                                            PlayerId     = Guid.NewGuid().ToString(),
                                            PlayerLetter = "O",
                                            PlayerName   = playerName,
                                            IsPlayerTurn = false
                                        };

                UpdateSession(gameName, session);
            }

            return new JsonResult
                       {
                           ContentType = "application/json",
                           Data = new
                                      {
                                          isNewGame = false,
                                          session = session
                                      }
                       };;
        }

        private SessionModel GetFromCache(string key)
        {
            var cache = this.Request.RequestContext.HttpContext.Cache;

            if (string.IsNullOrWhiteSpace(key))
                return null;

            return (SessionModel)cache.Get(key);
        }

        private bool SessionIsValid(string gameName, object session)
        {
            if (session == null)
            {
                Response.StatusCode = 404;
                Response.Write("Game name --" + gameName + "-- was not found");

                return false;
            }

            return true;
        }

        private void UpdateSession(string key, SessionModel sessionData)
        {
            RemoveFromCache(key);
            AddToCache(key, sessionData);
        }

        private void RemoveFromCache(string key)
        {
            var cache = this.Request.RequestContext.HttpContext.Cache;

            cache.Remove(key);
        }

        private void AddToCache(string key, object value)
        {
            var cache = this.Request.RequestContext.HttpContext.Cache;

            cache.Add(key, value, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, 30, 0), CacheItemPriority.Normal, null);
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