using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextLocalization : MonoBehaviour
{
    public TextMeshPro textMeshPro;
    public TextMeshProUGUI textMeshProUGUI;
    public int overrideID = -1;
    private string savedENText;
    public bool onlyFont = false;

    private void Awake()
    {
        LocalizationManager.OnLanguageSwich.AddListener(TextUpdate);
        if (!textMeshPro) textMeshPro = GetComponent<TextMeshPro>();
        if (!textMeshProUGUI) textMeshProUGUI = GetComponent<TextMeshProUGUI>();

        if (!textMeshPro && !textMeshProUGUI) Debug.LogError("can't get TextMeshPro component");
        if (textMeshPro) savedENText = textMeshPro.text;
        if (textMeshProUGUI) savedENText = textMeshProUGUI.text;
        TextUpdate();
    }

    private void TextUpdate()
    {
        if (LocalizationManager.currentLocale != LocalizationManager.Locale.EN)
        {
            if (!onlyFont)
                if (overrideID == -1)
                {
                    if (textMeshPro)
                        textMeshPro.text = LocalizationManager.GetTranlationENtoCurrent(textMeshPro.text);
                    if (textMeshProUGUI)
                        textMeshProUGUI.text = LocalizationManager.GetTranlationENtoCurrent(textMeshProUGUI.text);
                }
                else
                {
                    if (textMeshPro)
                        textMeshPro.text = LocalizationManager.GetTranlationByID(overrideID);
                    if (textMeshProUGUI)
                        textMeshProUGUI.text = LocalizationManager.GetTranlationByID(overrideID);
                }

            if (LocalizationManager.currentLocale == LocalizationManager.Locale.RU && LocalizationManager.ruFont)
            {
                if (textMeshPro)
                    textMeshPro.font = LocalizationManager.ruFont;
                if (textMeshProUGUI)
                    textMeshProUGUI.font = LocalizationManager.ruFont;
            }
        }
        else
        {
            if (textMeshPro)  textMeshPro.text = savedENText;
            if (textMeshProUGUI) textMeshProUGUI.text = savedENText;
        }
    }

}
