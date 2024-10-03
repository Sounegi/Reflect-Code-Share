using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningManager : MonoBehaviour
{
    public void ToMainMenu()
    {
        if(Transition.GetInstance() != null)
        {
            Transition.GetInstance().transitionDelay = 0.5f;
            Transition.GetInstance().transitionDuration = 1.5f;
            Transition.GetInstance().Exit(() => SceneManager.LoadScene("MainMenu"));
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }   
    }
}
