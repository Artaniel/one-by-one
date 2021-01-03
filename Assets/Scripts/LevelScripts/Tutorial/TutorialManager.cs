using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public int phase = 0;

    public static TutorialManager instance;
    public GameObject door0To1;
    public Collider2D door0To1Blocker;
    public GameObject door1To2;
    public Collider2D door1To2Blocker;
    public GameObject door2To3;
    public Collider2D door2To3Blocker;
    public GameObject door3ToExit;
    public Collider2D door3ToExitBlocker;

    public GameObject mobPrefab;
    public Transform[] spawnPositions;

    private bool phase3started = false;
    private GameObject[] mobArray;

    private void Awake()
    {
        instance = this;
    }

    public void DoorStage0Hit() { 
        if (phase == 0)
        {
            door0To1Blocker.enabled = false;
            foreach (var animation in door0To1.GetComponentsInChildren<Animation>())
            {
                animation.Play();
            }
            foreach (var animation in door0To1.GetComponentsInChildren<Animator>())
            {
                animation.Play("Open");
            }
            phase = 1;
        }
    }

    public void Phase1Kill() {
        if (phase == 1)
        {
            door1To2Blocker.enabled = false;
            foreach (var animation in door1To2.GetComponentsInChildren<Animation>())
            {
                animation.Play();
            }
            foreach (var animation in door1To2.GetComponentsInChildren<Animator>())
            {
                animation.Play("Open");
            }
            phase = 2;
        }
        else Debug.Log("Error on tutorial phase change.");
    }
    
    public void Phase2Kill()
    {
        if (phase == 2)
        {
            door2To3Blocker.enabled = false;
            foreach (var animation in door2To3.GetComponentsInChildren<Animation>())
            {
                animation.Play();
            }
            foreach (var animation in door2To3.GetComponentsInChildren<Animator>())
            {
                animation.Play("Open");
            }
            phase = 3;
        }
        else Debug.Log("Error on tutorial phase change.");
    }

    public void Phase3StartTrigger() {
        if (!phase3started)
        {
            mobArray = new GameObject[spawnPositions.Length];
            //mobList = new List<GameObject>();
            //foreach (Transform targetTransform in spawnPositions)
            for (int i = 0; i < spawnPositions.Length; i++)
            {
                Spawn(spawnPositions[i], mobPrefab, i);
            }
            phase3started = true;
        }
    }

    public void Phase3Kill() {
        bool aliveFound = false;
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            bool dead = false;
            if (mobArray[i] == null) { // exception to prevent call of MonsterLife on deleted gobject
                dead = true;
            }else if (mobArray[i].GetComponent<MonsterLife>().HP <= 0)
                dead = true;
            if (dead)
                mobArray[i] = null;
            else
                aliveFound = true;
        }
        if (!aliveFound)
        {
            OpenExit();
            phase = 4;
        }
    }

    public void Phase3Absorb() {
        for (int i = 0; i < spawnPositions.Length; i++) {
            if (mobArray[i] == null)
                Spawn(spawnPositions[i], mobPrefab, i);
        }
    }

    private void Spawn(Transform targetTransform, GameObject mobPrefab, int id) {
        GameObject mob = Instantiate(mobPrefab, targetTransform.position, targetTransform.rotation);
        mobArray[id] = mob;
        MonsterLife monsterlife = mob.GetComponent<MonsterLife>();
        monsterlife.OnThisDead.AddListener(Phase3Kill);
        monsterlife.OnThisAbsorb.AddListener(Phase3Absorb);
        monsterlife.maxHP = 1;
        monsterlife.HP = 1;
        mob.GetComponent<MoveForward>().enabled = false;
    }

    private void OpenExit() {
        door3ToExitBlocker.enabled = false;
        foreach (var animation in door3ToExit.GetComponentsInChildren<Animation>())
        {
            animation.Play();
        }
        foreach (var animation in door3ToExit.GetComponentsInChildren<Animator>())
        {
            animation.Play("Open");
        }
        SaveLoading.SaveAchievement(SaveLoading.achevNames.finishedTutorial3Once, 1);
    }
}
