using System.Threading.Tasks;

namespace serginian.Gameplay
{
    public class Context
    {
        protected GameContext context;

        public void AssignContext(GameContext context)
        {
            this.context = context;
        }
    }
}