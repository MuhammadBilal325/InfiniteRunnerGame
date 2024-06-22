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
    private Vector3 newPosition;
    private Vector3 originalRotation;
    private float lerp = 0f;
    private Vector2 averageMovementVector;

    private void Start() {
        originalRotation = transform.localEulerAngles;
        //  averageMovementVector = GameInput.Instance.GetPlayerMovementVectorNormalized();
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
        // HandleRotation();
    }

    //private void HandleRotation() {
    //    //Keep a weighted movement vector that determines the tilt direction of the body;
    //    Vector2 inputVector = GameInput.Instance.GetPlayerMovementVectorNormalized();
    //    averageMovementVector = Vector2.Lerp(averageMovementVector, inputVector, Time.deltaTime*rotateBlend);
    //    Vector3 rotateVector = averageMovementVector * rotateMultiplier;
    //    transform.localEulerAngles = originalRotation;
    //    transform.Rotate(rotateVector.y, 0, rotateVector.x * -1);
    //}

}
