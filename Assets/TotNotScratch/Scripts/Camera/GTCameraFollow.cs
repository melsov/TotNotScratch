using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

public class GTCameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    public void setTarget(Transform target) {
        this.target = target;
    }

    public enum ModeType
    {
        STATIC_DONT_FOLLOW, ALWAYS_CENTERED, VIEWPORT_TRAVERSAL
    }

    public ModeType currentMode;

    private Dictionary<ModeType, Action> follows;
    [SerializeField]
    private float smoothing = .5f;

    private static Vector3 vxy = new Vector3(1f, 1f, 0f);

    private void Awake() {
        follows = new Dictionary<ModeType, Action>() {

            { ModeType.STATIC_DONT_FOLLOW, () => { } },
            { ModeType.ALWAYS_CENTERED, () => {
                transform.position = Vector3.Scale(Vector3.Lerp(transform.position, target.position, 4f * smoothing * Time.deltaTime), vxy) +
                Vector3.Scale(transform.position, Vector3.forward);
            }
            },
            { ModeType.VIEWPORT_TRAVERSAL, () => { viewportTraversal(); } }

        };

        if(recentTraversals.traversalSpeed <= 0f) {
            recentTraversals.traversalSpeed = 1f;
        }
    }

    [System.Serializable]
    struct RecentTraversals
    {
        [HideInInspector]
        public bool recentlyTraversed;

        [SerializeField]
        public float traversalSpeed;

    }
    private RecentTraversals recentTraversals;

    private void viewportTraversal() {
        if(ViewPortHelper.Instance.containsGlobal(target.position)) { return; }

        VectorXY dir = new VectorXY(target.position - transform.position).saturatedNegPos;
        if(Mathf.Abs(dir.x) > 0f) {
            StartCoroutine(traversalTimeout(dir));
        }
        if (Mathf.Abs(dir.y) > 0f) {
            StartCoroutine(traversalTimeout(dir));
        }
    }

    private IEnumerator traversalTimeout(VectorXY dir) {
        if(!recentTraversals.recentlyTraversed) {
            recentTraversals.recentlyTraversed = true;
            VectorXY target = new VectorXY(transform.position) + new VectorXY(ViewPortHelper.Instance.viewPortGlobalSize) * dir;
            for(int i = 0; i < 24; ++i) {
                transform.position = Vector3.Lerp(transform.position, target.vector3(transform.position.z), 4f * recentTraversals.traversalSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            transform.position = target.vector3(transform.position.z);
            recentTraversals.recentlyTraversed = false;
        }
    }


    private void FixedUpdate() {
        follows[currentMode].Invoke();
    }

}
