using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

public class GTCameraFollow : MonoBehaviour
{
//TODO: camera offset
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Vector3 offset;

    public void setTarget(Transform target) {
        this.target = target;
    }

    public enum ModeType
    {
        STATIC_DONT_FOLLOW, ALWAYS_CENTERED, CENTER_X_LAZY_Y, VIEWPORT_TRAVERSAL
    }

    public ModeType followMode;

    private Dictionary<ModeType, Action> follows;
    [SerializeField]
    private float smoothing = .5f;

    private ViewPortHelper viewportHelper;

    private static Vector3 vxy = new Vector3(1f, 1f, 0f);

    private void Awake() {
        viewportHelper = ComponentHelper.FindAnywhereOrAdd<ViewPortHelper>();
        follows = new Dictionary<ModeType, Action>() {

            { ModeType.STATIC_DONT_FOLLOW, () => { } },
            { ModeType.ALWAYS_CENTERED, () => {
                transform.position = Vector3.Scale(Vector3.Lerp(transform.position, target.position, 4f * smoothing * Time.deltaTime), vxy) + offset +
                Vector3.Scale(transform.position, Vector3.forward);
            }
            },
            { ModeType.CENTER_X_LAZY_Y, () => {
                centerXLazyY();
            }
            },
            { ModeType.VIEWPORT_TRAVERSAL, () => {
                viewportTraversal();
            }
            }

        };

        if(recentTraversals.traversalSpeed <= 0f) {
            recentTraversals.traversalSpeed = 1f;
        }
    }

    private void centerXLazyY() {
        float xx = Mathf.Lerp(transform.position.x, target.position.x, 4f * smoothing * Time.deltaTime) + offset.x;
        VectorXY edgeCatchup = viewportHelper.normalizedCenteredness(target.position);

        float yy = Mathf.Lerp(transform.position.y, target.position.y, 4f * smoothing * edgeCatchup.y * edgeCatchup.y * catchupWhenNearEdgeSpeed / yLaziness * Time.deltaTime) + offset.y;
        transform.position = new Vector3(xx, yy, transform.position.z);
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
    [SerializeField]
    private float yLaziness = 10f;
    [SerializeField]
    private float catchupWhenNearEdgeSpeed = 20f;

    private void viewportTraversal() {
        if(viewportHelper.containsGlobal(target.position)) { return; }

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
            VectorXY target = new VectorXY(transform.position) + new VectorXY(viewportHelper.viewPortGlobalSize) * dir;
            for(int i = 0; i < 24; ++i) {
                transform.position = Vector3.Lerp(transform.position, target.vector3(transform.position.z), 4f * recentTraversals.traversalSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            transform.position = target.vector3(transform.position.z);
            recentTraversals.recentlyTraversed = false;
        }
    }


    private void FixedUpdate() {
        follows[followMode].Invoke();
    }

}
