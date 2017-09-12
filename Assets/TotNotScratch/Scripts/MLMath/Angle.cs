using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Angle
{
    public static float angle(Vector2 p_vector2) {
        float temp = p_vector2.y;
        p_vector2.y = p_vector2.x;
        p_vector2.x = temp;
        if (p_vector2.x < 0) {
            return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
        } else {
            return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
        }
    }

    public static float angle(Vector3 p_vector2) {
        float temp = p_vector2.y;
        p_vector2.y = p_vector2.x;
        p_vector2.x = temp;
        if (p_vector2.x < 0) {
            return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
        } else {
            return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
        }
    }

    //public static float angleMaybeFlipped(Vector2 p_vector2) {
    //    if (p_vector2.x < 0) {
    //         return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
    //    } else {
    //        return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
    //    }
    // }


}
