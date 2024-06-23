using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour {

    public event EventHandler JumpInputPressed;
    public event EventHandler Attack1Pressed;
    public static GameInput Instance { get; private set; }
    private PlayerInputActions playerInputActions;
    // Start is called before the first frame update
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Jump.performed += Jump_performed;
        playerInputActions.Player.Attack.performed += Attack_performed;
        playerInputActions.Player.Enable();
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        Attack1Pressed?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        JumpInputPressed?.Invoke(this, EventArgs.Empty);
    }

    public float GetMovementInput() {
        return playerInputActions.Player.Move.ReadValue<float>();
    }
}