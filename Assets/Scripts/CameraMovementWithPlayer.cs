using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CameraMovementWithPlayer : MonoBehaviour {
    public static CameraMovementWithPlayer Instance { get; private set; }
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform shakeEmpty;
    [SerializeField] private float firstThreshold;
    [SerializeField] private float secondThreshold;
    [SerializeField] private float cameraSnapSpeed;
    [SerializeField] private float maxYaw;
    [SerializeField] private float maxPitch;
    [SerializeField] private float maxRoll;
    [SerializeField] private float maxOffsetX;
    [SerializeField] private float maxOffsetY;
    [SerializeField] private int noiseSeed;
    [SerializeField] private float traumaDecreaseFactor;
    [SerializeField] private float noiseFrequency;
    [SerializeField] private float shakeSnapSpeed;
    [SerializeField] private float shakeMagnitude;
    private float playerOffsetY;
    private float trauma = 0f;
    private float shake;
    private float yaw;
    private float pitch;
    private float roll;
    private float shakeOffsetX;
    private float shakeOffsetY;
    private Vector3 offsetVector;
    private Coroutine cameraShakeRoutine;
    private void Awake() {
        Instance = this;
    }

    private void Start() {
        playerOffsetY = transform.position.y - playerTransform.position.y;
    }


    private void LateUpdate() {
        HandleCameraTranslation();
        HandleCameraShake();
    }

    private void HandleCameraTranslation() {
        //Get Vector from player to camera, x should be less than secondThreshold, if it is greater than secondThreshold then move camera on x axis by the difference
        //We need to invert the checks based on direction so we can just invert the playerRelativeToCamera.x instead
        Vector3 playerRelativeToCamera = playerTransform.position - transform.position;
        int playerDirection = Player.Instance.GetRotationDirection();
        float newXPosition = transform.position.x;
        if (playerDirection == 1) {
            if (playerRelativeToCamera.x > secondThreshold) {
                newXPosition = playerTransform.position.x - secondThreshold;
            }
            else if (playerRelativeToCamera.x < firstThreshold) {
                newXPosition = playerTransform.position.x - firstThreshold;
            }
        }
        else if (playerDirection == -1) {
            if (playerRelativeToCamera.x < -secondThreshold) {
                newXPosition = playerTransform.position.x + secondThreshold;
            }
            else if (playerRelativeToCamera.x > -firstThreshold) {
                newXPosition = playerTransform.position.x + firstThreshold;
            }
        }
        Vector3 newLocation = new Vector3(newXPosition, transform.position.y, transform.position.z);
        newLocation.y = playerTransform.position.y + playerOffsetY;
        transform.position = Vector3.Lerp(transform.position, newLocation, Time.deltaTime * cameraSnapSpeed);
    }

    private void HandleCameraShake() {
        shake = trauma * trauma;
        CameraShakeRotation();
        CameraShakeLocation();
        trauma -= traumaDecreaseFactor * Time.deltaTime;
        trauma = Mathf.Clamp(trauma, 0, 1);
    }
    private void CameraShakeRotation() {
        yaw = maxYaw * shake * Mathf.Lerp(-1, 1, Mathf.PerlinNoise(noiseSeed, Time.realtimeSinceStartup * noiseFrequency));
        pitch = maxPitch * shake * Mathf.Lerp(-1, 1, Mathf.PerlinNoise(noiseSeed + 1, Time.realtimeSinceStartup * noiseFrequency));
        roll = maxRoll * shake * Mathf.Lerp(-1, 1, Mathf.PerlinNoise(noiseSeed + 2, Time.realtimeSinceStartup * noiseFrequency));
        Vector3 shakeRotationVector = new Vector3(pitch, yaw, roll);
        shakeRotationVector *= shakeMagnitude;
        Quaternion shakeRotation = Quaternion.Euler(shakeRotationVector);
        shakeEmpty.localRotation = Quaternion.Lerp(shakeEmpty.localRotation, shakeRotation, Time.deltaTime * shakeSnapSpeed);
    }
    private void CameraShakeLocation() {
        shakeOffsetX = Mathf.Lerp(-1, 1, Mathf.PerlinNoise(noiseSeed + 3, Time.realtimeSinceStartup * noiseFrequency));
        shakeOffsetY = Mathf.Lerp(-1, 1, Mathf.PerlinNoise(noiseSeed + 4, Time.realtimeSinceStartup * noiseFrequency));
        offsetVector = new Vector3(shakeOffsetX, shakeOffsetY, shakeEmpty.localPosition.z);
        offsetVector = offsetVector.normalized;
        offsetVector *= shake;
        offsetVector.x *= maxOffsetX;
        offsetVector.y *= maxOffsetY;
        offsetVector *= shakeMagnitude;
        shakeEmpty.localPosition = Vector3.Lerp(shakeEmpty.localPosition, offsetVector, Time.deltaTime * shakeSnapSpeed);
    }

    public void AddTrauma(float t) {
        trauma += t;
    }
}
