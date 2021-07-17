using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class LocalizationManager : MonoBehaviour
{
    //  контент
    //      перебрать скиллы
    //      пересмотреть спски названий локации для всех боевых сцен

    //  баги
    //      запоминает название при смене языка и Continue

    //  cleanup
    //      отключить настройки на главной сцене

    public enum Locale {EN, RU};
    static public Locale currentLocale = Locale.EN;
    static Dictionary<Locale,Dictionary<int,string>> translations;
    static public TMP_FontAsset ruFont;
    public TMP_FontAsset defaultRUFont;
    public static UnityEvent OnLanguageSwich = new UnityEvent();

    private void Awake()
    {
        Init();
        LoadTranslations();
        ruFont = defaultRUFont;
        if (PlayerPrefs.HasKey("Language"))
            if (PlayerPrefs.GetString("Language") != "EN")
                ChangeLanguage(PlayerPrefs.GetString("Language"));
    }

    static public void Init() {
        translations = new Dictionary<Locale, Dictionary<int, string>>();
        translations[Locale.EN] = new Dictionary<int, string>();
        translations[Locale.RU] = new Dictionary<int, string>();
    }

    public void ChangeLanguage(string lang) {
        if (lang == "EN" && currentLocale != Locale.EN)
        {
            currentLocale = Locale.EN;
            SaveLoading.sceneNameForCurrentLocation = ""; // needed to prevent saving loc name on one language and load same on different
            OnLanguageSwich.Invoke();
        }
        if (lang == "RU" && currentLocale != Locale.RU)
        {
            currentLocale = Locale.RU;
            SaveLoading.sceneNameForCurrentLocation = "";
            OnLanguageSwich.Invoke();
        }
        //reinit all... event?
        Debug.Log(currentLocale);

        PlayerPrefs.SetString("Language", lang);
    }

    static public string GetTranlationENtoCurrent(string textEN) {
        textEN = textEN.Trim();
        if (textEN == "") return "";
        foreach (int key in translations[Locale.EN].Keys) 
            if (translations[Locale.EN][key] == textEN)
                return translations[currentLocale][key];
        Debug.LogError("error in translation, EN text not found \n"+ textEN);
        return textEN;
    }

    static public string GetTranlationByID(int ID)
    {
        if (translations[currentLocale].ContainsKey(ID))
            return translations[currentLocale][ID];
        else
        {
            Debug.LogError("error in translation, key text not found \n" + ID.ToString());
            return translations[Locale.EN][ID];
        }
    }

    static public LocationName GetLocationNameFromList(List<LocationName> listLocationNames) {
        LocationName ENlocationName = null;
        foreach (LocationName locationName in listLocationNames) {
            if (locationName.language == currentLocale)
                return locationName;
            if (locationName.language == Locale.EN)
                ENlocationName = locationName;
        }
        return ENlocationName;
    }

    static public EvilDictionary GetEvilDictionaryFromList(List<EvilDictionary> list) {
        EvilDictionary ENEvilDictionary = null;
        foreach (EvilDictionary evilDictionary in list)
        {
            if (evilDictionary.language == currentLocale)
                return evilDictionary;
            if (evilDictionary.language == Locale.EN)
                ENEvilDictionary = evilDictionary;
        }
        return ENEvilDictionary;
    }

    static public void LoadTranslations()
    {
        translations[Locale.EN][13] = "Start";
        translations[Locale.RU][13] = "Старт";

        translations[Locale.EN][18] = "Bestiary\n[Coming soon!]";
        translations[Locale.RU][18] = "Бестиарий\n[В разработке!]";
        translations[Locale.EN][19] = "Event room\n[Coming soon!]";
        translations[Locale.RU][19] = "Комната эвентов\n[В разработке!]";
        translations[Locale.EN][20] = "????\n[Coming soon!]";
        translations[Locale.RU][20] = "????\n[В разработке!]";
        translations[Locale.EN][21] = "Character selection\n[Coming soon!]";
        translations[Locale.RU][21] = "Выбор персонажа\n[В разработке!]";
        translations[Locale.EN][22] = "Portal room\n[Complete the game]";
        translations[Locale.RU][22] = "Комната порталов\n[Пройти игру]";
        translations[Locale.EN][23] = "Item room\n[Complete the game \n on < color = red > hardmode </ color >]";
        translations[Locale.RU][23] = "Комната предметов\n[Закончить игру\nна <color=red>хардмоде</color>]";
        translations[Locale.EN][24] = "I am entering";
        translations[Locale.RU][24] = "Я захожу в";
        translations[Locale.EN][25] = "Just wait a moment...";
        translations[Locale.RU][25] = "Один момент...";
        translations[Locale.EN][26] = "Press <color=red>R</color> to restart.";
        translations[Locale.RU][26] = "Нажмите  <color=red>R</color> для рестарта.";
        translations[Locale.EN][27] = "Time:";
        translations[Locale.RU][27] = "Время:";
        translations[Locale.EN][28] = "Difficulty:";
        translations[Locale.RU][28] = "Сложность:";
        translations[Locale.EN][29] = "Monsters:";
        translations[Locale.RU][29] = "Монстры:";
        translations[Locale.EN][30] = "Artifacts:";
        translations[Locale.RU][30] = "Артефакты:";
    }
}
