using serginian.Gameplay.Storytelling;
using serginian.UI;
using UnityEngine;

namespace serginian.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private string storyJsonAddress;
        private GameContext _context;

        
        /****************** UNITY BEHAVIOUR ******************/
        private void Awake()
        {
            _context = GameContext.Create()
                .AddContext(new StoryContext())
                .AddContext(new CharacterContext());
        }

        private async void Start()
        {
            var storyContext = _context.GetContext<StoryContext>();

            await storyContext.Initialize(storyJsonAddress);
            await UiMaster.GetWindow<BackgroundWindow>().ShowAsync();
            await storyContext.StartStory();
        }

        private void OnDestroy()
        {
            _context?.Dispose();
        }
        
    } // end of class
}