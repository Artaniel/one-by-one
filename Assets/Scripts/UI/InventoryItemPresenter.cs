﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemPresenter : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public Image itemImage = null;
    [SerializeField] public GameObject emptyCell = null;
    [SerializeField] private Sprite baseImg = null;

    private SkillBase currentSkill = null;
    private Inventory inventory = null;

    private Transform draggingParent;
    private Transform originalParent;
    private Sprite originalFrame;
    bool onDrag = false;

    public void Init(Transform draggingparent)
    {
        draggingParent = draggingparent;
        originalParent = transform.parent;
        Reboot();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentSkill && inventory) inventory.UpdateToolTip(currentSkill);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inventory) inventory.RemoveToolTip();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventory != null)
        {
            onDrag = true;
            var cellFrameImage = transform.parent.GetComponent<Image>();
            originalFrame = cellFrameImage.sprite;
            cellFrameImage.sprite = inventory.weaponEmptyFrame;
            transform.SetParent(draggingParent);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (inventory != null)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (inventory != null)
        {
            int closestIndex = 0;
            for (int i = 0; i < originalParent.parent.transform.childCount; i++)
            {
                if (Vector3.Distance(transform.position, originalParent.parent.transform.GetChild(i).position) <
                    Vector3.Distance(transform.position, originalParent.parent.transform.GetChild(closestIndex).position))
                {
                    closestIndex = i;
                }
            }
            var destinationCell = originalParent.parent.GetChild(closestIndex);
            if (destinationCell.childCount == 0)
            {
                transform.SetParent(destinationCell);
                transform.localPosition = new Vector2(0, 0);
                transform.parent.GetComponent<Image>().sprite = originalFrame;
            }
            else
            {
                var tmpFrame = destinationCell.GetComponent<Image>().sprite;
                Transform destinationSkillImage = destinationCell.GetChild(2);
                destinationSkillImage.SetParent(originalParent);
                destinationSkillImage.transform.localPosition = new Vector2(0, 0);
                destinationSkillImage.GetComponent<InventoryItemPresenter>().SetOriginalParent(originalParent);
                destinationSkillImage.parent.GetComponent<Image>().sprite = tmpFrame;
                transform.SetParent(destinationCell);
                transform.parent.GetComponent<Image>().sprite = originalFrame;
                transform.localPosition = new Vector2(0, 0);
            }
            originalParent = transform.parent;
            onDrag = false;
        }
    }

    public void Render(SkillBase item, Inventory inventory)
    {
        itemImage.enabled = true;
        if (item is WeaponSkill)
        {
            if (item.pickupSprite) itemImage.sprite = item.pickupSprite;
            else itemImage.sprite = item.miniIcon;
        }
        else
        {
            if (item.miniIcon) itemImage.sprite = item.miniIcon;
            else itemImage.sprite = item.pickupSprite;
        }

        currentSkill = item;
        this.inventory = inventory;
    }

    private void Reboot()
    {
        itemImage.sprite = baseImg;
    }

    public void SetOriginalParent(Transform parent)
    {
        originalParent = parent;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!onDrag)
            if (inventory != null)
                inventory.OnCellClick(currentSkill, transform);
    }
}
