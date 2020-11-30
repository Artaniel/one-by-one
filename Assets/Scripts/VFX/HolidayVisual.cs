using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolidayVisual : MonoBehaviour
{
    [SerializeField] Vector2 dayMonthFrom = Vector2.one;
    [SerializeField] Vector2 dayMonthTo = Vector2.one;

    [Header("Choose anything you want")]
    [SerializeField] Sprite sprite = null;
    [SerializeField] RuntimeAnimatorController animator = null;
    [SerializeField] GameObject holidayObject = null;

    void Start()
    {
        var timeNow = System.DateTime.Now;

        if (DateCheck())
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

    private bool DateCheck()
    {
        var timeNow = System.DateTime.Now;
        var from = new System.DateTime(timeNow.Year, (int)dayMonthFrom.y, (int)dayMonthFrom.x, 0, 0, 0);
        var to = new System.DateTime(timeNow.Year, (int)dayMonthTo.y, (int)dayMonthTo.x, 0, 0, 0);
        if (dayMonthFrom.y > dayMonthTo.y)
        {
            to.AddYears(1);
        }

        return timeNow > from && timeNow < to;
    }
}
