using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class IKBodyVisual : MonoBehaviour {
    [SerializeField] private Transform[] IKTargetsLeft;
    [SerializeField] private Transform[] IKTargetsRight;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float bobbingSpeed;
    [SerializeField] private float bobbingAmplitude;
    [SerializeField] private float rotateMultiplier;
    [SerializeField] private float rotateBlend;
    [SerializeField] private float backwardsDampen;
    private Vector3 newPosition;
    private Vector3 originalRotation;
    private float lerp = 0f;
    private float averageMovement;

    private void Start() {
        originalRotation = transform.localEulerAngles;
        averageMovement = GameInput.Instance.GetMovementInput();
    }
    private void Update() {
        if (lerp < 1f) {
            newPosition = transform.localPosition + offset;
            newPosition.y = (Mathf.Sin(lerp * Mathf.PI) * bobbingAmplitude) + offset.y;
            lerp += Time.deltaTime * bobbingSpeed;
        }
        else {
            lerp = 0f;
        }
        transform.localPosition = newPosition;
        HandleRotation();
    }

    private void HandleRotation() {
        //Keep a weighted movement vector that determines the tilt direction of the body;
        int movement = (int)GameInput.Instance.GetMovementInput();
        float blend = rotateBlend;
        if (movement < 0) {
            if (backwardsDampen != 0)
                blend = rotateBlend / backwardsDampen;
        }
        averageMovement = Mathf.Lerp(averageMovement, GameInput.Instance.GetMovementInput(), Time.deltaTime * blend);
        float rotation = averageMovement * rotateMultiplier;
        transform.localEulerAngles = originalRotation;
        transform.Rotate(rotation, 0, 0);
    }

}
