using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack2 : BaseAttack {
    Vector3 spawnOffset;
    private void Awake() {
        spawnOffset = transform.position - Player.Instance.transform.position;
        StartCoroutine(DestroyAttack());
    }

    private IEnumerator DestroyAttack() {
        yield return new WaitForSeconds(0.1f);
        Destroy(this.gameObject);
    }
    // Update is called once per frame
    private void FixedUpdate() {
        transform.position = Player.Instance.transform.position + spawnOffset;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.GetComponent<IHittable>() != null) {
            Hit(collision.gameObject.GetComponent<IHittable>());
        }
    }

    public override void Hit(IHittable hittable) {
        hittable.OnHit(this);
        CameraMovementWithPlayer.Instance.ShakeCamera(0.1f, 0.1f);
        Destroy(gameObject);
    }
}
