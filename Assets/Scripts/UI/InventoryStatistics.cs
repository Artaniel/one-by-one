using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryStatistics : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI monstersDefeated = null;
    [SerializeField] private TextMeshProUGUI timePassed = null;
    [SerializeField] private TextMeshProUGUI difficulty = null;
    [SerializeField] private TextMeshProUGUI artifactsNumber = null;

    SkillManager skillManager;

    void Awake()
    {
        skillManager = GameObject.FindGameObjectWithTag("Player").GetComponent<SkillManager>();
    }

    void OnEnable()
    {
        int timeM = Random.Range(10, 40);
        int timeS = Random.Range(10, 60);
        timePassed.text = $"{timeM}:{timeS}";

        int monstersDead = Random.Range(10, 450);
        monstersDefeated.text = monstersDead.ToString();

        difficulty.text = SaveLoading.difficulty.ToString() == "2" ? "Hardcore" : "Normal";

        artifactsNumber.text = skillManager.skills.Count.ToString();
    }
}
