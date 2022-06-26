using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject failScreen;
    [SerializeField] private TextMeshProUGUI linesClearedText;
    [SerializeField] private TextMeshProUGUI speedLevelText;

    public enum UIScreen
    {
        FailScreen
    }

    public enum UIText
    {
        LinesCleared,
        SpeedLevel
    }

    private GameObject GetUiScreenObj(UIScreen uiScreen)
    {
        GameObject uiScreenObj = null;
        switch (uiScreen)
        {
            case UIScreen.FailScreen:
                uiScreenObj = failScreen;
                break;
        }
        Debug.Assert(uiScreenObj != null, "Attempted to trigger UI Screen that does not exist");
        return uiScreenObj;
    }

    public void ToggleUiScreen(UIScreen uiScreen, bool isActive)
    {
        GameObject uiScreenObj = GetUiScreenObj(uiScreen);
        uiScreenObj.SetActive(isActive);
    }

    public void UpdateGameUiText(int linesCleared, int speedLevel)
    {
        linesClearedText.text = "Lines Cleared: " + linesCleared;
        speedLevelText.text = "Speed Level: " + speedLevel;
    }
}
