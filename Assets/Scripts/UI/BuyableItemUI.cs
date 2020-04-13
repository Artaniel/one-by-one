using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyableItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI outputTextField = null;
    [SerializeField] private Image itemSprite = null;

    private SkillBase itemAsset;

    private void Start()
    {
        itemAsset = transform.parent.GetComponent<BuyableItem>().itemAsset;

        outputTextField.text = itemAsset.price.ToString();
        itemSprite.sprite = itemAsset.pickupSprite;
    }
}
