using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlCameraFollow : MonoBehaviour
{
	private void centerXLazyY()
	{
		float xx = Mathf.Lerp(transform.position.x, target.position.x, 4f * smoothing * Time.deltaTime) + offset.x;
		VectorXY edgeCatchup = viewportHelper.normalizedCenteredness(target.position);

		float yy = Mathf.Lerp(transform.position.y, target.position.y, 4f * smoothing * edgeCatchup.y * edgeCatchup.y * catchupWhenNearEdgeSpeed / yLaziness * Time.deltaTime) + offset.y;
		transform.position = new Vector3(xx, yy, transform.position.z);
	}
}
