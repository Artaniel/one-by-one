using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits04 : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
    }

    public void GoToMenu()
    {
        SceneLoading.LoadScene("MainMenu");
    }
}
