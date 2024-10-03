using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class SelectPanelBehaviour : MonoBehaviour
{
    #region Class Variable
    int m_upperLimit;
    int m_lowerLimit;
    [SerializeField] int m_size;
    public int Size { get { return m_size; } private set { m_size = value; } }
    [SerializeField] int m_selectedIdx;

    [SerializeField] private GameObject[] m_selectionObjects;
    private string[] UpgradeDescription;
    private Button[] m_selectionButtons;
    private Image[] m_selectionImages;
    private TextMeshProUGUI[] m_titleTexts;
    private TextMeshProUGUI[] m_detailsTexts;

    [SerializeField] private GameObject m_confirmObjects;
    private bool m_isHasConfirmButton;
    public bool IsHasConfirmButton { get { return m_isHasConfirmButton; } private set { m_isHasConfirmButton = value; } }

    private Button m_confirmButton;
    private TextMeshProUGUI m_tmpText;
    public string StringTmpText
    {
        get
        {
            if (m_tmpText != null) return m_tmpText.text;
            else { Debug.LogWarning("Text is not available, since object is not assigned. Returned empty string."); return ""; }
        }
        set
        {
            if (m_tmpText != null) m_tmpText.text = value;
            else Debug.LogWarning("Text cannot be changed, since object is not assigned");
        }
    }
    #endregion

    #region Input & System Variable
    private InputAction m_upArrowAction;
    private InputAction m_downArrowAction;
    private InputAction m_enterAction;

    private System.Action<int> onConfirmed;
    private System.Action<int> onSelectionChange;
    #endregion

    #region Routine Function
    public void Awake()
    {
        GetComponents();
        if (m_selectionButtons.Length == 0)
        {
            Debug.LogError("No buttons to be select");
        }
        if (!m_isHasConfirmButton)
        {
            Debug.LogWarning("No buttons to confirm selection, change method by pressing enter and direct select.");
        }
        SetupSelection();
        SetupConfirmation();
        
        // TODO need to be set for what purpose of selection is this...
        onSelectionChange += (value) => LevelManager.GetInstance().SetLevel(value);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Setup Button's Appaearance overall (sprite, title, detail/description, and its callback)
    /// </summary>
    /// <param name="idx"></param> which button wants to be edited (START AT 0 INDEX).
    /// <param name="image"></param>
    /// <param name="title"></param>
    /// <param name="details"></param>
    /// <param name="callback"></param>
    public void SetupButtonAt(int idx, Sprite image = null, string title = null, string details = null, System.Action callback = null)
    {
        // TODO: SFX, show DETIALS by clicking the item (reshape TMP and use it as the container) do these procedural on the callback.
        // Alternatively using the animation to show and hide if selected.
        
        if (callback != null)
        {
            UnityAction unityAction = new UnityAction(callback);
            m_selectionButtons[idx].onClick.AddListener(unityAction);
        }
        if (title != null)
        {
            m_titleTexts[idx].text = title;
        }
        if (details != null)
        {
            m_detailsTexts[idx].text = details;
        }
        if (image != null)
        {
            m_selectionImages[idx].sprite = image;
        }
    }


    public void SelectLevel(int level)
    {
        Debug.Log("Selecting level " + level);
        if (m_selectedIdx >= 0) m_selectionButtons[m_selectedIdx].OnPointerExit(null);
        m_selectedIdx = level;
        m_selectionButtons[m_selectedIdx].Select();

        if (onSelectionChange != null) onSelectionChange?.Invoke(m_selectedIdx);
        
        StringTmpText = "" + (m_selectedIdx + 1);

        if (m_isHasConfirmButton) m_confirmButton.interactable = true;
    }

    public void SetupOnSelectionChange(System.Action<int> callback)
    {
        onSelectionChange = callback;
    }

    public void SetupOnConfirmation(System.Action<int> callback)
    {
        onConfirmed = callback;
    }
    #endregion

    #region Internal Functions
    void GetComponents()
    {
        // Selction Button
        int len = m_selectionObjects.Length;
        m_selectionButtons = new Button[len];
        m_selectionImages = new Image[len];
        m_titleTexts = new TextMeshProUGUI[len];
        m_detailsTexts = new TextMeshProUGUI[len];
        UpgradeDescription = new string[len];

        for (int i = 0; i < len; i++)
        {
            // m_selectionButtons[i] = m_selectionObjects[i].GetComponent<Button>();
            // m_selectionImages[i] = m_selectionObjects[i].GetComponentInChildren<Image>();
            bool isFoundButton = false;
            foreach ( Transform g in  m_selectionObjects[i].GetComponentsInChildren<Transform>())
            {
                switch (g.name)
                {
                    case "Image":
                        m_selectionImages[i] = g.transform.GetComponent<Image>();
                        break;
                    case "Title":
                        m_titleTexts[i] = g.transform.GetComponent<TextMeshProUGUI>();
                        break;
                    case "Details":
                        m_detailsTexts[i] = g.transform.GetComponent<TextMeshProUGUI>();
                        break;
                    case "TempUpgradeTrigger":
                        Debug.Log("jackpot!");
                        break;
                    default:
                        if (g.transform.GetComponent<Button>() != null) { 
                            if (isFoundButton)
                            {
                                Debug.LogWarning("Object with name " + g.name + "is selected to be the button");
                            }
                            m_selectionButtons[i] = g.transform.GetComponent<Button>();
                            isFoundButton = true;
                            //m_selectionButtons[i].GetComponent<TempUpgradeTrigger>().enabled = true;
                            //UpgradeDescription[i] = m_selectionButtons[i].GetComponent<TempUpgradeTrigger>().GetUpgradeDesc();
                            //Debug.Log("upgrade description is " + UpgradeDescription[i]);
                        }
                        break;
                }
            }

            if (!isFoundButton)
            {
                Debug.LogError("Object with name missing Button components");
            }
        }

        // Setup abal-abal
        for (int i = 0; i < len; i++)
        {
            m_titleTexts[i].text = "Item-" + (i + 1);
            m_detailsTexts[i].text = "Details for item number " + (i + 1);
            // m_selectionImages[i].sprite = ...
        }


        // Confirmation Button
        m_isHasConfirmButton = (m_confirmObjects != null);

        if (m_isHasConfirmButton)
        {
            m_confirmButton = m_confirmObjects.GetComponent<Button>();
            m_tmpText = m_confirmObjects.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    void SetupConfirmation()
    {
        m_enterAction = new InputAction(binding: "<Keyboard>/enter");
        m_enterAction.performed += _ => {
            if (m_isHasConfirmButton)
                if (m_confirmButton.interactable) onConfirmed(m_selectedIdx);
            else if (m_selectedIdx != 0) onConfirmed(m_selectedIdx);
        };

        if (m_isHasConfirmButton)
        {
            m_confirmButton.onClick.AddListener(() => onConfirmed(m_selectedIdx));
            m_confirmButton.interactable = false;
        }
    }

    void SetupSelection()
    {
        m_upArrowAction = new InputAction(binding: "<Keyboard>/upArrow");
        m_downArrowAction = new InputAction(binding: "<Keyboard>/downArrow");

        m_upArrowAction.performed += _ => SelectLevel(Mathf.Min(m_selectedIdx + 1, m_upperLimit));
        m_downArrowAction.performed += _ => SelectLevel(Mathf.Max(m_selectedIdx - 1, m_lowerLimit));

        int i = 0;
        foreach (Button btns in m_selectionButtons)
        {
            int x = i;
            if (i < m_size)
            {
                btns.onClick.AddListener(() => SelectLevel(x));
                i++;
            }
            else
            {
                btns.interactable = false;
            }
        }

        m_upperLimit = (m_size < i) ? m_size - 1 : i - 1;
        m_lowerLimit = 0;

        m_selectedIdx = -1;

        StringTmpText = "-";
    }

    void OnEnable()
    {
        m_upArrowAction.Enable();
        m_downArrowAction.Enable();
        m_enterAction.Enable();
    }

    void OnDisable()
    {
        m_upArrowAction.Disable();
        m_downArrowAction.Disable();
        m_enterAction.Disable();
    }
    #endregion

}
