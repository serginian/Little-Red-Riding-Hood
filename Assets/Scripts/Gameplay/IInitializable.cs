using System.Threading.Tasks;

namespace serginian.Gameplay
{
    public interface IInitializable<in T>
    {
        Task Initialize(T arg);
    }
    
    public interface IInitialize
    {
        Task Initialize();
    }
}