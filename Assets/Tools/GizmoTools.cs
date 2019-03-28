using UnityEngine;

public class GizmoTools
{
    public static void DrawCube(Vector3 pos, Vector3 size , Quaternion rotation ,  Color color )
    {
        float halfX = size.x / 2;
        float halfY = size.y / 2;
        float halfZ = size.z / 2;
        
        
        var leftFrontUp = pos + MathTools.RotateVectorByQuat(rotation ,  new Vector3(-halfX, halfY, halfZ));
        var rightFrontUp = pos + MathTools.RotateVectorByQuat(rotation , new Vector3(halfX, halfY, halfZ));
        var rightBackUp = pos + MathTools.RotateVectorByQuat(rotation , new Vector3(halfX, halfY, -halfZ));
        var leftBackUp = pos + MathTools.RotateVectorByQuat(rotation , new Vector3(-halfX, halfY, -halfZ));

        var leftFrontDown = pos + MathTools.RotateVectorByQuat(rotation, new Vector3(-halfX, -halfY, halfZ));
        var rightFrontDown = pos + MathTools.RotateVectorByQuat(rotation, new Vector3(halfX, -halfY, halfZ));
        var rightBackDown = pos + MathTools.RotateVectorByQuat(rotation, new Vector3(halfX, -halfY, -halfZ));
        var leftBackDown = pos + MathTools.RotateVectorByQuat(rotation, new Vector3(-halfX, -halfY, -halfZ));
        Debug.DrawLine(leftFrontUp, leftBackUp, color);
        Debug.DrawLine(leftFrontUp, rightFrontUp, color);
        Debug.DrawLine(leftFrontUp, leftFrontDown, color);
        Debug.DrawLine(rightFrontDown, rightFrontUp, color);
        Debug.DrawLine(rightFrontDown, leftFrontDown, color);
        Debug.DrawLine(rightFrontDown, rightBackDown, color);
        Debug.DrawLine(rightBackUp, rightBackDown, color);
        Debug.DrawLine(rightBackUp, rightFrontUp, color);
        Debug.DrawLine(rightBackUp, leftBackUp, color);
        Debug.DrawLine(leftBackDown, leftBackUp, color);
        Debug.DrawLine(leftBackDown, leftFrontDown, color);
        Debug.DrawLine(leftBackDown, rightBackDown, color);

    }

   
}
