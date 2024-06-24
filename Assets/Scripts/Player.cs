using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour {

    private enum State {
        Grounded,
        JumpingUp,
        JumpingDown,
    }
    public static Player Instance { get; private set; }
    public event EventHandler Attack1Pressed;
    public event EventHandler Attack2Pressed;
    [SerializeField] private float attack1ResetTime;
    [SerializeField] private float attack2ResetTime;
    private float attackCooldown = 0f;
    [SerializeField] private float playerSpeed;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private AttackListSO attackList;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Vector3 gravity;
    [SerializeField] private Vector3 jumpVector;
    [SerializeField] private float jumpDamping;
    [SerializeField] private float gravityDamping;
    private CharacterController characterController;
    private float jumpLerp = 1f;
    private float gravityLerp = 1f;
    private Vector3 movement;
    private Vector3 oldPosition;
    private Vector3 newPosition;
    private State state;
    private void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        state = State.Grounded;
        characterController = GetComponent<CharacterController>();
        GameInput.Instance.JumpInputPressed += GameInput_JumpInputPressed;
        GameInput.Instance.Attack1Pressed += GameInput_Attack1Pressed;
        GameInput.Instance.Attack2Pressed += GameInput_Attack2Pressed;
    }

    private void GameInput_Attack2Pressed(object sender, EventArgs e) {
        if (attackCooldown <= 0) {
            attackCooldown = attack2ResetTime;
            Attack2();
        }
    }

    private void GameInput_Attack1Pressed(object sender, EventArgs e) {
        if (attackCooldown <= 0) {
            attackCooldown = attack1ResetTime;
            Attack1();
        }
    }

    private void GameInput_JumpInputPressed(object sender, System.EventArgs e) {
        if (state == State.Grounded) {
            state = State.JumpingUp;
            jumpLerp = 1f;
        }
    }

    // Update is called once per frame
    void Update() {
        if (attackCooldown > 0) {
            attackCooldown -= Time.deltaTime;
        }
        movement = new Vector3(GameInput.Instance.GetMovementInput(), 0, 0);
        if (state != State.JumpingUp)
            state = Physics.Raycast(transform.position + new Vector3(0, 1, 0), Vector3.down, 1.1f, groundLayer) ? State.Grounded : state;
    }

    private void FixedUpdate() {
        HandleMovement();
    }
    private void HandleMovement() {
        oldPosition = transform.position;
        //Simulate horizontal movement
        characterController.Move(movement * playerSpeed * Time.deltaTime);
        newPosition = transform.position;
        newPosition.z = oldPosition.z;
        newPosition.y = oldPosition.y;
        //Simulate vertical movement with inverse velocity if player is jumping up
        if (state == State.JumpingUp) {
            characterController.Move(jumpLerp * jumpVector);
            jumpLerp -= jumpDamping * Time.deltaTime;
            if (jumpLerp <= 0) {
                state = State.JumpingDown;
                gravityLerp = 1f;
            }
        }
        else if (state == State.JumpingDown) {
            characterController.Move(gravity * gravityLerp * Time.deltaTime);
            gravityLerp += gravityDamping * Time.deltaTime;

        }
        else {
            characterController.Move(gravity * Time.deltaTime);
        }
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
    private void Attack1() {
        Attack1Pressed?.Invoke(this, EventArgs.Empty);
        Instantiate(attackList.attacks[0].prefab, attackPoint.position, Quaternion.identity);
    }

    private void Attack2() {
        Attack2Pressed?.Invoke(this, EventArgs.Empty);
        Instantiate(attackList.attacks[1].prefab, attackPoint.position, Quaternion.identity);
    }
}
