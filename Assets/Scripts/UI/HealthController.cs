using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Slider), typeof(RectTransform))]
[ExecuteAlways]
public class HealthController : MonoBehaviour
{
    private static HealthController instance;

    [Header("Size Delta")]

    [Header("Bar Pixel Size")]
    [SerializeField] private float m_maxBar;
    [SerializeField] private float m_curBar;

    private Slider m_slider;
    private RectTransform m_rt;

    public TMP_Text value;

    public static HealthController GetInstance()
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
        string current_hp = m_curBar % 1 == 0 ? m_curBar.ToString("F0") : m_curBar.ToString("F1");
        value.text = current_hp + "/" + m_maxBar.ToString();
    }

    public void SetMax(float value)
    {
        m_maxBar = value;
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

    public void SubValue(float value)
    {
        AddValue(-value);
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
