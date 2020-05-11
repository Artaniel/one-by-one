using System;
using UnityEngine;
using System.Collections.Generic;

namespace Game.Network.Scoreboard
{
    [Serializable]
    public class ScoreboardEntry
    {
        public int place;
        public string playerName;
        public float gameTime;
    }

    [Serializable]
    public class ScoreboardSendEntry
    {
        public string playerName;
        public float gameTime;
    }

    [Serializable]
    public class ScoreboardList
    {
        public List<ScoreboardEntry> entries;
    }
}