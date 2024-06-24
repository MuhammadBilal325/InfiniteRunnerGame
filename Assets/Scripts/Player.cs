using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour {
    public static Player Instance { get; private set; }
    public event EventHandler Attack1Pressed;
    public event EventHandler Attack2Pressed;
    [SerializeField] private float attack1ResetTime;
    [SerializeField] private float attack2ResetTime;
    private float attackCooldown = 0f;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float fallMultiplier = 2.5f; // Multiplier to increase fall speed
    [SerializeField] private float lowJumpMultiplier = 2.0f; // Multiplier to make jumps more responsive
    [SerializeField] private AttackListSO attackList;
    [SerializeField] private Transform attackPoint;
    Coroutine attackCoroutine;
    private Rigidbody rb;
    private Vector3 movement;
    private bool isGrounded;

    private void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
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
        if (isGrounded) {
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }
    }

    // Update is called once per frame
    void Update() {
        if (attackCooldown > 0) {
            attackCooldown -= Time.deltaTime;
        }
        movement = new Vector3(GameInput.Instance.GetMovementInput(), 0, 0);
        isGrounded = Physics.Raycast(transform.position + new Vector3(0, 1, 0), Vector3.down, 1.1f, groundLayer);
    }

    private void FixedUpdate() {
        rb.velocity = new Vector3(movement.x * 200 * playerSpeed * Time.fixedDeltaTime, rb.velocity.y, rb.velocity.z);
        // Apply fall multiplier if the player is falling
        if (rb.velocity.y < 0) {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        // Apply low jump multiplier if the player is jumping
        else if (rb.velocity.y > 0) {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
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
