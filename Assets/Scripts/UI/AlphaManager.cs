using System;
using UnityEngine;

[Serializable]
public class AlphaManager
{
    [SerializeField]
    private TransparencySetterUI transparencySetter;
    [SerializeField]
    private float fadeInTime = 0f;
    [SerializeField]
    private float showTime = 2f;
    [SerializeField]
    private float fadeOutTime = 0f;

    private AnimationState state;

    private class AnimationState
    {
        public float OnScreenTime = 0f;
        public float AlphaValue = 0f;
        public bool FadeInProgress = false;
    }

    public AlphaManager()
    {
        state = new AnimationState();
    }

    public void Show()
    {
        state.FadeInProgress = true;
    }

    public void Hide()
    {
        state.OnScreenTime = Mathf.Max(state.OnScreenTime, fadeInTime + showTime);
    }

    public void HideImmediate()
    {
        state.OnScreenTime = 0f;
        state.AlphaValue = 0f;
        state.FadeInProgress = false;

        transparencySetter.AlphaValue = 0f;
    }

    public void Update(float deltaTime)
    {
        if (state.FadeInProgress)
            state.OnScreenTime += deltaTime;

        if (state.FadeInProgress && state.OnScreenTime < fadeInTime + showTime + fadeOutTime + 0.1)
        {
            if (state.OnScreenTime < fadeInTime + showTime)
                state.AlphaValue = Mathf.Max(
                    Mathf.Clamp01(state.OnScreenTime / fadeInTime),
                    state.AlphaValue
                );
            else if (state.OnScreenTime >= fadeOutTime + showTime)
                state.AlphaValue = 1 - Mathf.Clamp01((state.OnScreenTime - showTime - fadeInTime) / fadeOutTime);

            transparencySetter.AlphaValue = state.AlphaValue;
        }
        else HideImmediate();

        transparencySetter.AlphaValue = state.AlphaValue;
    }
}