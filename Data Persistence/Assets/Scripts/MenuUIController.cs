using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    private void Start()
    {
        if (HighScoreManager.Instance)
        {
            nameField.text = HighScoreManager.Instance.GetCurrentHighScoreName();
        }
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
