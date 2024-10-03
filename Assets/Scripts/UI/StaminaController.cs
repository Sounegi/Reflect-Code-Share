using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Slider), typeof(RectTransform))]
[ExecuteAlways]
public class StaminaController : MonoBehaviour
{
    private static StaminaController instance;

    [Header("Bar Pixel Size")]
    [SerializeField] private float m_maxBar;
    [SerializeField] private float m_curBar;

    private Slider m_slider;
    private RectTransform m_rt;

    public TMP_Text value;

    public static StaminaController GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            m_slider = GetComponent<Slider>();
            m_rt = GetComponent<RectTransform>();
            
            m_slider.minValue = 0;
            m_slider.wholeNumbers = true;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }      
    }

    void Update()
    {
        value.text = m_curBar.ToString() + "/" + m_maxBar.ToString();
    }

    public void SetMax(float value)
    {
        m_maxBar = value;
        // float _width = m_minBarSize + m_perBarSize * (m_maxBar/10 - 1);
        // m_rt.sizeDelta = new Vector2(_width, m_rt.sizeDelta.y);
        m_slider.maxValue = m_maxBar;
    }
    
    public void AddValue(float value)
    {
        m_curBar += value;
        if (m_curBar > m_maxBar)
        {
            m_curBar = m_maxBar;
        }
        else if (m_curBar < 0)
        {
            m_curBar = 0;
        }
        m_slider.value = m_curBar;
    }
    
    public void SetValue(float value)
    {
        m_curBar = value;
        if (m_curBar > m_maxBar)
        {
            m_curBar = m_maxBar;
        }
        else if (m_curBar < 0)
        {
            m_curBar = 0;
        }
        m_slider.value = m_curBar;
    }
}
