using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Mission : MonoBehaviour
{
    public TextMeshProUGUI text;

    void OnEnable()
    {
        text = gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText(string value)
    {
        if (text != null)
        {
            text.text = value;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component is null. Make sure it's assigned or present on the first child.");
        }
    }
}
