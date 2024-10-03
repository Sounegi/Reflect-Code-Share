using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionSettings : MonoBehaviour
{
    // Start is called before the first frame update
    private Slider VolumeSlider;
    private bool mute;
    void Start()
    {
        mute = PlayerPrefs.GetFloat("Mute") == 0f ? false : true;
        VolumeSlider = GetComponentInChildren<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void MuteVolume()
    {
        //NEED TO IMPLEMENT SOUND MANAGER
        //TOGGLE MUTE HERE
        //    MusicManager.GetInstance().ToggleMute();
        //
        if (mute)
        {
            BGMManager.instance.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("CurrVolume");
        }
        else
        {
            BGMManager.instance.GetComponent<AudioSource>().volume = 0f;
        }
        
    }

    public void ChangeVolume()
    {
        //NEED TO IMPLEMENT SOUND MANAGER
        //TOGGLE MUTE HERE
        //    MusicManager.GetInstance().SetVolume(newvolume);
        //
        BGMManager.instance.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("CurrVolume");
    }

    public void ExitOptionScene()
    {
        LevelManager.GetInstance().ExitOptionScene();
    }
}
