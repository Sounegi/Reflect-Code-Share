using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    public void ExitControlScene()
    {
        LevelManager.GetInstance().ExitControlScene();
    }
}
