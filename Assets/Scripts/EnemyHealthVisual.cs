using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthVisual : MonoBehaviour {
    [SerializeField] private Transform interfaceObject;
    private IHittable hittable;
    [SerializeField] private Image progressBar;
    void Start() {
        if (interfaceObject.TryGetComponent<IHittable>(out IHittable hit)) {
            this.hittable = hit;
        }
        else {
            hittable = null;
        }
        if (hittable != null)
            hittable.OnHitEvent += Hittable_OnHitEvent;
    }

    private void Hittable_OnHitEvent(object sender, System.EventArgs e) {
        progressBar.fillAmount = (float)hittable.getHealth() / hittable.getMaxHealth();
    }

}
