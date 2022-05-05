using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlashOnImpactEffect : MonoBehaviour {
    private float flashTime = 0.1f;
    private float curTime = 0;
    SpriteRenderer spriteRenderer;
    Material spriteMaterial;
    Material flashMaterial;
    bool impacting = false;
    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteMaterial = spriteRenderer.material;
        flashMaterial = new Material(Shader.Find("GUI/Text Shader"));
    }

    public void Impact() {
        impacting = true;
        curTime = flashTime;
        spriteRenderer.material = flashMaterial;
    }

    private void Update() {
        if (impacting) {
            if (curTime > 0) {
                curTime -= Time.deltaTime;
            }
            else {
                spriteRenderer.material = spriteMaterial;
                impacting = false;
            }
        }
    }
}
