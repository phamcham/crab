using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashEffect : MonoBehaviour {
    [SerializeField] Animation anim;
    private const string SLASH_EFFECT = "slash effect";
    private void OnEnable() {
        StartCoroutine(AutoReturnPool());
    }
    IEnumerator AutoReturnPool() {
        anim.Play(SLASH_EFFECT);
        while (anim.IsPlaying(SLASH_EFFECT)) {
            yield return null;
        }
        //print("return pool");
        SlashEffectManager.ReturnBulletPooled(this);
    }
}
