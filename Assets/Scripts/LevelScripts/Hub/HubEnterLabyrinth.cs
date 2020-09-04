using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubEnterLabyrinth : MonoBehaviour
{
    public string sceneToLoad = "Hub";
    private Scene scene;
    private int sceneToLoadInt;

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (!coll.CompareTag("Player")) return;

        if (loading) return;
        loading = true;
        RelodScene.OnSceneChange?.Invoke();
        SceneLoading.LoadScene(sceneToLoad);
    }

    private bool loading = false;
}
