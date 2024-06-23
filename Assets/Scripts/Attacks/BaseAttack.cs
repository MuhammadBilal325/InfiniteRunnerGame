using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttack : MonoBehaviour {

    [SerializeField] protected float damage;
    [SerializeField] protected AttackSO attackSO;
    public virtual void Hit(IHittable hittable) {
        Debug.LogError("BaseAttackHit");
    }
    public float GetDamage() {
        return damage;
    }
}
