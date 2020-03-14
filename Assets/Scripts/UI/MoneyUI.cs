using Game.Events;
using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI сounterUI = null;

    private void Start()
    {
        сounterUI.text = MoneyManager.MoneyAmount.ToString();
        EventManager.OnMoneyChange.AddListener(UpdateMoneyCounter);
    }

    void UpdateMoneyCounter(int delta)
    {
        сounterUI.text = MoneyManager.MoneyAmount.ToString();
    }
}