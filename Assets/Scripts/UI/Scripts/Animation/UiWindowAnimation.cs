using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace serginian.UI.Animation
{
    public abstract class UiWindowAnimation : ScriptableObject
    {
        public abstract Awaitable ShowAsync(UiWindow window);
        public abstract Awaitable HideAsync(UiWindow window);
        
    }
}