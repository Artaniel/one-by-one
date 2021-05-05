﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "TutorialShovelSkill", menuName = "ScriptableObject/ActiveSkill/TutorialShovelSkill", order = 11)]
public class TutorialShovel : ActiveSkill
{
    protected override void ActivateSkill()
    {
        PlayerPrefs.SetInt("FinishedTutorial3Once", 1);
        SceneLoading.LoadScene("LabirintChapter1");
    }
}
