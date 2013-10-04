using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeduTicTacToe.Models
{
    public class GetStateModel
    {
        public bool  isTurn { get; set; }
        public string gameName { get; set; }
        public Cell[][] rows { get; set; }
    }
}