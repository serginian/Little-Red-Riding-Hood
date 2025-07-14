using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace serginian.UI
{
    public class UiMaster : MonoBehaviour
    {
        public string contextID;

        private static readonly List<UiWindow> AllWindows = new List<UiWindow>();

        private static readonly Dictionary<Type, AsyncOperationHandle<GameObject>> LoadedWindows =
            new Dictionary<Type, AsyncOperationHandle<GameObject>>();

        public static string CurrentContext { get; private set; } = String.Empty;

        
        /********************** MONO BEHAVIOUR **********************/
        
        private void Awake()
        {
            // Find all UiWindow components in children
            UiWindow[] childWindows = GetComponentsInChildren<UiWindow>(true);
            
            // Register each found window
            foreach (UiWindow window in childWindows)
                RegisterWindow(window);
        }
        
        

        /********************** PUBLIC INTERFACE **********************/

        /// <summary>
        /// Sets the current UI context.
        /// </summary>
        /// <param name="contextID">The context identifier to set.</param>
        public static void SetContext(string contextID)
        {
            CurrentContext = contextID;
        }

        /// <summary>
        /// Retrieves an existing window of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the UI window to retrieve.</typeparam>
        /// <returns>The window of type T if found; otherwise, null.</returns>
        public static T GetWindow<T>() where T : UiWindow
        {
            foreach (var window in AllWindows)
                if (window.GetType() == typeof(T))
                    return window as T;
            return null;
        }

        /// <summary>
        /// Retrieves an existing window or creates a new one if it does not exist.
        /// </summary>
        /// <typeparam name="T">The type of the UI window.</typeparam>
        /// <param name="contextID">The optional context identifier.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the requested window.</returns>
        public static async Task<T> GetOrCreateWindow<T>(string contextID = "") where T : UiWindow
        {
            T window = GetWindow<T>();
            if (window == null)
                return await CreateWindow<T>(contextID);
            return window;
        }

        /// <summary>
        /// Creates a new window of the specified type within a given context.
        /// </summary>
        /// <typeparam name="T">The type of the UI window to create.</typeparam>
        /// <param name="contextID">The optional context identifier.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created window.</returns>
        /// <exception cref="Exception">Thrown when no UiMaster component with the specified context exists in the scene.</exception>
        public static async Task<T> CreateWindow<T>(string contextID = "") where T : UiWindow
        {
            var allMasters = FindObjectsByType<UiMaster>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            var uiContext = string.IsNullOrWhiteSpace(contextID) ? CurrentContext : contextID;
            UiMaster parent = allMasters.FirstOrDefault(t => t.contextID == uiContext);

            if (parent == null)
                throw new Exception($"There is no UiMaster component in the scene with context \"{uiContext}\".");

            return await CreateWindow_Internal<T>(uiContext, parent);
        }

        /// <summary>
        /// Creates a new window of the specified type within a specified UI master and window context.
        /// </summary>
        /// <typeparam name="T">The type of the UI window to create.</typeparam>
        /// <param name="uiMasterContext">The context identifier for the UiMaster.</param>
        /// <param name="windowContext">The context identifier for the window.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created window.</returns>
        /// <exception cref="Exception">Thrown when no UiMaster component with the specified context exists in the scene.</exception>
        public static async Task<T> CreateWindow<T>(string uiMasterContext, string windowContext) where T : UiWindow
        {
            var allMasters = FindObjectsByType<UiMaster>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            UiMaster parent = allMasters.FirstOrDefault(t => t.contextID == uiMasterContext);

            if (parent == null)
                throw new Exception($"There is no UiMaster component in the scene with context \"{uiMasterContext}\".");

            return await CreateWindow_Internal<T>(windowContext, parent);
        }
        


        /********************** INNER LOGIC **********************/

        private static async Task<T> CreateWindow_Internal<T>(string uiContext, UiMaster uiMaster) where T : UiWindow
        {
            string windowName = typeof(T).Name;
            var address = !String.IsNullOrWhiteSpace(uiContext) ? $"UI/{uiContext}/{windowName}" : $"UI/{windowName}";

            if (LoadedWindows.TryGetValue(typeof(T), out var e) && e.IsValid())
                return await InstantiateWindowAsync<T>(e, uiMaster);

            var handle = Addressables.LoadAssetAsync<GameObject>(address);
            handle.WaitForCompletion();
            
            LoadedWindows[typeof(T)] = handle;
            return await InstantiateWindowAsync<T>(handle, uiMaster);
        }

        private static async Task<T> InstantiateWindowAsync<T>(AsyncOperationHandle<GameObject> handle, UiMaster parent)
            where T : UiWindow
        {
            var obj = await InstantiateAsync(handle.Result, parent.transform);
            var window = obj[0].GetComponent<T>();
            
            var rectTransform = window.GetComponent<RectTransform>();
            await Awaitable.NextFrameAsync();
            ResetWindowTransform(rectTransform);

            if (!window)
                throw new Exception($"{typeof(T).Name} does not have a UiWindow component.");

            RegisterWindow(window);
            return window;
        }

        private static void ResetWindowTransform(RectTransform rectTransform)
        {
            // Store original anchor settings
            Vector2 originalAnchorMin = rectTransform.anchorMin;
            Vector2 originalAnchorMax = rectTransform.anchorMax;
    
            // Reset position and scale first
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = Vector3.zero;
    
            // Restore original anchors
            rectTransform.anchorMin = originalAnchorMin;
            rectTransform.anchorMax = originalAnchorMax;
    
            // Now set offsets to 0 - this should work correctly
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

        }

        private static void RegisterWindow(UiWindow window)
        {
            if (AllWindows.Contains(window))
                return;
            
            AllWindows.Add(window);
            window.OnRelease += UnregisterWindow;
        }

        private static void UnregisterWindow(UiWindow window)
        {
            AllWindows.Remove(window);
            
            Type windowType = window.GetType();
            if (LoadedWindows.TryGetValue(windowType, out var handle) && handle.IsValid())
            {
                Addressables.Release(handle);
                LoadedWindows.Remove(windowType);
            }
        }
        
    } // end of class
}