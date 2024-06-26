using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class EnemyAttackSO : ScriptableObject {
    public Transform prefab;
    public float damage;
    public Sprite Sprite;
    public string attackName;
}
