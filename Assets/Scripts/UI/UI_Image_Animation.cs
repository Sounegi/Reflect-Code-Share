using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UI_Image_Animation : MonoBehaviour
{
    public Image m_Image;

    public Sprite[] m_SpriteArray;
    private float m_Speed = 0.005f;

    private int m_IndexSprite;
    bool IsDone;
    public void Func_PlayUIAnim()
    {
        IsDone = false;
        StartCoroutine(CoroutineFunc_PlayAnimUI());
    }

    public void Func_StopUIAnim()
    {
        IsDone = true;
        StopCoroutine(CoroutineFunc_PlayAnimUI());
    }
    IEnumerator CoroutineFunc_PlayAnimUI()
    {
        yield return new WaitForSeconds(m_Speed);
        if (m_IndexSprite >= m_SpriteArray.Length)
        {
            m_IndexSprite = 0;
        }
        m_Image.sprite = m_SpriteArray[m_IndexSprite];
        m_IndexSprite += 1;
        if (IsDone == false)
            StartCoroutine(CoroutineFunc_PlayAnimUI());
    }

    private void OnEnable()
    {
        Func_PlayUIAnim();
    }
}
