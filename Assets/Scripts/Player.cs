using KinematicCharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour, KinematicCharacterController.ICharacterController {

    private enum State {
        Grounded,
        JumpingUp,
        JumpingDown,
    }
    public static Player Instance { get; private set; }
    //Attacking 

    public event EventHandler StartHitPause;
    public event EventHandler EndHitPause;
    public event EventHandler Attack1Pressed;
    public event EventHandler<AttackEventArgs> Attack2Pressed;
    public class AttackEventArgs : EventArgs {
        public int attackStage;
    }
    private bool attackTimersPaused = false;
    private bool allowAttacks = true;
    private Coroutine hitPauseCoroutine = null;
    [SerializeField] private float hitPauseTime;
    [SerializeField] private AttackListSO attackList;
    [SerializeField] private Transform attackPoint;

    [SerializeField] private float attack1CoolDown;
    [SerializeField] private float attack1Delay;
    [SerializeField] private float attack2ResetTime;
    [SerializeField] private float attack2MidCoolDown;
    [SerializeField] private float attack2MidDelay;
    [SerializeField] private float attack2EndCoolDown;
    [SerializeField] private float attack2EndDelay;
    private float attack2ResetTimer;
    private Coroutine attackCoroutine;
    private int attack2Stage = 0;
    private readonly int attack2MaxStages = 3;

    private float attackCooldown = 0f;
    private Vector3 attackPointOffset;
    private Vector3 attackPointInverseOffset;
    //Movement
    [SerializeField] private float playerSpeed;
    [SerializeField] private float maxStableMoveSpeed;
    [SerializeField] private float stableMovementSharpness;
    [SerializeField] private float maxAirMoveSpeed;
    [SerializeField] private float airAccelerationSpeed;
    [SerializeField] private float drag;
    [SerializeField] private Transform playerRotation;
    [SerializeField] private Vector3 gravity;
    private KinematicCharacterMotor Motor;
    private readonly string ATTACK_TAG = "Attack";
    private Vector3 movement;
    //Rotation
    [SerializeField] private float rotationSpeed;
    private int direction;
    private Quaternion forwardQuaternion;
    private Quaternion backwardQuaternion;

    //Jumping
    public event EventHandler PlayerJumped;
    public event EventHandler PlayerLanded;
    [SerializeField] private bool AllowJumpingWhenSliding = false;
    [SerializeField] private float JumpSpeed = 10f;
    [SerializeField] private float JumpPreGroundingGraceTime = 0f;
    [SerializeField] private float JumpPostGroundingGraceTime = 0f;
    private bool _jumpRequested = false;
    private bool _jumpConsumed = false;
    private bool _jumpedThisFrame = false;
    private float _timeSinceJumpRequested = Mathf.Infinity;
    private float _timeSinceLastAbleToJump = 0f;

    private void Awake() {
        Instance = this;

    }

    // Start is called before the first frame update
    void Start() {
        attackPointOffset = attackPoint.position - transform.position;
        attackPointInverseOffset = new Vector3(-attackPointOffset.x, attackPointOffset.y, attackPointOffset.z);
        Vector3 forwardRotation = playerRotation.rotation.eulerAngles;
        Vector3 backwardRotation = new(playerRotation.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 120, transform.rotation.eulerAngles.z);
        forwardQuaternion = Quaternion.Euler(forwardRotation);
        backwardQuaternion = Quaternion.Euler(backwardRotation);
        Motor = GetComponent<KinematicCharacterMotor>();
        Motor.CharacterController = this;
        GameInput.Instance.JumpInputPressed += GameInput_JumpInputPressed;
        GameInput.Instance.Attack1Pressed += GameInput_Attack1Pressed;
        GameInput.Instance.Attack2Pressed += GameInput_Attack2Pressed;
    }

    private void GameInput_Attack2Pressed(object sender, EventArgs e) {
        if (!allowAttacks) {
            return;
        }
        if (attackCooldown <= 0) {
            //If attack cooldown has been reset
            if (attack2Stage == attack2MaxStages - 1) {
                attackCooldown = attack2EndCoolDown;
            }
            else
                attackCooldown = attack2MidCoolDown;
            attack2ResetTimer = 0;
            Attack2();
            attack2Stage++;
            attack2Stage %= attack2MaxStages;
        }
    }

    private void GameInput_Attack1Pressed(object sender, EventArgs e) {
        if (!allowAttacks) {
            return;
        }
        if (attackCooldown <= 0) {
            attackCooldown = attack1CoolDown;
            Attack1();
        }
    }

    private void GameInput_JumpInputPressed(object sender, System.EventArgs e) {
        if (Motor.GroundingStatus.IsStableOnGround) {
            _timeSinceJumpRequested = 0f;
            _jumpRequested = true;
        }
    }

    // Update is called once per frame
    void Update() {
        //Dont puase attackCooldown during hitpause to maintain snappy feel
        if (attackCooldown > 0) {
            attackCooldown -= Time.deltaTime;
        }
        if (!attackTimersPaused) {
            if (attack2ResetTimer < attack2ResetTime) {
                attack2ResetTimer += Time.deltaTime;
            }
            else {
                attack2ResetTimer = attack2ResetTime;
                attack2Stage = 0;
            }
        }
        movement = new Vector3(GameInput.Instance.GetMovementInput(), 0, 0);
        direction = PointerXRelativeToPlayer();
    }


    private void FixedUpdate() {
        HandleRotationOfPlayer();
    }

    private void HandleRotationOfPlayer() {
        if (direction == 1) {
            playerRotation.rotation = Quaternion.Lerp(playerRotation.rotation, forwardQuaternion, rotationSpeed * Time.fixedDeltaTime);
            attackPoint.SetLocalPositionAndRotation(attackPointOffset, Quaternion.Euler(0, 0, 0));
        }
        else if (direction == -1) {
            attackPoint.SetLocalPositionAndRotation(attackPointInverseOffset, Quaternion.Euler(0, 180, 0));
            playerRotation.rotation = Quaternion.Lerp(playerRotation.rotation, backwardQuaternion, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void Attack1() {
        Attack1Pressed?.Invoke(this, EventArgs.Empty);
        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackInstantiateDelay(attack1Delay, attackList.attacks[0].prefab, attackPoint));
    }

    private void Attack2() {
        Attack2Pressed?.Invoke(this, new AttackEventArgs { attackStage = attack2Stage });
        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
        if (attack2Stage == 0) {
            attackCoroutine = StartCoroutine(AttackInstantiateDelay(attack2MidDelay, attackList.attacks[1].prefab, attackPoint));
        }
        else if (attack2Stage == 1) {
            attackCoroutine = StartCoroutine(AttackInstantiateDelay(attack2MidDelay, attackList.attacks[2].prefab, attackPoint));
        }
        else if (attack2Stage == 2) {
            attackCoroutine = StartCoroutine(AttackInstantiateDelay(attack2EndDelay, attackList.attacks[3].prefab, attackPoint));
        }
    }
    IEnumerator AttackInstantiateDelay(float delay, Transform prefab, Transform attackPoint) {
        yield return new WaitForSeconds(delay);
        Instantiate(prefab, attackPoint.position, attackPoint.rotation);
        attackCoroutine = null;
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime) {

    }

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
        //Handle jumping
        _jumpedThisFrame = false;
        _timeSinceJumpRequested += deltaTime;
        if (_jumpRequested) {
            // See if we actually are allowed to jump
            if (!_jumpConsumed && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime)) {
                // Calculate jump direction before ungrounding
                Vector3 jumpDirection = Motor.CharacterUp;
                if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround) {
                    jumpDirection = Motor.GroundingStatus.GroundNormal;
                }

                // Makes the character skip ground probing/snapping on its next update. 
                // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                Motor.ForceUnground(0.1f);

                // Add to the return velocity and reset jump state
                currentVelocity += (jumpDirection * JumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                _jumpRequested = false;
                _jumpConsumed = true;
                _jumpedThisFrame = true;
            }
        }
    }



    public int GetDirection() {
        return direction;
    }
    public int GetMovementDirection() {
        return (int)GameInput.Instance.GetMovementInput() * direction;
    }
    public bool IsSprinting() {
        return GameInput.Instance.GetSprintInput() > 0;
    }
    private int PointerXRelativeToPlayer() {
        Vector3 pointer = Input.mousePosition;
        pointer.z = transform.position.z - Camera.main.transform.position.z;
        pointer = Camera.main.ScreenToWorldPoint(pointer);
        pointer.z = 0;
        Vector3 player = transform.position;
        player.z = 0;
        Vector3 direction = pointer - player;
        return direction.x > 0 ? 1 : -1;
    }


    public void BeforeCharacterUpdate(float deltaTime) { }
    public void PostGroundingUpdate(float deltaTime) {
        // Handle landing and leaving ground
        if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround) {
            PlayerLanded?.Invoke(this, EventArgs.Empty);
        }
        else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround) {
            PlayerJumped?.Invoke(this, EventArgs.Empty);
        }
    }
    public void AfterCharacterUpdate(float deltaTime) {
        // Handle jump-related values
        {
            // Handle jumping pre-ground grace period
            if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime) {
                _jumpRequested = false;
            }

            // Handle jumping while sliding
            if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) {
                // If we're on a ground surface, reset jumping values
                if (!_jumpedThisFrame) {
                    _jumpConsumed = false;
                }
                _timeSinceLastAbleToJump = 0f;
            }
            else {
                // Keep track of time since we were last able to jump (for grace period)
                _timeSinceLastAbleToJump += deltaTime;
            }
        }
    }

    public void PausePlayerAttackTimers() {
        attackTimersPaused = true;
        allowAttacks = false;
    }

    public void UnPausePlayerAttackTimers() {
        attackTimersPaused = false;
        allowAttacks = true;
    }
    public void HitPause() {
        StartHitPause?.Invoke(this, EventArgs.Empty);
        PausePlayerAttackTimers();
        if (hitPauseCoroutine != null)
            StopCoroutine(hitPauseCoroutine);
        hitPauseCoroutine = StartCoroutine(HitPauseDisableCoroutine());
    }

    private IEnumerator HitPauseDisableCoroutine() {
        yield return new WaitForSecondsRealtime(hitPauseTime);
        EndHitPause?.Invoke(this, EventArgs.Empty);
        UnPausePlayerAttackTimers();
        hitPauseCoroutine = null;
    }
    public bool IsColliderValidForCollisions(Collider coll) {
        return !coll.CompareTag(ATTACK_TAG);
    }
    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }
    public void OnDiscreteCollisionDetected(Collider hitCollider) { }
}
