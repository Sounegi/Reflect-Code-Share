using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Points System
    [SerializeField] int m_points;
    public int Points { get { return m_points; } private set { m_points = value; } }
    [SerializeField] int[] m_points_band = new int[3];
    public int Stars {
        get {
            int i = 0;
            for (i = 0; i < m_points_band.Length; i++)
                if (m_points < m_points_band[i]) break;
            return i;
        }
    }


    [SerializeField] bool m_isWon;
    public bool IsWon { get { return m_isWon; } set { m_isWon = value; } }

    private LevelManager LM;
    private EffectManager EM;

    public static GameManager Instance;
    void Awake(){
        if (Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        // Get Managers
        LM = GetComponent<LevelManager>();
        EM = GetComponent<EffectManager>();

        // Set for Sound Manager Player Prefs
        PlayerPrefs.SetFloat("CurrVolume", 0.5f);
        PlayerPrefs.SetFloat("TempVolume", 0.5f);
        PlayerPrefs.SetFloat("Mute", 0f);

    }
    void Start()
    {
        // TODO: Read ScriptedObject to get points band
        // m_points_band[0] = ;
        // m_points_band[1] = ;
        // m_points_band[2] = ;
    }
    

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(PlayerPrefs.GetFloat("CurrVolume"));
        // Debug.Log((PlayerPrefs.GetFloat("CurrVolume"))>0.0f);
    }
}
