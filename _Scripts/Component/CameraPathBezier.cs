using UnityEngine;
using System.Collections.Generic;

public class CameraPathBezier : MonoBehaviour
{
    public List<Transform> controlPoints;  // 用于控制贝塞尔曲线的多个点
    public float speed = 1.0f;
    private float t = 0.0f;

    void Update()
    {
        // 通过贝塞尔曲线计算摄像机位置
        t += Time.deltaTime * speed;

        if (t > 1.0f)
        {
            t = 1.0f;
        }

        Vector3 position = CalculateBezierPoint(t, controlPoints);
        transform.position = position;
    }

    // 计算贝塞尔曲线上的点，支持多个控制点
    private Vector3 CalculateBezierPoint(float t, List<Transform> points)
    {
        int n = points.Count;

        if (n == 1)
        {
            // 只有一个控制点时，返回该点
            return points[0].position;
        }

        // 递归地将控制点对合并，直到剩下一个点
        List<Transform> newPoints = new List<Transform>();

        for (int i = 0; i < n - 1; i++)
        {
            // 在每对相邻点之间进行插值
            Vector3 newPoint = Vector3.Lerp(points[i].position, points[i + 1].position, t);
            newPoints.Add(new GameObject().transform); // 创建新的临时 Transform 作为插值结果
            newPoints[i].position = newPoint;
        }

        // 递归调用，继续减少控制点
        return CalculateBezierPoint(t, newPoints);
    }
}