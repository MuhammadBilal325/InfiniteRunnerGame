using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack1 : BaseAttack {
    // Start is called before the first frame update
    [SerializeField] private float speed;
    [SerializeField] private float range;
    Vector3 instancePoint;
    private void Awake() {
        instancePoint = transform.position;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        transform.Translate(Vector3.right * Time.deltaTime * speed);
        if (transform.position.x - instancePoint.x > range) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log("collided with " + collision.gameObject.name);
        if (collision.gameObject.GetComponent<IHittable>() != null) {
            Hit(collision.gameObject.GetComponent<IHittable>());
        }
    }

    public override void Hit(IHittable hittable) {
        Debug.Log("Attack1");
        hittable.OnHit(this);
        Destroy(gameObject);
    }
}
