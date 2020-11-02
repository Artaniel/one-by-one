using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PassiveItemPresenter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    public Image itemImage = null;

    private SkillBase currentSkill = null;
    private Inventory inventory = null;
    public void Render(SkillBase item, Inventory inventory)
    {
        itemImage.sprite = item.pickupSprite;
        Image img = GetComponent<Image>();
        img.color = Color.white;
        currentSkill = item;
        this.inventory = inventory;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentSkill && inventory) inventory.UpdateToolTip(currentSkill);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inventory) inventory.RemoveToolTip();
    }
}
