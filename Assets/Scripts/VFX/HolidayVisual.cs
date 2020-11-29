using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolidayVisual : MonoBehaviour
{
    [SerializeField] Vector2 dayMonthFrom = Vector2.one;
    [SerializeField] Vector2 dayMonthTo = Vector2.one;

    [Header("Choose anything you want")]
    [SerializeField] Sprite sprite;
    [SerializeField] RuntimeAnimatorController animator;
    [SerializeField] GameObject holidayObject;

    void Start()
    {
        var timeNow = System.DateTime.Now;

        if (timeNow.Month >= dayMonthFrom.y && timeNow.Month <= dayMonthTo.y &&
            timeNow.Day >= dayMonthFrom.x && timeNow.Day <= dayMonthTo.x)
        {
            if (sprite)
            {
                if (animator)
                {
                    GetComponent<Animator>().runtimeAnimatorController = animator;
                }
                GetComponent<SpriteRenderer>().sprite = sprite;
            }
            if (holidayObject)
            {
                Instantiate(holidayObject, transform);
            }
        }
    }
}
