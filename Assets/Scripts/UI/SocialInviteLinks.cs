using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class SocialInviteLinks : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void OpenNewTab(string url);

    public void OpenIt(string url)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        OpenNewTab(url);
#else
        Application.OpenURL(url);
#endif
    }

    public void DiscordInvite()
    {
        OpenIt("https://discordapp.com/invite/mfD9285");
    }

    public void VKInviteLink()
    {
        OpenIt("https://vk.com/cozystorm");
    }
}
