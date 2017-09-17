using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class GTAnimator : MonoBehaviour
{
    
    public enum AnimatorStateProtagonist
    {
        IDLE = -1, WALKING, HADOKEN, JUMPING
    }

    private Animator _animator;
    private Animator animator {
        get {
            if (!_animator) { _animator = GetComponent<Animator>(); }
            return _animator;
        }
    }

   

    [SerializeField]
    protected string defaultActionStateParam = "ActionState";
    [SerializeField]
    protected string isDeadParam = "IsDead";


    public void idle() { setIntegerParam((int)AnimatorStateProtagonist.IDLE); }

    public void walk() { setIntegerParam((int)AnimatorStateProtagonist.WALKING); }

    public void jump() { setIntegerParam((int)AnimatorStateProtagonist.JUMPING); }
    
    public void hadoken() { setIntegerParam((int)AnimatorStateProtagonist.HADOKEN); }

    public void die() { setBoolParam(isDeadParam, true); }

    public void revive() { setBoolParam(isDeadParam, false); }


    public void setIntegerParam(int state) { setIntegerParam(defaultActionStateParam, state); }

    public void setIntegerParam(string param, int state) {
        animator.SetInteger(param, state);
    }

    public void setBoolParam(string param, bool state) {
        animator.SetBool(param, state);
    }

    public void setFloatParam(string param, float n) {
        animator.SetFloat(param, n);
    }

    public void returnToWalking() {
        walk();
    }
}
