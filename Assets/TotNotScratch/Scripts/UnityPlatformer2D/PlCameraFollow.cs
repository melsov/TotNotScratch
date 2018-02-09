using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlCameraFollow : MonoBehaviour
{
	[SerializeField]
	private Transform target;
	[SerializeField]
	private PlayerPlatformerController player;

	private ViewPortHelper viewportHelper;
	[SerializeField]
	private float smoothing = .5f;
	[SerializeField]
	private float catchupWhenNearEdgeSpeed;
	[SerializeField]
	private float yLaziness;

	private Vector2 smoothGroundPoint;
	private RingBuffer<float> lastGroundPointYs;
    private Abyss abyss;

    ViewPortHelper viewPortHelper;

    [HideInInspector]
    public bool centerPlayerMode {
        set {
            if (value) {
                followAction = centerPlayer;
            } else {
                followAction = centerXLazyY;
            }
        }
    }

    private void centerPlayer() {
        Vector3 pos = transform.position;
        pos.x = player.transform.position.x;
        pos.y = player.transform.position.y;
        transform.position = pos;
    }

    private Action followAction;


	private void Awake()
	{
		lastGroundPointYs = new RingBuffer<float>(24);
		viewportHelper = ComponentHelper.FindAnywhereOrAdd<ViewPortHelper>();
	}

	private void Start()
	{
        centerPlayerMode = false;
        abyss = FindObjectOfType<Abyss>();
		smoothGroundPoint = player.groundPoint;
        viewportHelper = FindObjectOfType<ViewPortHelper>();
	}

	private void FixedUpdate()
	{
		lerpGroundPoint();
        followAction();
		//centerXLazyY();
        stayAboveTheAbyss();
	}

    public void zoomToContainWidth(float worldScaleWidth) {
        float propoX = viewportHelper.viewPortGlobal.size.x / worldScaleWidth;
        Camera.main.orthographicSize /= propoX;
    }

    private void stayAboveTheAbyss() {
        float dif = abyss.surfaceY - viewportHelper.viewPortGlobal.min.y;
        if(dif > 0f) {
            Vector3 pos = transform.position;
            pos.y += dif;
            transform.position = pos;
        }
    }

    private void lerpGroundPoint()
	{
		smoothGroundPoint.x = player.groundPoint.x;
		lastGroundPointYs.push(player.groundPoint.y);
		float avgY = 0;
		foreach(float y in lastGroundPointYs.getValues())
		{
			avgY += y;
		}
		avgY /= lastGroundPointYs.size;

		smoothGroundPoint.y = Mathf.Lerp(smoothGroundPoint.y, avgY, .2f);
	}

	private void centerXLazyY()
	{
		float xx = Mathf.Lerp(transform.position.x, target.position.x, 4f * smoothing * Time.deltaTime);
		VectorXY edgeCatchup = viewportHelper.normalizedCenteredness01(target.position);
		float yy = Mathf.Lerp(target.position.y, smoothGroundPoint.y, .5f);

		yy = Mathf.Lerp(transform.position.y, yy, 4f * smoothing * edgeCatchup.y * edgeCatchup.y * catchupWhenNearEdgeSpeed / yLaziness * Time.deltaTime);
		transform.position = new Vector3(xx, yy, transform.position.z);
	}


}
