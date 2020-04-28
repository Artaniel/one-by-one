using System;
using UnityEngine;

namespace Game.Network
{
    [Serializable]
    public class JsonDeserializer
    {
        public static T CreateFromJSON<T>(string jsonString)
        {
            return JsonUtility.FromJson<T>(jsonString);
        }
    }
}