using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private static MainMenu instance;

    void Awake()
    {
        instance = this;
    }

    void OnEnable()
    {
        if (PlayerManager.GetInstance() != null)
        {
            Destroy(PlayerManager.GetInstance().gameObject);
        }

        if (PlayerControlScript.GetInstance() != null)
        {
            Destroy(PlayerControlScript.GetInstance().gameObject);
        }
    }

    public static MainMenu GetInstance()
    {
        return instance;
    }
    
    #region Intermediary
    public void LoadLevel(int level)
    {
        DisableOtherButtons();
        LevelManager.GetInstance().LoadScene(level);
    }

    public void testLoadUpgrade(int level)
    {
        DisableOtherButtons();
        LevelManager.GetInstance().LoadUpgradeScene();
    }

    public void LoadTutorialLevelScene()
    {
        LevelManager.GetInstance().LoadControlScene();
    }

    public void LoadOptionScene()
    {
        LevelManager.GetInstance().LoadOptionScene();
    }

    public void QuitGame()
    {
        LevelManager.GetInstance().QuitGame();
    }

    public void CloseGame()
    {
        Application.Quit();
    }
    #endregion

    private void DisableOtherButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            if (button != null)
            {
                button.interactable = false;
            }
        }
    }

    private void EnableOtherButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            if (button != null)
            {
                button.interactable = true;
            }
        }
    }
}
