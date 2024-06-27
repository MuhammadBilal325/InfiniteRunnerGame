using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IHittable {
    // Start is called before the first frame update
    public event EventHandler OnHitEvent;
    public void OnHit(BaseAttack attack) {
        OnHitEvent?.Invoke(this, EventArgs.Empty);
    }
    public void OnHit() {
    }

    public float getHealth() {
        return 10000;
    }
    public float getMaxHealth() {
        return 10000;
    }
}
