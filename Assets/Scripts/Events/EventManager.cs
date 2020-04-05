using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Events
{
    [Serializable]
    public class CoinPickupEvent : UnityEvent<Vector3, int> {}
    
    [Serializable]
    public class MonsterDeathEvent : UnityEvent<Vector3> {}
    
    [Serializable]
    public class MoneyChangeEvent : UnityEvent<int> {}
    
    [Serializable]
    public class NotificationEvent: UnityEvent<string, int> {}
    
    public class EventManager : MonoBehaviour
    {
        public static readonly CoinPickupEvent OnCoinPickup = new CoinPickupEvent();
        public static readonly MonsterDeathEvent OnMonsterDead = new MonsterDeathEvent();
        public static readonly MoneyChangeEvent OnMoneyChange = new MoneyChangeEvent(); 
        public static readonly NotificationEvent OnNotify = new NotificationEvent();

        public static void Notify(string message, int urgency) => OnNotify.Invoke(message, urgency);
    }
}
