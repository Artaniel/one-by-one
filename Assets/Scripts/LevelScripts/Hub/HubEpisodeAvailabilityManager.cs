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
        string complitedEpisodes = PlayerPrefs.GetString("finishedEpisodes");
        if (complitedEpisodes != "")
        {
            if (complitedEpisodes.Contains(","))
                foreach (string idString in complitedEpisodes.Split(','))
                    TurnOffEpisodeEntrance(entrances[int.Parse(idString)]);
            else
                TurnOffEpisodeEntrance(entrances[int.Parse(complitedEpisodes)]);
        }
    }

    private void HubZoneAvailability()
    {
        if (PlayerPrefs.GetInt("GameCompleted04", -1) == 4)
        {
            barrierToPortals.SetActive(false);
        }
        if (PlayerPrefs.GetInt("HardmodeCompleted04", -1) == 4)
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
        string s = PlayerPrefs.GetString("finishedEpisodes");
        if (s != "") s += ",";
        s += id.ToString();
        PlayerPrefs.SetString("finishedEpisodes", s);
        PlayerPrefs.Save();
    }

    public static void ClearComplitedEpisodesList() {
        PlayerPrefs.SetString("finishedEpisodes", "");
        PlayerPrefs.Save();
    }
}
