using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IHittable {
    // Start is called before the first frame update

    public void OnHit(BaseAttack attack) {
        Debug.Log("Obstacle " + this.gameObject.name + " Hit by " + attack.GetName());
    }
}
