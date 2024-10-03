using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInstance : MonoBehaviour
{
    private static CameraInstance instance;

    private Camera cam;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static CameraInstance GetInstance()
    {
        return instance;
    }

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    public Camera GetCamera()
    {
        return cam;
    }
}
