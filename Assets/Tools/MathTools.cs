using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathTools  {

    public static Vector3 RotateVectorByQuat(Quaternion lhs, Vector3 rhs)
    {
        float x = lhs.x * 2;
        float y = lhs.y * 2;
        float z = lhs.z * 2;
        float xx = lhs.x * x;
        float yy = lhs.y * y;
        float zz = lhs.z * z;
        float xy = lhs.x * y;
        float xz = lhs.x * z;
        float yz = lhs.y * z;
        float wx = lhs.w * x;
        float wy = lhs.w * y;
        float wz = lhs.w * z;
        Vector3 res;
        res.x = (1.0f - (yy + zz)) * rhs.x + (xy - wz) * rhs.y + (xz + wy) * rhs.z;
        res.y = (xy + wz) * rhs.x + (1.0f - (xx + zz)) * rhs.y + (yz - wx) * rhs.z;
        res.z = (xz - wy) * rhs.x + (yz + wx) * rhs.y + (1.0f - (xx + yy)) * rhs.z;
        return res;
    }
}
