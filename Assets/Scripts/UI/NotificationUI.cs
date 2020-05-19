using System;
using System.Collections;
using Game.Events;
using TMPro;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notificationText = null;
    [SerializeField] private RectTransform notificationUICenter = null;
    [SerializeField] private float transitionTime = 0.35f;
    [SerializeField] private float fadeInTime = 0.1f;
    [SerializeField] private float fadeOutTime = 0.1f;
    [SerializeField] private float showTime = 2f;

    public void HideImmediate()
    {
        AnimationState.FadeInProgress = false;
        AnimationState.AnimInProgress = false;
        AnimationState.AnimationTime = 0f;
        AnimationState.OnScreenTime = 0f;
        AnimationState.AlphaValue = 0f;
        AnimationState.Urgency = 0;

        transparencyManager.AlphaValue = 0f;
    }

    private TransparencySetterUI transparencyManager = null;

    private static class AnimationState
    {
        public static int Urgency = 0;
        public static Vector2 OldSize = new Vector2(0, 0);
        public static Vector2 NewSize = new Vector2(0, 0);
        
        public static float AnimationTime = 0f;
        public static bool AnimInProgress = false;

        public static float OnScreenTime = 0f;
        public static float AlphaValue = 0f;
        public static bool FadeInProgress = false;
    }

    private void Awake()
    {
        EventManager.OnNotify.AddListener(OnNotify);
        transparencyManager = GetComponent<TransparencySetterUI>();
        transparencyManager.AlphaValue = 0f;
    }

    private void OnNotify(string text, int urgency)
    {
        if (AnimationState.OnScreenTime > fadeInTime + showTime || urgency > AnimationState.Urgency)
        {
            AnimationState.Urgency = urgency;
            AnimationState.AnimInProgress = true;
            AnimationState.FadeInProgress = true;
            AnimationState.OnScreenTime = 0f;
            
            var newWidth = Mathf.Min(text.Length * 8f, 640f);
            var newHeight = notificationUICenter.sizeDelta.y;
            ChangeUISize(newWidth, newHeight);
            
            notificationText.text = text;
        } 
        // else Debug.Log("Not changing notification because urgency is lower than current");
        
    }

    private void Update()
    {
        if (AnimationState.AnimInProgress)
        {
            var progress = Mathf.Clamp01(AnimationState.AnimationTime / transitionTime);
            AnimationState.AnimationTime += Time.deltaTime;
            
            var newW = Mathf.Lerp(AnimationState.OldSize.x, AnimationState.NewSize.x, progress);
            var newH = Mathf.Lerp(AnimationState.OldSize.y, AnimationState.NewSize.y, progress);
            notificationUICenter.sizeDelta = new Vector2(newW, newH);
            
            if (progress >= 1)
            {
                AnimationState.AnimInProgress = false;
                AnimationState.AnimationTime = 0f;
            }
        }
        
        if (AnimationState.FadeInProgress)
            AnimationState.OnScreenTime += Time.deltaTime;

        if (AnimationState.FadeInProgress && AnimationState.OnScreenTime < fadeInTime + showTime + fadeOutTime + 0.1)
        {
            if (AnimationState.OnScreenTime < fadeInTime + showTime)
                AnimationState.AlphaValue = Mathf.Max(
                    Mathf.Clamp01(AnimationState.OnScreenTime / fadeInTime),
                    AnimationState.AlphaValue
                );
            else if (AnimationState.OnScreenTime >= fadeOutTime + showTime)
            {
                AnimationState.AlphaValue =
                    1 - Mathf.Clamp01((AnimationState.OnScreenTime - showTime - fadeInTime) / fadeOutTime);
                AnimationState.Urgency = 0;
            }
            
            transparencyManager.AlphaValue = AnimationState.AlphaValue;
        }
        else AnimationState.FadeInProgress = false;
    }

    private void ChangeUISize(float newWidth, float newHeight)
    {
        AnimationState.OldSize = notificationUICenter.sizeDelta;

        AnimationState.NewSize.x = newWidth;
        AnimationState.NewSize.y = newHeight;
    }
}