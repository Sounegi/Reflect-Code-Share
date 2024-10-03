using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSettingsBehaviour : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Toggle _toggle;

    private void Awake()
    {
        // Initalize by player prefs
        _toggle.isOn = PlayerPrefs.GetFloat("Mute") == 0f? true:false;
        _slider.value = PlayerPrefs.GetFloat("CurrVolume");
        _slider.interactable = _toggle.isOn;

        // Add listener after initializing toggle and slider
        _toggle.onValueChanged.AddListener(delegate {ToggleSliderInteractable(); });
        _slider.onValueChanged.AddListener(delegate {ValueChangeCheck(); });
    }

    private void ToggleSliderInteractable(){
        // Update Slider UI
        _slider.interactable = _toggle.isOn;
        if (_toggle.isOn) {
            // Load TempVolume to Slider
            _slider.value = PlayerPrefs.GetFloat("TempVolume");
        }else{
            // Save Volume to TempVolume
            PlayerPrefs.SetFloat("TempVolume", _slider.value);
            _slider.value = 0;
        }

        // Update Player Prefs Volume and Mute
        PlayerPrefs.SetFloat("CurrVolume", _slider.value);
        PlayerPrefs.SetFloat("Mute", _toggle.isOn? 0f:1f);
    }

    private void ValueChangeCheck()
    {
        // Update Player Prefs Volume
        PlayerPrefs.SetFloat("CurrVolume", _slider.value);
    }


}
