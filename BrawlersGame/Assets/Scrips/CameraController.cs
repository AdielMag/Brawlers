using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    private void Awake()
    {
        instance = this;
    }

    public Transform[] playersTransforms;
    public float minDistance = 10;
    public float yOffset = 2;

    float xMin, xMax, yMin, yMax;

    Vector3 targetPos;

    private void LateUpdate()
    {
        xMin = xMax = playersTransforms[0].position.x;
        yMin = yMax = playersTransforms[0].position.y;

        for (int i = 0; i < playersTransforms.Length; i++)
        {
            if (playersTransforms[i].position.x < xMin)
                xMin = playersTransforms[i].position.x;
            if (playersTransforms[i].position.x > xMax)
                xMax = playersTransforms[i].position.x;
            if (playersTransforms[i].position.y < yMin)
                yMin = playersTransforms[i].position.y;
            if (playersTransforms[i].position.y > yMax)
                yMax = playersTransforms[i].position.y;
        }

        float distance = (xMax - xMin) / 1.3f > minDistance ? (xMax - xMin) / 1.3f : minDistance;
        distance = (yMax - yMin) * 1.65f > distance ? (yMax - yMin) * 1.65f : distance;

        targetPos = new Vector3((xMin + xMax) / 2, (yMin + yMax) / 2 + yOffset, -distance);

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5);
    }
}
