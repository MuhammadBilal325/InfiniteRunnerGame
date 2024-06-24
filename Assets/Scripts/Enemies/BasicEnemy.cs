using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour, IHittable {

    public event EventHandler OnHitEvent;
    [SerializeField] private float health = 100;
    private float maxHealth;

    private void Awake() {
        maxHealth = health;
    }

    public void OnHit(BaseAttack attack) {
        health -= attack.GetDamage();
        OnHitEvent?.Invoke(this, EventArgs.Empty);
        if (health <= 0) {
            Destroy(gameObject);
        }
    }
    public float getHealth() {
        return health;
    }

    public float getMaxHealth() {
        return maxHealth;
    }
}
