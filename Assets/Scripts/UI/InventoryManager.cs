using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private CharacterShooting shooting;
    [SerializeField]
    public GameObject inventory = null;

    public static bool opened = true;

    public void Start()
    {
        opened = false;
        inventory.SetActive(false);
        var player = GameObject.FindGameObjectWithTag("Player");
        shooting = player.GetComponent<CharacterShooting>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab) || (inventory.activeSelf && Input.GetKeyDown(KeyCode.Escape)))
        {
            shooting.enabled = !shooting.enabled;
            inventory.SetActive(!inventory.activeSelf);
            MouseCursor.state = inventory.activeSelf ? MouseCursor.CursorState.HardwareRendered : MouseCursor.CursorState.SoftwareRendered;
            opened = !opened;
        }
    }
}
