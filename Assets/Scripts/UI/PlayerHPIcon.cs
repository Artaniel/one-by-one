using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHPIcon : MonoBehaviour
{
    public GameObject[] hpIcons;
    
    public void UpdateHP(int hp)
    {
        for (int i = 0; i < hp; i++)
        {
            hpIcons[i].SetActive(true);
        }
        for (int i = hp; i < hpIcons.Length; i++)
        {
            hpIcons[i].SetActive(false);
        }
    }
}
