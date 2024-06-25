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
    private string ATTACK2_STAGE = "Attack2Stage";
    private string SPRINT_PARAMETER = "SprintMultiplier";
    private int attack2State = 0;
    private Coroutine attackSwordTrailCoroutine = null;
    private void Start() {
        animator = GetComponent<Animator>();
        Player.Instance.Attack1Pressed += Player_Attack1Pressed;
        Player.Instance.Attack2Pressed += Player_Attack2Pressed; ;
    }

    private void Player_Attack2Pressed(object sender, Player.AttackEventArgs e) {
        Attack2Visual(e.attackStage);
    }

    private void Player_Attack1Pressed(object sender, System.EventArgs e) {
        Attack1Visual();
    }

    // Update is called once per frame
    private void Update() {
        int input = Player.Instance.GetMovementDirection();
        animator.SetInteger(MOVEMENT_INPUT, input);
        bool isSprinting = Player.Instance.IsSprinting();
        if (isSprinting) {
            animator.SetFloat(SPRINT_PARAMETER, 2f);
        }
        else {
            animator.SetFloat(SPRINT_PARAMETER, 1f);
        }
    }
    private void Attack1Visual() {
        animator.SetTrigger(ATTACK1_TRIGGER);
        RenderSwordTrails(attack1Duration);
    }

    private void Attack2Visual(int attack2Stage) {
        animator.SetTrigger(ATTACK2_TRIGGER);
        animator.SetInteger(ATTACK2_STAGE, attack2Stage);
        swordTips[attack2State].gameObject.SetActive(true);
        RenderSwordTrails(attack2Duration);
    }

    void RenderSwordTrails(float duration) {
        if (attackSwordTrailCoroutine != null) {
            StopCoroutine(attackSwordTrailCoroutine);
        }
        for (int i = 0; i < swordTips.Length; i++) {
            swordTips[i].gameObject.SetActive(true);
        }
        attackSwordTrailCoroutine = StartCoroutine(AttackVisualStopSwordTrailCoroutine(duration));

    }
    IEnumerator AttackVisualStopSwordTrailCoroutine(float time) {
        yield return new WaitForSeconds(time);
        for (int i = 0; i < swordTips.Length; i++) {
            swordTips[i].gameObject.SetActive(false);
        }
        attackSwordTrailCoroutine = null;
    }
}
