using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DisintegrationShield : BaseAbillity
{
    // Start is called before the first frame update
    public void CreateField(GameObject prefab, PlayerControlScript mainController)
    {
        GameObject holder = Instantiate(prefab, mainController.transform.position, Quaternion.identity);
        Endure endureScript = holder.GetComponent<Endure>();
        endureScript.SetProperties(mainController.DisintegrationDuration);
        holder.transform.SetParent(mainController.transform);
    }
}
