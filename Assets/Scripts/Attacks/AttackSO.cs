using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AttackSO : ScriptableObject {
    public Transform prefab;
    public float damage;
    public Sprite Sprite;
    public string attackName;
    public bool hasHitPause = false;
}
