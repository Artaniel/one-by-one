using System;
using System.Collections;
using Game.Events;
using TMPro;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI notificationText = null;
    [SerializeField]
    private RectTransform notificationUICenter = null;
    [SerializeField]
    private float transitionTime = 0.35f;

    private static class AnimationState
    {
        public static int Urgency = 0;
        public static Vector2 oldSize = new Vector2(0, 0);
        public static Vector2 newSize = new Vector2(0, 0);
        public static float animationTime = 0f;
        public static bool inProgress = false;
    }

    private void Start()
    {
        EventManager.OnNotify.AddListener(OnNotify);
    }

    private void OnNotify(string text, int urgency)
    {
        if (urgency > AnimationState.Urgency)
        {
            AnimationState.Urgency = urgency;
            AnimationState.inProgress = true;
            
            var newWidth = Mathf.Min(text.Length * 10f, 640f);
            var newHeight = notificationUICenter.sizeDelta.y;
            ChangeUISize(newWidth, newHeight);
            
            notificationText.text = text;
        } 
        else Debug.Log("Not changing notification because urgency is lower than current");
        
    }

    private void Update()
    {
        if (AnimationState.inProgress)
        {
            var progress = Mathf.Clamp01(AnimationState.animationTime / transitionTime);
            AnimationState.animationTime += Time.deltaTime;
            
            var newW = Mathf.Lerp(AnimationState.oldSize.x, AnimationState.newSize.x, progress);
            var newH = Mathf.Lerp(AnimationState.oldSize.y, AnimationState.newSize.y, progress);
            notificationUICenter.sizeDelta = new Vector2(newW, newH);
            
            if (progress >= 1)
            {
                AnimationState.inProgress = false;
                AnimationState.Urgency = 0;
                AnimationState.animationTime = 0f;
            }
        }
    }
    
    void ChangeUISize(float newWidth, float newHeight)
    {
        AnimationState.oldSize = notificationUICenter.sizeDelta;

        AnimationState.newSize.x = newWidth;
        AnimationState.newSize.y = newHeight;
    }

    // IEnumerator ChangeUISize(float newWidth)
    // {
    //     var oldWidth = notificationUICenter.sizeDelta.x;
    //     var oldHeight = notificationUICenter.sizeDelta.y;
    //
    //     var n = 10;
    //     var t1 = Time.fixedTime;
    //     for (int i = 0; i < n; i++)
    //     {
    //         var newW = Mathf.Lerp(oldWidth, newWidth, Mathf.Clamp01((float) i / n));
    //         notificationUICenter.sizeDelta = new Vector2(newW, oldHeight);
    //         yield return new WaitForSeconds(transitionTime / n);
    //     }
    //     Debug.Log(Time.fixedTime - t1);
    // }
}