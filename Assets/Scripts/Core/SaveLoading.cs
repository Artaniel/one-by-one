using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class SaveRecord {
    public Dictionary<string, string> formatedDictionary = new Dictionary<string, string>();
    public SkillsRecord skills;
}

[System.Serializable]
public class PermanentSaveRecord
{
    public Dictionary<string, int> achivements = new Dictionary<string, int>();
    public Dictionary<string, string> formatedDictionary = new Dictionary<string, string>(); // если нам понащдобится не int, чтобы не менять формат сохранения
}

public class SaveLoading : MonoBehaviour
{
    static private string fileName = "/progress.sav";
    static private string fileNamePermanent = "/permanent.sav";
    static public SaveRecord record;

    static public string currentScene = "Hub";
    static public SkillsRecord skills;
    static public int difficulty = 1; // 1 - default; 2 - hardmode
    static public string seed = "";
    static public List<int> finishedEpisodes = new List<int>();

    public enum AchievName { GameCompleted04, HardmodeCompleted04, FinishedTutorial3Once };
    static public Dictionary<AchievName, int> achievеments;

    private void Awake()
    {
        if (record == null)
            Load();
    }

    static public void SaveAll()
    {
        SaveProgress();
        SavePermanent();
    }

    static public void SaveProgress() {
        if (record != null)
        {
            record.formatedDictionary = new Dictionary<string, string>();
            record.formatedDictionary.Add("currentScene", currentScene);
            record.formatedDictionary.Add("difficulty", difficulty.ToString());
            record.formatedDictionary.Add("seed", seed);

            string s = "";
            foreach (int episodeID in finishedEpisodes)
            {
                s += episodeID.ToString() + ",";
            }
            record.formatedDictionary.Add("finishedEpisodes", s);

            record.skills = skills;

            BinaryFormatter binaryformatter = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + fileName);
            binaryformatter.Serialize(file, record);
            file.Close();
        }
    }

    static public void SavePermanent()
    {
        if (achievеments != null)
        {
            if (achievеments.Count > 0)
            {// to prevent rewrite empty achievemenst in case of save after load errors
                PermanentSaveRecord recordPermanent = new PermanentSaveRecord();
                recordPermanent.achivements = new Dictionary<string, int>();
                foreach (var achev in achievеments)
                {
                    recordPermanent.achivements.Add(achev.Key.ToString(), achev.Value);
                }
                BinaryFormatter binaryformatter = new BinaryFormatter();
                FileStream file = File.Create(Application.persistentDataPath + fileNamePermanent);
                binaryformatter.Serialize(file, recordPermanent);
                file.Close();
            }
        }
        else
        {
            Debug.Log("Trying to save before load");
            Load();
        }
    }

    static public void Load() {
        LoadProgress();
        LoadPermanent();
    }

    static private void LoadProgress() {
        if (File.Exists(Application.persistentDataPath + fileName))
        {
            BinaryFormatter binaryformatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + fileName, FileMode.Open);
            try
            {
                if (file.Length > 0)
                {
                    record = (SaveRecord)binaryformatter.Deserialize(file);
                    file.Close();
                    ParceProgressRecord();
                }
                else
                {
                    Debug.Log("Empty save file exception");
                    record = new SaveRecord();
                    file.Close();
                    CreateNewSave();
                }
            }
            catch (UnityException E)
            {
                Debug.Log("Unexpected error on save load.");
                Debug.Log(E);
                file.Close();
                CreateNewSave();
            }
        }
        else
        {
            CreateNewSave();
        }
    }

    static private void LoadPermanent()
    {
        PermanentSaveRecord permSaveRecord = new PermanentSaveRecord();
        if (File.Exists(Application.persistentDataPath + fileNamePermanent))
        {
            BinaryFormatter binaryformatterAch = new BinaryFormatter();
            FileStream fileAch = File.Open(Application.persistentDataPath + fileNamePermanent, FileMode.Open); try
            {
                if (fileAch.Length > 0)
                {
                    permSaveRecord = (PermanentSaveRecord)binaryformatterAch.Deserialize(fileAch);
                    fileAch.Close();
                    ParcePermanent(permSaveRecord);
                }
                else
                {
                    Debug.Log("Empty permanent save file exception");
                    permSaveRecord = new PermanentSaveRecord();
                    fileAch.Close();
                    // don't overwrite in case of error
                }
            }
            catch (UnityException E)
            {
                Debug.Log("Unexpected error on permanent save load.");
                Debug.Log(E);
                fileAch.Close();
                // don't overwrite in case of error
            }
        }
        else
        { // only if file is not found, make new
            PermanentSaveRecord recordPermanent = new PermanentSaveRecord();
            recordPermanent.achivements = new Dictionary<string, int>();
            recordPermanent.formatedDictionary = new Dictionary<string, string>();
            achievеments = new Dictionary<AchievName, int>();
        }
    }

    static private void ParceProgressRecord() {
        if (record.formatedDictionary.ContainsKey("currentScene"))
            currentScene = record.formatedDictionary["currentScene"];
        else
            currentScene = "Hub";

        difficulty = 0;  //default
        if (record.formatedDictionary.ContainsKey("difficulty"))
            int.TryParse(record.formatedDictionary["difficulty"], out difficulty);

        if (record.formatedDictionary.ContainsKey("seed"))
            seed = record.formatedDictionary["seed"];
        else
            seed = "";

        if (record.formatedDictionary.ContainsKey("finishedEpisodes")) {
            finishedEpisodes = new List<int>();
            foreach (string episode in record.formatedDictionary["finishedEpisodes"].Split(','))
            {
                if (!string.IsNullOrEmpty(episode))
                {
                    finishedEpisodes.Add(int.Parse(episode));
                }
            }
        }

        if (record.skills == null)
        {
            SkillManager skillManager = GameObject.FindWithTag("Player").GetComponent<SkillManager>();
            if (skillManager)
                skills = new SkillsRecord(skillManager.skills, skillManager.activeSkills, skillManager.equippedWeapons, 0);
        }
        else {
            skills = record.skills;
        }
    }

    static private void ParcePermanent(PermanentSaveRecord achevRecord)
    {
        achievеments = new Dictionary<AchievName, int>();
        foreach (var achev in achevRecord.achivements) {
            achievеments.Add(StringToAchivName(achev.Key), achev.Value);
        }
    }

    static private AchievName StringToAchivName(string s) {
        switch (s)
        {
            case "gameCompleted04":
                return AchievName.GameCompleted04;                
            case "hardmodeCompleted04":
                return AchievName.HardmodeCompleted04;
            case "finishedTutorial3Once":
                return AchievName.FinishedTutorial3Once;
            default:
                Debug.Log("error on achivement name deserelize");
                return AchievName.GameCompleted04; // невозможно вернуть нулл, надо заткнуть хоть чем то. В теории мы не должны тут оказаться.
        }
    }

    static private void CreateNewSave()
    {
        record = new SaveRecord();
        record.formatedDictionary = new Dictionary<string, string>();
        record.formatedDictionary.Add("currentScene", currentScene);
        record.formatedDictionary.Add("difficulty", difficulty.ToString());
        record.formatedDictionary.Add("seed", seed);
        
        GameObject.FindWithTag("Player").GetComponent<SkillManager>()?.SaveSkills(); // It will trigger SaveAll inside after creating skill record
    }

    static public void ResetNonPermanentSaveData()
    {
        currentScene = "Hub";
        SaveProgress();
    }

    static public void SaveCurrentScene(string sceneName) 
    {
        currentScene = sceneName;
        SaveProgress();
    }

    static public void SaveDiffilucty(int value)
    {
        difficulty = value;
        SaveProgress();
    }

    static public void SaveSeed(string value)
    {
        seed = value;
        SaveProgress();
    }

    static public void SaveAchievement(AchievName name, int value)
    {
        if (!achievеments.ContainsKey(name))
            achievеments.Add(name, value);
        else
            achievеments[name] = value;
        SavePermanent();
    }

    static public bool CheckAchievement(AchievName name) {
        if (achievеments == null)
            Load();
        if (achievеments.ContainsKey(name))
            return (achievеments[name] != 0);
        else
            return false;
    }

    static public void AddFinishedEpisode(int episodeID)
    {
        finishedEpisodes.Add(episodeID);
        SaveProgress();
    }

    static public void SaveSkills(SkillsRecord skillsRecord) {
        skills = skillsRecord;
        SaveProgress();
    }

    static public SkillsRecord LoadSkillsSafe() { // can be called even without Load() first, needed for start from not MainMenu
        if (skills == null) {
            Load();
        }
        return skills;
    }
}
