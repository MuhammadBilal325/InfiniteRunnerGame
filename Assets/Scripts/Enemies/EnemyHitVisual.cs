using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitVisual : MonoBehaviour {
    [SerializeField] private BaseEnemy baseEnemy;
    [SerializeField] private float flashSpeed;
    private MeshRenderer meshRenderer;
    private Color originalEmissionColor;
    private Color emissionColor;
    private Color flashColor;
    private string EMISSION_COLOR = "_EmissionColor";
    private float flashLerp = 1f;
    private bool isFlashing = false;
    // Start is called before the first frame update
    void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        originalEmissionColor = meshRenderer.material.GetColor("_EmissionColor");
        meshRenderer.material.EnableKeyword("_EMISSION");
        flashColor = Color.red;
        baseEnemy.OnHitEvent += BaseEnemy_OnHitEvent;
    }

    private void BaseEnemy_OnHitEvent(object sender, System.EventArgs e) {
        flashLerp = 0f;
        isFlashing = true;
        meshRenderer.material.SetColor(EMISSION_COLOR, flashColor);

    }

    private void Update() {
        if (flashLerp < 1f) {
            flashLerp += Time.deltaTime * flashSpeed;
            emissionColor = Color.Lerp(flashColor, originalEmissionColor, flashLerp);
            meshRenderer.material.SetColor(EMISSION_COLOR, emissionColor);
        }
        else if (isFlashing) {
            isFlashing = false;
            meshRenderer.material.SetColor(EMISSION_COLOR, originalEmissionColor);
        }
    }
}
