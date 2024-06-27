using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbShot : BaseEnemyAttack {
    [SerializeField] private float orbSpeed = 10f;
    [SerializeField] private float range = 10f;
    private Vector3 instancePoint;
    private void Start() {
        instancePoint = transform.position;
    }

    // Update is called once per frame
    private void Update() {
        transform.Translate(Vector3.right * Time.deltaTime * orbSpeed);
        if (Vector3.Distance(instancePoint, transform.position) > range) {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.GetComponent<Player>() != null) {
            Hit(collision.gameObject.GetComponent<Player>());
        }
        else if (collision.gameObject.GetComponent<IHittable>() != null && collision.gameObject.CompareTag(Tags.OBSTACLE_TAG)) {
            Hit(collision.gameObject.GetComponent<IHittable>());
        }
    }

    public override void Hit(IHittable hittable) {
        hittable.OnHit();
        Destroy(gameObject);
    }

    public override void Hit(Player player) {
        player.OnHit(this);
    }


}
