using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject failScreen;

    public enum UIScreen
    {
        FailScreen
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
}
