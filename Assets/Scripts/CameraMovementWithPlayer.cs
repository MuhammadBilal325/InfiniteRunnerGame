using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementWithPlayer : MonoBehaviour {
    [SerializeField] private Transform Player;
    [SerializeField] private float safeDistance = 0;
    private float xOffset = 0;
    private float validMaxPlayerPos;
    private float validMinPlayerPos;
    private float adjustment = 0f;
    private void Start() {
        xOffset = Player.position.x - transform.position.x;
    }

    private void FixedUpdate() {
        validMaxPlayerPos = transform.position.x + xOffset + safeDistance;
        validMinPlayerPos = transform.position.x + xOffset - safeDistance;
        if (Player.transform.position.x > validMaxPlayerPos) {
            adjustment = Player.transform.position.x - validMaxPlayerPos;
        }
        else if (Player.transform.position.x < validMinPlayerPos) {
            adjustment = Player.transform.position.x - validMinPlayerPos;
        }
        else
            adjustment = 0;
        if (adjustment != 0)
            transform.position = new Vector3(transform.position.x + adjustment, transform.position.y, transform.position.z);
    }
}
