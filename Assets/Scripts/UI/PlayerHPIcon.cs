using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHPIcon : MonoBehaviour
{
    public GameObject[] hpIcons;
    
    public void UpdateHP(int hp, int maxHp)
    {
        for (int i = 0; i < hp; i++)
        {
            hpIcons[i].transform.GetChild(0).gameObject.SetActive(true);
            hpIcons[i].transform.GetChild(1).gameObject.SetActive(false);
            hpIcons[i].transform.GetChild(2).gameObject.SetActive(false);
        }
        for (int i = hp; i < maxHp; i++)
        {
            hpIcons[i].transform.GetChild(0).gameObject.SetActive(false);
            hpIcons[i].transform.GetChild(1).gameObject.SetActive(true);
            hpIcons[i].transform.GetChild(2).gameObject.SetActive(false);
        }
        for (int i = maxHp; i < hpIcons.Length; i++)
        {
            hpIcons[i].transform.GetChild(0).gameObject.SetActive(false);
            hpIcons[i].transform.GetChild(1).gameObject.SetActive(false);
            hpIcons[i].transform.GetChild(2).gameObject.SetActive(true);
        }
    }
}
