using System;
using Game.Events;
using UnityEngine;

[Serializable]
public class AlphaManager
{
    [SerializeField] 
    private string managerName = "";
    [SerializeField]
    private TransparencySetterUI transparencySetter;
    [SerializeField]
    private float fadeInTime = 0f;
    [SerializeField]
    private float showTime = 2f;
    [SerializeField]
    private float fadeOutTime = 0f;
    [SerializeField] 
    private bool autoHide = true;
    
    private AnimationProgress animationState;
    
    private enum AnimationState
    { IDLE, FADEIN, SHOW, FADEOUT }

    private class AnimationProgress
    {
        public float OnScreenTime = 0f;
        public float AlphaValue = 0f;
        public bool FadeInProgress = false;
        public AnimationState state = AnimationState.IDLE;
    }

    public AlphaManager()
    {
        animationState = new AnimationProgress();
    }

    public AlphaManager(TransparencySetterUI setter, float fadeInTime, float showTime, float fadeOutTime, bool autoHide)
    {
        this.transparencySetter = setter;
        this.fadeInTime = fadeInTime;
        this.showTime = showTime;
        this.fadeOutTime = fadeOutTime;
        this.autoHide = autoHide;
        animationState = new AnimationProgress();
    }

    public void Show()
    {
        animationState.FadeInProgress = true;
        animationState.state = AnimationState.FADEIN;
    }

    public void Hide()
    {
        animationState.FadeInProgress = true;
        animationState.OnScreenTime = Mathf.Max(animationState.OnScreenTime, fadeInTime + showTime);
        animationState.state = AnimationState.FADEOUT;
    }

    public void HideImmediate()
    {
        animationState.OnScreenTime = 0f;
        animationState.AlphaValue = 0f;
        animationState.FadeInProgress = false;
        animationState.state = AnimationState.IDLE;

        transparencySetter.AlphaValue = 0f;
    }

    public void Update(float deltaTime)
    {
        if (animationState.FadeInProgress)
            animationState.OnScreenTime += deltaTime;

        if (animationState.FadeInProgress)// && animationState.OnScreenTime <= fadeInTime + showTime + fadeOutTime)
        {
            if (animationState.OnScreenTime <= fadeInTime)
            {
                animationState.AlphaValue = Mathf.Max(
                    Mathf.Clamp01(animationState.OnScreenTime / fadeInTime),
                    animationState.AlphaValue
                );
                animationState.state = AnimationState.FADEIN;
            }
            else if (animationState.OnScreenTime <= fadeInTime + showTime)
            {
                if (!autoHide) animationState.FadeInProgress = false;
                animationState.state = AnimationState.SHOW;
            }
            else if (animationState.OnScreenTime <= fadeInTime + showTime + fadeOutTime)
            {
                animationState.AlphaValue = 1 - Mathf.Clamp01((animationState.OnScreenTime - showTime - fadeInTime) / fadeOutTime);
                animationState.state = AnimationState.FADEOUT;
            }
            else
            {
                HideImmediate();
                EventManager.OnAlphaManagerComplete.Invoke(managerName);
                return;
            }
            
            transparencySetter.AlphaValue = animationState.AlphaValue;
        }

        transparencySetter.AlphaValue = animationState.AlphaValue;
    }
}