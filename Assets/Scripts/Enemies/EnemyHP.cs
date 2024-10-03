using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour
{
    [SerializeField] Slider slider;

    public void UpdateHealth(float maxHP, float curHP)
    {
        slider.value = curHP / maxHP;
    }
}
