using System;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class TransparencySetterUI : MonoBehaviour
{
    private float _alphaValue = 1f;
    public float AlphaValue
    {
        get => _alphaValue;
        set => _alphaValue = Mathf.Clamp01(value);
    }
    
    List<Tuple<Image, float>> images = new List<Tuple<Image, float>>(); 
    List<Tuple<Text, float>> texts = new List<Tuple<Text, float>>();
    List<Tuple<TextMeshProUGUI, float>> textsTMPro = new List<Tuple<TextMeshProUGUI, float>>();

    private void Start()
    {
        List<GameObject> toCheck = new List<GameObject> { gameObject };
        
        // Adding only objects with depth up to 10
        for (int i = 0; i < 10 && toCheck.Count != 0; i++)
            toCheck = AddImageComponents(toCheck);
    }

    List<GameObject> AddImageComponents(List<GameObject> toCheck)
    {
        List<GameObject> children = new List<GameObject>();
        
        foreach (var o in toCheck)
        {
            foreach (var component in o.GetComponents<Image>())
                images.Add(Tuple.Create(component, component.color.a));
            
            foreach (var component in o.GetComponents<Text>())
                texts.Add(Tuple.Create(component, component.color.a));
            
            foreach (var component in o.GetComponents<TextMeshProUGUI>())
                textsTMPro.Add(Tuple.Create(component, component.color.a));

            for (int i = 0; i < o.transform.childCount; i++)
                children.Add(o.transform.GetChild(i).gameObject);
        }

        return children;
    }

    public void SetAlpha(float val)
    {
        foreach (var t in images)
        {
            Color c = t.Item1.color;
            c.a = Mathf.Clamp01(t.Item2 * val);
            t.Item1.color = c;
        }
        
        foreach (var t in texts)
        {
            Color c = t.Item1.color;
            c.a = Mathf.Clamp01(t.Item2 * val);
            t.Item1.color = c;
        }
        
        foreach (var t in textsTMPro)
        {
            Color c = t.Item1.color;
            c.a = Mathf.Clamp01(t.Item2 * val);
            t.Item1.color = c;
        }
    }

    private void Update()
    {
        SetAlpha(AlphaValue);
    }
}
