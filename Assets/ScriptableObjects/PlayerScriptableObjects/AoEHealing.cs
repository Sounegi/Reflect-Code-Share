using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AoEHealing : BaseAbillity
{
    // Start is called before the first frame update
    public void CreateField(GameObject prefab, PlayerControlScript mainController)
    {
        GameObject holder = Instantiate(prefab, mainController.transform.position, Quaternion.identity);
        HealingCircleLogic healscript = holder.GetComponent<HealingCircleLogic>();
        healscript.SetProperties(mainController.aoeHealTotal, mainController.aoeHealTime, mainController.aoeHealRadius);
    }
}
