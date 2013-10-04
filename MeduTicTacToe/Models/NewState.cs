using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeduTicTacToe.Models
{
    public class NewState
    {
        public string playerName { get; set; }
        public string gameName { get; set; }
        public bool isTurn { get; set; }
        public Cell[][] rows { get; set; }
    }
}