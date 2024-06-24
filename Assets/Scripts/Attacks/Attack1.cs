using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack1 : BaseAttack {
    // Start is called before the first frame update
    [SerializeField] private float speed;
    [SerializeField] private float range;
    private float addSpeed = 1;
    Vector3 instancePoint;
    private void Awake() {
        instancePoint = transform.position;
        int movement = (int)GameInput.Instance.GetMovementInput();
        if (movement == 1) {
            addSpeed = 2;
        }
        //Shake Camera on Spawn
        CameraMovementWithPlayer.Instance.ShakeCamera(0.1f, 0.1f);
    }

    // Update is called once per frame
    private void FixedUpdate() {
        transform.Translate(Vector3.right * Time.deltaTime * speed * addSpeed);
        if (transform.position.x - instancePoint.x > range * addSpeed) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.GetComponent<IHittable>() != null) {
            Hit(collision.gameObject.GetComponent<IHittable>());
        }
    }

    public override void Hit(IHittable hittable) {
        hittable.OnHit(this);
        Destroy(gameObject);
    }
}
