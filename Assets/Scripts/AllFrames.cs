using System;
using System.Threading.Tasks;
using serginian.Data;
using serginian.Gameplay;
using serginian.Gameplay.Storytelling;
using serginian.IO;
using serginian.UI;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace serginian
{
    public enum FrameType
    {
        Dialogue,
        Text,
        Choice,
        Final
    }

    public enum CharacterPosition
    {
        Left,
        Right
    }
    
    [Serializable]
    public abstract class Frame
    {
        public uint id;
        public FrameType type;
        
        public virtual Task OnEnter(GameContext context)
        {
            Debug.Log($"Entering frame #{id}");
            return Task.CompletedTask;
        }
        
        public static string Localize(string key)
        {
            try
            {
                return LocalizationSettings.StringDatabase.GetLocalizedString(key);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
            
        }
    }
    
    [Serializable]
    public class TextFrame : Frame
    {
        public string textKey;
        public uint nextFrameId;
        
        private GameContext _context;
        
        public override async Task OnEnter(GameContext context)
        {
            await UiMaster.GetWindow<DialogueWindow>().ShowTextAsync(Localize(textKey));
            await base.OnEnter(context);
            
            _context = context;
            InputManager.OnInteracted += OnInteracted;
        }
        
        public void OnInteracted()
        {
            InputManager.OnInteracted -= OnInteracted;
            _context.GetContext<StoryContext>().GoToFrame(nextFrameId);
        }
    }
    
    [Serializable]
    public sealed class DialogueFrame : TextFrame
    {
        public string characterAddress;
        public CharacterPosition characterPosition;
        
        public override async Task OnEnter(GameContext context)
        {
            Character character = await context.GetContext<CharacterContext>().GetCharacter(characterAddress);
            _ = UiMaster.GetWindow<DialogueWindow>().ShowNameAsync(Localize(character.nameKey), character.nameColor);
            _ = UiMaster.GetWindow<CharacterWindow>().ShowCharacterAsync(character, characterPosition);
            await base.OnEnter(context);
        }
    }
    
    [Serializable]
    public class ChoiceFrame : Frame
    {
        public string textKey;
        public Option[] options;

        private GameContext _context;
        
        public override async Task OnEnter(GameContext context)
        {
            _context = context;
            var choiceWindow = UiMaster.GetWindow<ChoiceWindow>();
            choiceWindow.OnOptionSelected += OnOptionSelected;
            await choiceWindow.ShowChoiceAsync(options);
            await UiMaster.GetWindow<DialogueWindow>().ShowTextAsync(Localize(textKey));
            await base.OnEnter(context);
        }

        private async void OnOptionSelected(Option option)
        {
            var choiceWindow = UiMaster.GetWindow<ChoiceWindow>();
            choiceWindow.OnOptionSelected -= OnOptionSelected;
            await choiceWindow.CloseAsync();
            await _context.GetContext<StoryContext>().GoToFrame(option.targetFrameId);
        }
    }
    
    [Serializable]
    public class FinalFrame : Frame
    {
        public  string messageKey;
        
        public override async Task OnEnter(GameContext context)
        {
            await UiMaster.GetWindow<DialogueWindow>().CloseAsync();
            await UiMaster.GetWindow<MessageWindow>().ShowMessageAsync(Localize(messageKey));
            await base.OnEnter(context);
        }
    }
    
    [Serializable]
    public class Option
    {
        public string textKey;
        public uint targetFrameId;

        public string Text => Frame.Localize(textKey);
    }
}