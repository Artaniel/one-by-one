using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "TutorialShovelSkill", menuName = "ScriptableObject/ActiveSkill/TutorialShovelSkill", order = 11)]
public class TutorialShovel : ActiveSkill
{
    public override void ActivateSkill()
    {
        PlayerPrefs.SetInt("FinishedTutorialOnce", 1);
        SceneManager.LoadScene("LabirintChapter1");
    }
}
