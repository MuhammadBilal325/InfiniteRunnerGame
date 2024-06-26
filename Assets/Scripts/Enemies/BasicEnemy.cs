using KinematicCharacterController;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class BasicEnemy : BaseEnemy, KinematicCharacterController.ICharacterController {

    [SerializeField] private float playerDistance;
    [SerializeField] private float decisionDelay;
    private float decisionTimer = 0f;
    private KinematicCharacterMotor Motor;
    private readonly string ATTACK_TAG = "Attack";
    //Movement
    [SerializeField] private float maxStableMoveSpeed;
    [SerializeField] private float stableMovementSharpness;
    [SerializeField] private float maxAirMoveSpeed;
    [SerializeField] private float airAccelerationSpeed;
    [SerializeField] private float drag;
    [SerializeField] private Vector3 gravity;
    private Vector3 movement;
    private Vector3 targetPosition;


    private void Start() {
        Motor = GetComponent<KinematicCharacterMotor>();
        Motor.CharacterController = this;
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime) { }
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime) {
        Vector3 targetMovementVelocity = Vector3.zero;
        if (Motor.GroundingStatus.IsStableOnGround) {
            // Reorient source velocity on current ground slope (this is because we don't want our smoothing to cause any velocity losses in slope changes)
            currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, Motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;
            // Calculate target velocity
            Vector3 inputRight = Vector3.Cross(movement, Motor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(Motor.GroundingStatus.GroundNormal, inputRight).normalized * movement.magnitude;

            targetMovementVelocity = reorientedInput * maxStableMoveSpeed;

            // Smooth movement Velocity
            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-stableMovementSharpness * deltaTime));
        }
        else {
            // Add move input
            if (movement.sqrMagnitude > 0f) {
                targetMovementVelocity = movement * maxAirMoveSpeed;

                // Prevent climbing on un-stable slopes with air movement
                if (Motor.GroundingStatus.FoundAnyGround) {
                    Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                    targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                }

                Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, gravity);
                currentVelocity += airAccelerationSpeed * deltaTime * velocityDiff;
            }

            // gravity
            currentVelocity += gravity * deltaTime;

            // Drag
            currentVelocity *= (1f / (1f + (drag * deltaTime)));
        }
    }
    public void BeforeCharacterUpdate(float deltaTime) {
        MakeMovementDecision();
    }

    //Enemy makes decision to move towards certain position, we set movement vector towards that position and check after movement if we are there, if we are we set movement vector to zero
    private void MakeMovementDecision() {
        Vector3 playerPosition = Player.Instance.transform.position;
        Vector3 enemyPosition = transform.position;
        //Direction of enemy with player as origin
        Vector3 direction = enemyPosition - playerPosition;
        //Change movement if it has been 3 seconds since last change
        decisionTimer += Time.deltaTime;
        if (decisionTimer > decisionDelay) {
            if (direction.sqrMagnitude < playerDistance * playerDistance) {
                movement = direction.normalized;
                targetPosition = playerPosition + (direction.normalized * playerDistance);
            }
            else {
                movement = Vector3.zero;
            }
            decisionTimer = 0f;
        }
    }
    public void PostGroundingUpdate(float deltaTime) { }
    public void AfterCharacterUpdate(float deltaTime) {
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f) {
            movement = Vector3.zero;
        }
    }
    public bool IsColliderValidForCollisions(Collider coll) {
        if (coll.gameObject.CompareTag(ATTACK_TAG)) {
            return false;
        }
        return true;
    }
    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }
    public void OnDiscreteCollisionDetected(Collider hitCollider) { }
}
