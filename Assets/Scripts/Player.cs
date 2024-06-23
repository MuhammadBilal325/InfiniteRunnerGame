using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour {
    public static Player Instance { get; private set; }
    public event EventHandler Attack1Pressed;
    [SerializeField] private float attack1ResetTime;
    [SerializeField] private float attack1Delay;
    private float attack1Cooldown = 0f;
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
    }

    private void GameInput_Attack1Pressed(object sender, EventArgs e) {
        if (attack1Cooldown <= 0) {
            attack1Cooldown = attack1ResetTime;
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
        if (attack1Cooldown > 0) {
            attack1Cooldown -= Time.deltaTime;
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
        if (attackCoroutine == null) {
            attackCoroutine = StartCoroutine(WaitAndInstantiate(attack1Delay, attackList.attacks[0].prefab, attackPoint, Quaternion.identity));
        }
    }
    IEnumerator WaitAndInstantiate(float secondsToWait, Transform objectToInstantiate, Transform objectToInstantiateOn, Quaternion rotation) {
        yield return new WaitForSeconds(secondsToWait);
        Instantiate(objectToInstantiate, objectToInstantiateOn.position, rotation);
        attackCoroutine = null;
    }
}
