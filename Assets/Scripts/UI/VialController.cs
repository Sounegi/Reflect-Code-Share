using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider), typeof(RectTransform))]
[ExecuteAlways]
public class VialController : MonoBehaviour
{
    private static VialController instance;

    [Header("Variables")]
    [SerializeField] private float m_maxBar;
    [SerializeField] private float m_useBar;
    [SerializeField] private float m_curBar;

    private Slider m_slider;
    private RectTransform m_rt;

    public GameObject indicator;

    public static VialController GetInstance()
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

    void Start()
    {
        indicator.SetActive(false);
    }

    public void SetMax(float value)
    {
        m_maxBar = value;
        m_slider.maxValue = m_maxBar;
    }

    public void SetUse(float value)
    {
        m_useBar = value;
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

        if(m_curBar >= m_useBar)
        {
            indicator.SetActive(true);
        }
        else
        {
            indicator.SetActive(false);
        }

        m_slider.value = m_curBar;
    }
}