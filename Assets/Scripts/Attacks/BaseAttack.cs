using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttack : MonoBehaviour {
    [SerializeField] protected AttackSO attackSO;
    public virtual void Hit(IHittable hittable) {
        Debug.LogError("BaseAttackHit");
    }
    public float GetDamage() {
        return attackSO.damage;
    }

    public AttackSO GetAttackSO() {
        return attackSO;
    }

    public string GetName() {
        return attackSO.attackName;
    }
}
