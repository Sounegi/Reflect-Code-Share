using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour         //Used for an effect that's supposed to linger after parent's death
{
    public float timeout;
    [SerializeField] AudioClip effectSound;

    void Start() {
        Destroy(gameObject, timeout);
        GetComponent<AudioSource>().PlayOneShot(effectSound);
    }
}
