using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour, IHittable {
    // Start is called before the first frame update
    public event EventHandler OnHitEvent;
    [SerializeField] private float health = 100;
    private float maxHealth;

    public virtual void Awake() {
        maxHealth = health;
    }
    public virtual void OnHit(BaseAttack attack) {
        health -= attack.GetDamage();
        OnHitEvent?.Invoke(this, EventArgs.Empty);
        if (health <= 0) {
            Destroy(gameObject);
        }
    }
    public virtual float getHealth() {
        return health;
    }

    public virtual float getMaxHealth() {
        return maxHealth;
    }


}
