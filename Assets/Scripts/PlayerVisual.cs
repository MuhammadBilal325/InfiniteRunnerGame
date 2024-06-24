using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour {
    // Start is called before the first frame update
    [SerializeField] private float attack2Duration;
    [SerializeField] private float attack1Duration;
    private Animator animator;
    [SerializeField] private Transform[] swordTips;
    private string MOVEMENT_INPUT = "Movement";
    private string ATTACK1_TRIGGER = "Attack1";
    private string ATTACK2_TRIGGER = "Attack2";
    private string ATTACK2_STATE = "Attack2State";
    private int attack2State = 0;
    private Coroutine attackSwordTrailCoroutine = null;
    private void Start() {
        animator = GetComponent<Animator>();
        Player.Instance.Attack1Pressed += Player_Attack1Pressed;
        Player.Instance.Attack2Pressed += Player_Attack2Pressed; ;
    }

    private void Player_Attack2Pressed(object sender, System.EventArgs e) {
        Attack2Visual();
    }

    private void Player_Attack1Pressed(object sender, System.EventArgs e) {
        Attack1Visual();
    }

    // Update is called once per frame
    private void Update() {
        int input = Player.Instance.GetMovementDirection();
        animator.SetInteger(MOVEMENT_INPUT, input);
    }
    private void Attack1Visual() {
        animator.SetTrigger(ATTACK1_TRIGGER);
        if (attackSwordTrailCoroutine == null) {
            for (int i = 0; i < swordTips.Length; i++) {
                swordTips[i].gameObject.SetActive(true);
            }
            attackSwordTrailCoroutine = StartCoroutine(AttackVisualStopSwordTrailCoroutine(attack1Duration));
        }
    }

    private void Attack2Visual() {
        animator.SetTrigger(ATTACK2_TRIGGER);
        animator.SetInteger(ATTACK2_STATE, attack2State);
        attack2State = attack2State + 1;
        attack2State %= 2;
        swordTips[attack2State].gameObject.SetActive(true);
        if (attackSwordTrailCoroutine == null)
            attackSwordTrailCoroutine = StartCoroutine(AttackVisualStopSwordTrailCoroutine(attack2Duration));
    }

    IEnumerator AttackVisualStopSwordTrailCoroutine(float time) {
        yield return new WaitForSeconds(time);
        for (int i = 0; i < swordTips.Length; i++) {
            swordTips[i].gameObject.SetActive(false);
        }
        attackSwordTrailCoroutine = null;
    }
}
