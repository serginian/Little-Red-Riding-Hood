using UnityEngine;

namespace serginian.Data
{
    [CreateAssetMenu(menuName = "serginian/Create Character", fileName = "New Character", order = 0)]
    public class Character : ScriptableObject
    {
        public string nameKey;
        public Sprite sprite;
        public Color nameColor;
    }
}