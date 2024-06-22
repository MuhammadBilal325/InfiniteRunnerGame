using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class IKBoneTarget : MonoBehaviour {
    [SerializeField] private IKBoneTarget oppositeLeg;
    [SerializeField] private Transform body;
    [SerializeField] private IKTargetSettingsSO ikTargetSettings;
    [SerializeField] private bool drawGizmos;
    private float lerp = 0f;
    private bool lerpToOffset = false;
    //Current position of the leg
    private Vector3 currentPosition;
    //Addition vector added to the newPosition based on user input
    private Vector3 movementVector;
    //Offset vector from the body to the leg
    [SerializeField] private Vector3 offsetVector;
    private Vector3 originalOffsetVector;
    //New position of the leg
    private Vector3 newPosition;
    //Old position of the leg
    private Vector3 oldPosition;
    private Vector3 restPosition;
    private bool isMoving = false;

    private void Start() {
        offsetVector += transform.position - body.position;
        originalOffsetVector = offsetVector;
        currentPosition = transform.position;
        newPosition = transform.position;
        oldPosition = transform.position;
        movementVector = Vector3.zero;
        restPosition = body.transform.position;
    }



    // Update is called once per frame
    private void Update() {
        transform.position = currentPosition;
        if (!isMoving) {
            //Calculate the movementVector to add onto the raycastOrigin to make sure legs dont lag behind or go in front
            //Vector2 inputVector = GameInput.Instance.GetMovementInput();
            //if (inputVector == Vector2.zero) {
            //    movementVector = Vector3.zero;

            //}
            //else {
            //    movementVector = new Vector3(inputVector.x, 0, inputVector.y);
            //    movementVector *= ikTargetSettings.globalMovementVectorMultiplier;
            //}

            Vector3 rayOrigin = body.position + offsetVector + movementVector;
            rayOrigin.y += ikTargetSettings.rayVerticalOffset;
            Ray ray = new Ray(rayOrigin, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit info, ikTargetSettings.rayVerticalOffset + 3)) {
                //If the player has moved a certain distance from the old position, the leg will move to the new position
                if (Vector3.Distance(newPosition, info.point) > ikTargetSettings.stepDistance) {
                    newPosition = info.point;
                    lerp = 0f;
                    lerpToOffset = false;
                }
                //If the player has stood still for a while this condition will run
                if (!lerpToOffset) {
                    newPosition = info.point;
                    lerp = 0f;
                    lerpToOffset = true;
                    restPosition = body.position;
                }

            }
        }
        //Animation of leg
        if (lerp < 1f && (oppositeLeg.IsGrounded())) {
            isMoving = true;
            Vector3 footPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            footPosition.y = ikTargetSettings.stepHeightCurve.Evaluate(lerp) * ikTargetSettings.stepHeight;
            currentPosition = footPosition;
            lerp += Time.deltaTime * ikTargetSettings.speed;
        }
        if (lerp > 1f) {
            isMoving = false;
            currentPosition = newPosition;
            oldPosition = newPosition;

        }
    }

    public bool IsGrounded() {
        return !isMoving;
    }



    private void OnDrawGizmos() {
        //Gizmos.color = UnityEngine.Color.red;
        //Gizmos.DrawSphere(oldPosition, 1f);
        //Gizmos.color = UnityEngine.Color.green;
        //Gizmos.DrawSphere(newPosition, 1f);
        //Gizmos.color = UnityEngine.Color.magenta;

    }
}
