using System;
using System.Collections.Generic;
using UnityEngine;

namespace serginian.Gameplay
{
    // Service-Locator to handle dependencies
    public sealed class GameContext: IDisposable
    {
        private readonly Dictionary<Type, object> _dependencies = new();
        
        private GameContext()
        {
        }
        
        public static GameContext Create()
        {
            return new GameContext();
        }
        
        public GameContext AddContext<T>(T dependency) where T : Context
        {
            if (dependency == null)
                throw new ArgumentNullException(nameof(dependency));
            
            dependency.AssignContext(this);
            _dependencies[typeof(T)] = dependency;
            return this;
        }

        public GameContext RemoveContext<T>(T dependency) where T : Context
        {
            if (dependency == null)
                throw new ArgumentNullException(nameof(dependency));
                
            _dependencies.Remove(typeof(T));
            return this;
        }
        
        public T GetContext<T>() where T : Context
        {
            if (_dependencies.TryGetValue(typeof(T), out object dependency))
            {
                return (T)dependency;
            }
            
            throw new InvalidOperationException($"Dependency of type {typeof(T).Name} not found");
        }
        
        public bool HasContext<T>() where T : Context
        {
            return _dependencies.ContainsKey(typeof(T));
        }

        public void Dispose()
        {
            foreach (var dependency in _dependencies.Values)
            {
                if (dependency is not IDisposable disposable) 
                    continue;
                
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error disposing dependency of type {dependency.GetType().Name}: {ex.Message}");
                }
            }
            
            _dependencies.Clear();
        }
    }
}