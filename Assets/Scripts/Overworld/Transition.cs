using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{
    #region Class Variable
    public enum TransitionType
    {
        Fade,
        Circular,
    }

    private static Transition instance;

    [Header("Transition Details")]

    [SerializeField] private bool doOnStart;
    [SerializeField] private float m_transitionDuration = 1f;
    public float transitionDuration
    {
        get { return m_transitionDuration; }
        set { m_transitionDuration = value; }
    }
    [SerializeField] private float m_transitionDelay = 1f;
    public float transitionDelay
    {
        get { return m_transitionDelay; }
        set { m_transitionDelay = value; }
    }
    [SerializeField] private TransitionType m_transitionType = TransitionType.Fade;
    public TransitionType transitionType
    {
        get { return m_transitionType; }
        set { m_transitionType = value; }
    }

    [SerializeField] private string m_textTitle = "";
    public string textTitle
    {
        get { return m_textTitle; }
        set { m_textTitle = value; if (m_title1 != null) m_title1.text = value; if (m_title2 != null) m_title2.text = value; }
    }

    private Image m_image;
    private Text m_title1;
    private Text m_title2;

    private Color[] currentColor
    {
        get {
            return new Color[] {
                m_image.color,
                m_title1.color,
                m_title2.color
            };
        }
        set {
            m_image.color = value[0];
            m_title1.color = value[1];
            m_title2.color = value[2];
        }
    }

    private Color[] originalColor;

    #endregion

    public static Transition GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Initialize();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (doOnStart) Reset();
        else SetColorTo(Color.clear);
    }


    #region Public Method
    public void StartTransition(TransitionType type, Color? to, bool isExit, float duration, float delay = 0f, System.Action callback = null)
    {
        StartCoroutine(DoTransition(type, to, isExit, duration, delay, callback));
    }

    public void Enter(System.Action callback = null)
    {
        textTitle = m_textTitle;
        StartTransition(m_transitionType, Color.clear, false, m_transitionDuration, m_transitionDelay, callback);
    }

    public void Exit(System.Action callback = null)
    {
        textTitle = "";
        if(BGMManager.GetInstance() != null)
        {
            BGMManager.GetInstance().FadeOutMusic();
        }
        StartTransition(m_transitionType, Color.black, true, m_transitionDuration, m_transitionDelay, callback);
    }
    #endregion

    #region Private Method
    IEnumerator Circular(Color[] from, Color[] to, float duration, bool isExit)
    {
        m_image.fillMethod = Image.FillMethod.Radial360;
        m_image.fillOrigin = 2;

        Color[] temp = new Color[3];
        temp = (Color[]) from.Clone();

        float lerp_start, lerp_end;
        lerp_start = (isExit) ? 0f : 1f;
        lerp_end = (isExit) ? 1f : 0f;

        float elapsedTime = 0.0f;
        float t;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            t = elapsedTime / duration;
            for (int i = 1; i < 3; i++) // start form 1 to skip image part
            {
                temp[i] = Color.Lerp(from[i], to[i], t);
            }

            // Update Colour
            SetColorTo(temp);

            // Update Circular
            m_image.fillAmount = Mathf.Lerp(lerp_start, lerp_end, t);

            yield return null;
        }
    }

    IEnumerator Fade(Color[] from, Color[] to, float duration, bool isExit)
    {
        Color[] temp = new Color[3];
        temp = (Color[]) from.Clone();

        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            for (int i = 0; i < 3; i++)
            {
                temp[i] = Color.Lerp(from[i], to[i], elapsedTime / duration);
            }
            // Update Colour
            SetColorTo(temp);

            yield return null;
        }
    }

    IEnumerator DoTransition(TransitionType type, Color? to, bool isExit, float duration, float delay, System.Action callback = null)
    {
        // Pre-transition
        yield return new WaitForSeconds(delay);

        // Setup variable
        Color[] froms = new Color[3];
        froms = currentColor;
        Color[] tos;

        if (to == null){
            tos = (Color[])originalColor.Clone();
        }else{
            Color colorTemp = to ?? Color.clear;
            tos = (Color[])new[] { colorTemp, colorTemp, colorTemp };
        }

        switch (type)
        {
            case TransitionType.Circular:
                // Debug.Log("Circular");
                yield return Circular(froms, tos, duration, isExit);
                break;
            case TransitionType.Fade:
                // Debug.Log("Fade");
                yield return Fade(froms, tos, duration, isExit);
                break;
            default:
                // Debug.LogWarning("Transition type not found. Do fade transition instead.");
                yield return Fade(froms, tos, duration, isExit);
                break;
        }

        // Post-transition
        m_title1.text = "";
        m_title2.text = "";
        callback?.Invoke();
        
        yield return null;
    }

    private void Reset()
    {
        ResetColor();
        Enter();
    }
    
    public void SetTitle(string s)
    {
        textTitle = s;
    }

    private void ResetColor()
    {
        SetColorTo(originalColor);
    }

    private void SetColorTo(Color[] to)
    {
        if (to.Length == currentColor.Length)
            currentColor = to;
        else
            Debug.LogWarning("Color is not assigned, since the assigned length does not match.");
    }

    private void SetColorTo(Color to)
    {
        Color[] tos = (Color[])new[] { to, to, to };
        currentColor = tos;
    }

    /// <summary>
    /// Initialize any necessary object and the value.
    /// </summary>
    private void Initialize()
    {
        m_image = GetComponent<Image>();
        m_title1 = transform.GetChild(0).GetComponent<Text>();
        m_title2 = transform.GetChild(1).GetComponent<Text>();

        originalColor = new Color[3];
        originalColor = (Color[]) currentColor.Clone();

        m_image.color = Color.black;

        SetTitle(m_textTitle);
    }

    #endregion

}
