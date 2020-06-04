using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenContainer : MonoBehaviour
{
    [SerializeField] public GameObject NewGame;
    [SerializeField] public GameObject LoadGame;
    [SerializeField] public GameObject Settings;
    [SerializeField] public GameObject Credits;
    [SerializeField] public GameObject Exit;

    public GameObject GetButtonContinue()
    {
        return LoadGame;
    }

    public GameObject[] GetButtons()
    {
        return new GameObject[5]
        {
            NewGame, LoadGame, Settings, Credits, Exit
        };
    }

    public Sprite[] GetButtonSprites()
    {
        return new Sprite[5] {
            NewGame.GetComponent<Image>().sprite,
            LoadGame.GetComponent<Image>().sprite,
            Settings.GetComponent<Image>().sprite,
            Credits.GetComponent<Image>().sprite,
            Exit.GetComponent<Image>().sprite,
        };
    }
}
