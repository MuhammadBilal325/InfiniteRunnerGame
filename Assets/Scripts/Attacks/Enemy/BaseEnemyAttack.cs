using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyAttack : MonoBehaviour {
    [SerializeField] protected EnemyAttackSO attackSO;
    public virtual void Hit(Player player) {
        Debug.LogError("BaseAttackHit");
    }
    public virtual void Hit(IHittable hittable) {
        Debug.LogError("BaseAttackHit");
    }
    public float GetDamage() {
        return attackSO.damage;
    }

    public EnemyAttackSO GetAttackSO() {
        return attackSO;
    }

    public string GetName() {
        return attackSO.attackName;
    }
}
