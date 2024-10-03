using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Serialized Dictionary")]
public class EffectCollection : ScriptableObject
{
    [System.Serializable]
    struct pair {
        public string enemyName;
        public GameObject shootingEffect;
        public GameObject hurtingEffect;
        public GameObject dyingEffect;
    }

    [SerializeField] private List<pair> dictEntry;

    public Dictionary<string, Tuple<GameObject, GameObject, GameObject>> Dict() {
        Dictionary<string, Tuple<GameObject, GameObject, GameObject>> retdic = new Dictionary<string, Tuple<GameObject, GameObject, GameObject>>();
        foreach (pair entry in dictEntry) {
            retdic[entry.enemyName] = new Tuple<GameObject, GameObject, GameObject> (entry.shootingEffect, entry.hurtingEffect, entry.dyingEffect);
        }
        return retdic;
    }
}
