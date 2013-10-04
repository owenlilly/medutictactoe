using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeduTicTacToe.Models
{
    public class SessionModel
    {
        public string GameName { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public GameBoard GameBoard { get; set; }
    }

    public class Player
    {
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string PlayerLetter { get; set; }
        public bool IsPlayerTurn { get; set; }
    }

    public class GameBoard
    {
        public Cell[][] rows { get; set; }
    }

    public class Cell
    {
        public string id { get; set; }
        public string letter { get; set; }
    }
}