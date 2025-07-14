using System;
using System.Threading.Tasks;
using serginian.Data;
using serginian.UI;

namespace serginian.Gameplay.Storytelling
{
    public class StoryContext : Context, IInitializable<string>
    {
        private readonly Story _storyManager = new();
        private Frame _currentFrame;

        public async Task Initialize(string addressableKey)
        {
            await _storyManager.LoadFromJsonAsync(addressableKey);
        }

        public Task StartStory()
        {
            return GoToFrame(_storyManager.GetStartFrame());
        }

        public Task GoToFrame(uint frameId)
        {
            return GoToFrame(_storyManager.GetFrameById(frameId));
        }

        private async Task GoToFrame(Frame frame)
        {
            if (frame == null)
                throw new NullReferenceException();

            // don't like this hard-code line. The architecture should be modified to handle this in the correct way
            if (frame is not DialogueFrame && _currentFrame is DialogueFrame)
                await Clear(); // if the previous frame was Dialogue but current not, then hide characters and texts
            
            _currentFrame = frame;
            await frame.OnEnter(context);
        }

        private async Task Clear()
        {
            UiMaster.GetWindow<CharacterWindow>().HideCharacters();
            _ = UiMaster.GetWindow<DialogueWindow>().HideTextAsync();
            await UiMaster.GetWindow<DialogueWindow>().HideNameAsync();
        }
        
    }
}