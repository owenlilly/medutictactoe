using MeduTicTacToe.Domain;

namespace MeduTicTacToe.Models
{
    public class BigState
    {
        public UserState state { get; set; }
        public GameBoard game { get; set; }
    }
}