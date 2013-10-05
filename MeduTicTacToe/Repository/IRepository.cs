

namespace MeduTicTacToe.Repository
{
    public interface IRepository<T>
    {
        bool Any(string id);
        void Save(string id, T data);
        void Update(string id, T data);
        T Get(string id);
        T Delete(string id);
    }
}