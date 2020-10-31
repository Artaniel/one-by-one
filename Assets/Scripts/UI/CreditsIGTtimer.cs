using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreditsIGTtimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;

    private void Awake()
    {
        float summ = 0;
        if (Metrics.MetricsContainer == null) // custom type does not return bool
            Metrics.LoadMetrics(); // case for run from credits scene, debug only
        for (int i = 0; i < Metrics.MetricsContainer.levelComlpeted.Length; i++) {
            if (Metrics.MetricsContainer.levelComlpeted[i]) {
                summ += Metrics.MetricsContainer.levelTime[i];
            }
        }
        float minutes = Mathf.Floor(summ / 60f);
        float seconds = Mathf.Floor(summ - (minutes * 60f));
        float milisecs = Mathf.Floor((summ - (minutes * 60f) - seconds) * 1000f);
        textMesh.text = minutes.ToString() + ":" + seconds.ToString() + ".";
        if (milisecs < 100) textMesh.text += "0";
        if (milisecs < 10) textMesh.text += "0";
        textMesh.text += milisecs.ToString();
    }
}
