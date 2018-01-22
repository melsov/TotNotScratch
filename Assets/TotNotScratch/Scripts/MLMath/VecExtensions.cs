using System;
using UnityEngine;

namespace VctorExtensions
{
    public static class VctorExtensions
    {
		//Vec2 extensions
		public static Vector3 toVector3(this Vector2 v, float z) { return new Vector3(v.x, v.y, z); }

		public static Vector3 toVector3(this Vector2 v) { return v.toVector3(0f); }

		public static float dot(this Vector2 v, Vector2 other) { return v.x * other.x + v.y * other.y; }

		public static Vector2 scale(this Vector2 v, Vector2 other) { return new Vector2(v.x * other.x, v.y * other.y); }

		//Vec3 extensions
		public static Vector2 xy(this Vector3 v) { return new Vector2(v.x, v.y); }



	}
}
