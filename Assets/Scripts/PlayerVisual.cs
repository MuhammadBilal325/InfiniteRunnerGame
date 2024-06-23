using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour {
    // Start is called before the first frame update
    private Animator animator;
    private string MOVEMENT_INPUT = "Movement";
    private string ATTACK1_TRIGGER = "Attack1";
    private void Start() {
        animator = GetComponent<Animator>();
        Player.Instance.Attack1Pressed += Player_Attack1Pressed;
    }

    private void Player_Attack1Pressed(object sender, System.EventArgs e) {
        Attack1Visual();
    }

    // Update is called once per frame
    private void Update() {
        int input = (int)GameInput.Instance.GetMovementInput();
        animator.SetInteger(MOVEMENT_INPUT, input);
    }
    private void Attack1Visual() {
        animator.SetTrigger(ATTACK1_TRIGGER);
    }
}
