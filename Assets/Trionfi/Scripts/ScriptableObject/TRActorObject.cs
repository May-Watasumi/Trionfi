using UnityEngine;
using System.Collections.Generic;

namespace Trionfi
{
    [System.Serializable]
    public class TRSpriteAlias : SerializableDictionary<string, string>
    {
        void CheckExist()
        {
            foreach (KeyValuePair<string, string> key in this)
            {
                Sprite _object = Resources.Load(key.Value) as Sprite;
                if (_object == null)
                    ErrorLogger.Log("Sprite not found : " + key.Value);
            }
        }
    }

    public class TRActorObject : ScriptableObject
    {
        [SerializeField]
        string actorName = "new_actor";
        [SerializeField]
        string voicePrefix = "";
        [SerializeField]
        bool hasVoice = false;

        [SerializeField]
        public TRSpriteAlias emotion = new TRSpriteAlias()
        {
            { "通常", null },
            { "喜", null },
            { "怒", null },
            { "楽", null },
            { "照", null },
            { "恥", null },
        };
    }
}