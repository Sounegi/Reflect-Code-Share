using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ReflectShield : BaseAbillity
{
    // Start is called before the first frame update
    public void CreateField(GameObject prefab, PlayerControlScript mainController)
    {
        Debug.Log("Reflect Shield Created!");
        GameObject holder = Instantiate(prefab, mainController.transform.position, Quaternion.identity);
        TempShield shield_script = holder.GetComponent<TempShield>();
        shield_script.SetProperties(mainController.ReflectShieldHP);
        holder.transform.SetParent(mainController.transform);
    }

    // Update is called once per frame
}
