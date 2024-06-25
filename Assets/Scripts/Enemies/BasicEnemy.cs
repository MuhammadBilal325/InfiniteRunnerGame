using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class BasicEnemy : BaseEnemy {

    [SerializeField] private float speed;
    [SerializeField] private Vector3 gravityVector;
    [SerializeField] private float playerDistance;
    private GameObject player;
    private Vector3 playerPosition;
    private Vector3 oldPosition;
    private Vector3 newPosition;
    private Vector3 movementDir;
    private Queue<Vector3> lastPlayerPositions;
    private int lastPlayerPositionsCount = 0;
    private int lastPlayerPositionsCountMax = 5;
    private CharacterController enemyController;


    private void Start() {
        lastPlayerPositions = new Queue<Vector3>();
        player = Player.Instance.gameObject;
        playerPosition = player.transform.position;
        oldPosition = transform.position;
        newPosition = transform.position;
        enemyController = GetComponent<CharacterController>();
    }

    private void Update() {
        if (lastPlayerPositionsCount <= lastPlayerPositionsCountMax) {
            playerPosition = player.transform.position;
            lastPlayerPositions.Enqueue(playerPosition);
            lastPlayerPositionsCount++;
        }
        else {
            playerPosition = lastPlayerPositions.Dequeue();
            lastPlayerPositions.Enqueue(player.transform.position);
        }
    }
    private void FixedUpdate() {
        MakeDecision();
        HandleMovement();
    }
    private void MakeDecision() {
        if (Vector3.Distance(playerPosition, transform.position) < playerDistance) {
            movementDir = (transform.position - playerPosition).normalized;
            movementDir.y = 0;
        }
        else if (Vector3.Distance(playerPosition, transform.position) > playerDistance) {
            movementDir = Vector3.zero;
        }
    }

    private void HandleMovement() {
        oldPosition = transform.position;
        //  Simulate gravity
        enemyController.Move(gravityVector * Time.fixedDeltaTime);
        newPosition.y = transform.position.y;
        //  If not falling then move
        if (newPosition.y == oldPosition.y) {
            enemyController.Move(movementDir * speed * Time.fixedDeltaTime);
            newPosition.x = transform.position.x;
        };
        newPosition.z = 0;
        transform.position = newPosition;
    }

}
