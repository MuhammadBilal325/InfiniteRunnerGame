using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementWithPlayer : MonoBehaviour {
    public static CameraMovementWithPlayer Instance { get; private set; }
    [SerializeField] private Transform Player;
    [SerializeField] private float safeDistance = 0;
    [SerializeField] private Transform shakeEmpty;
    private float xOffset = 0;
    private float yOffset = 0;
    private float validMaxPlayerPos;
    private float validMinPlayerPos;
    private float xAdjustment = 0f;
    private Coroutine cameraShakeRoutine;
    private void Awake() {
        Instance = this;
    }
    private void Start() {
        xOffset = Player.position.x - transform.position.x;
        yOffset = Player.position.y - transform.position.y;
    }

    private void FixedUpdate() {
        validMaxPlayerPos = transform.position.x + xOffset + safeDistance;
        validMinPlayerPos = transform.position.x + xOffset - safeDistance;
        if (Player.transform.position.x > validMaxPlayerPos) {
            xAdjustment = Player.transform.position.x - validMaxPlayerPos;
        }
        else if (Player.transform.position.x < validMinPlayerPos) {
            xAdjustment = Player.transform.position.x - validMinPlayerPos;
        }
        else
            xAdjustment = 0;

        transform.position = new Vector3(transform.position.x + xAdjustment, Player.position.y - yOffset, transform.position.z);
    }

    public void ShakeCamera(float duration, float magnitude) {
        if (cameraShakeRoutine == null) {
            cameraShakeRoutine = StartCoroutine(Shake(duration, magnitude));
        }
    }

    IEnumerator Shake(float duration, float magnitude) {
        Vector3 originalPos = shakeEmpty.localPosition;
        float elapsed = 0.0f;
        while (elapsed < duration) {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            shakeEmpty.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        shakeEmpty.localPosition = originalPos;
        cameraShakeRoutine = null;
    }
}
