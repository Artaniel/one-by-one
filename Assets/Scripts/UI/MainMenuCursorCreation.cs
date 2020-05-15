using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCursorCreation : MonoBehaviour
{
    // TODO: use CharacterShooting class inheritance maybe

    [SerializeField] private GameObject mouseCursorObj = null;
    [SerializeField] private Texture2D spriteTexture = null;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_WEBGL
        Cursor.SetCursor(spriteTexture, new Vector2(spriteTexture.width / 2, spriteTexture.height / 2), CursorMode.Auto);
#else
        Cursor.visible = false;
        Instantiate(mouseCursorObj, transform);
#endif
    }

    void Update()
    {
#if UNITY_WEBGL
        
#else
        Cursor.visible = false;
#endif

    }
}
