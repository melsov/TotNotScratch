using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(GTAnimator), false)]
public class GTAnimatorDataEditor : Editor
{

    public override void OnInspectorGUI() {
        GTAnimator gt = (GTAnimator)target;
        Animator animator = gt.GetComponent<Animator>();
        if(!animator) {
            GUI.backgroundColor = new Color(1f, .5f, .7f);
            EditorGUILayout.LabelField("GTAnimator needs an animator component");
            if(GUILayout.Button("Add an animator component")) {
                gt.gameObject.AddComponent<Animator>();
            }
            return;
        }

        base.OnInspectorGUI();
    }
}
#endif

public class GTAnimator : MonoBehaviour
{
    
    //public enum AnimatorStateProtagonist
    //{
    //    IDLE = -1, WALKING, HADOKEN, JUMPING
    //}


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


    //public void idle() { setMainIntegerParam((int)AnimatorStateProtagonist.IDLE); }

    //public void walk() { setMainIntegerParam((int)AnimatorStateProtagonist.WALKING); }

    //public void jump() { setMainIntegerParam((int)AnimatorStateProtagonist.JUMPING); }

    //public void hadoken() { setMainIntegerParam((int)AnimatorStateProtagonist.HADOKEN); }

    public void die() {
        setBoolParam(isDeadParam, true);
    }

    public void revive() { setBoolParam(isDeadParam, false); }


    public void setMainIntegerParam(int state) { setIntegerParam(defaultActionStateParam, state); }

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
        setMainIntegerParam(1);
    }
}
