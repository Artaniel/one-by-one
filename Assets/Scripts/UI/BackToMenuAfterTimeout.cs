using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenuAfterTimeout : MonoBehaviour
{
    [SerializeField]
    private float TimeToMenu = 20f;
    private float TimeLeft = 1;
    private bool loading = false;

    // Start is called before the first frame update
    void Start()
    {
        TimeLeft = TimeToMenu;
    }

    // Update is called once per frame
    void Update()
    {
        if (loading) return;

        TimeLeft -= Time.deltaTime;
        if (TimeLeft < 0)
        {
            loading = true;
            SceneLoading.LoadScene("Credits04");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            loading = true;
            SceneLoading.LoadScene("Credits04");
        }
    }
}
