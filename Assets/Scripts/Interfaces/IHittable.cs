using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable {

    public event EventHandler OnHitEvent;
    void OnHit(BaseAttack attack);

    public float getHealth();
    public float getMaxHealth();
}
