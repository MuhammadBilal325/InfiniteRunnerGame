using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class IKTargetSettingsSO : ScriptableObject {
    public float stepDistance;
    public float stepHeight;
    public float speed;
    public float rayVerticalOffset;
    public float globalMovementVectorMultiplier;
    public float movementVectorResetSpeed;
    public float restRadius;
    public AnimationCurve stepHeightCurve;
}
