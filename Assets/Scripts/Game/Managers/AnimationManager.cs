using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class AnimationManager : MonoBehaviour {
    private Animation _anim;
    private Animation anim {
        get {
            if (_anim == null) {
                _anim = GetComponent<Animation>();
            }
            return _anim;
        }
    }

    private string currentAnimationName;

    public void Play(string animationName) {
        //if (currentAnimationName != animationName) {
            currentAnimationName = animationName;
            //print(animationName);
            //Stop();
            anim.Play(animationName, PlayMode.StopAll);
       // }
    }
    public void Stop() {
        anim.Stop();
    }
}
