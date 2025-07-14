using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using serginian.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace serginian.Gameplay.Storytelling
{
    public class Story
    {
        private Dictionary<uint, Frame> _frames;

        public IReadOnlyDictionary<uint, Frame> Frames => _frames;

        public async Task LoadFromJsonAsync(string addressableKey)
        {
            var textAssetHandle = Addressables.LoadAssetAsync<TextAsset>(addressableKey);
            TextAsset jsonAsset = await textAssetHandle.Task;

            if (jsonAsset == null)
            {
                throw new Exception($"Failed to load TextAsset with key: {addressableKey}");
            }

            LoadFromJson(jsonAsset);
            textAssetHandle.Release();
        }

        public void LoadFromJson(TextAsset jsonAsset)
        {
            _frames = new Dictionary<uint, Frame>();

            var rawArray = JArray.Parse(jsonAsset.text);

            foreach (var token in rawArray)
            {
                var typeToken = token["type"];
                if (typeToken == null)
                    throw new Exception($"Story json is damaged. Frame type declaration is not found in {token}");

                FrameType type = typeToken.ToObject<FrameType>();

                Frame frame = type switch
                {
                    FrameType.Dialogue => token.ToObject<DialogueFrame>(),
                    FrameType.Text => token.ToObject<TextFrame>(),
                    FrameType.Choice => token.ToObject<ChoiceFrame>(),
                    FrameType.Final => token.ToObject<FinalFrame>(),
                    _ => null
                };

                if (frame != null)
                {
                    _frames[frame.id] = frame;
                }
            }
        }

        public Frame GetFrameById(uint id)
        {
            return _frames.GetValueOrDefault(id);
        }

        public Frame GetStartFrame()
        {
            return _frames.FirstOrDefault().Value;
        }

    } // end of class
}