using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubEpisodeAvailabilityManager : MonoBehaviour
{
    [SerializeField] private GameObject[] entrances = null; // id in this array same as called from bosses win scripts
    [SerializeField] private GameObject barrierToItems = null;
    [SerializeField] private GameObject barrierToPortals = null;

    private void Awake()
    {
        SetupEntersAvailability();
        HubZoneAvailability();
    }

    private void SetupEntersAvailability() {
        foreach (int id in SaveLoading.finishedEpisodes)
            TurnOffEpisodeEntrance(entrances[id]);
    }

    private void HubZoneAvailability()
    {
        if (SaveLoading.CheckAchievement(SaveLoading.AchievName.GameCompleted04))
        {
            barrierToPortals.SetActive(false);
        }
        if (SaveLoading.CheckAchievement(SaveLoading.AchievName.HardmodeCompleted04))
        {
            barrierToItems.SetActive(false);
        }
    }

    private void TurnOffEpisodeEntrance(GameObject entrance) {
        // здесь будет поведение которое нам нужно для выключения дверей. Анимации? Переключение спрайтов? Подсвека?
        // пока мы только выключаем те объекты, которые соответствуют завершенным эпизодам
        entrance.SetActive(false);
    }

    public static void EpisodeComplited(int id) { // expected to be called from episode boss win script        
        SaveLoading.AddFinishedEpisode(id);
    }
}
