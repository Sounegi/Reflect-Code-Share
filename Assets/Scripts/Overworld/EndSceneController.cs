using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndSceneController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI m_title;
    [SerializeField] private TextMeshProUGUI m_message;
    [SerializeField] private TextMeshProUGUI m_points;
    [SerializeField] private Slider m_starBar;

    void Start()
    {
        if (GameManager.Instance.IsWon) {
            m_title.text = "You Won!";
            m_message.text = "Congratulations!";
        } else {
            m_title.text = "You Lose!";
            m_message.text = "Try Again...";
        }
        m_points.text = GameManager.Instance.Points.ToString();
        m_starBar.value = GameManager.Instance.Stars;
        m_starBar.maxValue = 3;
        m_starBar.minValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
