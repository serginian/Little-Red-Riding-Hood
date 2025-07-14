using System;
using System.Collections.Generic;
using serginian.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace serginian.Gameplay
{
    public sealed class CharacterContext: Context, IDisposable
    {
        private Dictionary<string, AsyncOperationHandle<Character>> _characterHandles = new Dictionary<string, AsyncOperationHandle<Character>>();
        
        public async Awaitable<Character> GetCharacter(string address)
        {
            if (_characterHandles.ContainsKey(address))
                return _characterHandles[address].Result;
            
            var characterHandle = Addressables.LoadAssetAsync<Character>(address);
            StoreCharacterHandle(address, characterHandle);
            await characterHandle.Task;
            return characterHandle.Result;
        }

        private void StoreCharacterHandle(string address, AsyncOperationHandle<Character> handle)
        {
            _characterHandles[address] = handle;
        }

        public void Dispose()
        {
            foreach (var handle in _characterHandles.Values)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
            _characterHandles.Clear();
        }
    }
}