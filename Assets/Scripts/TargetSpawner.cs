using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public GameObject targetPrefab;
    public int targetCount = 6;
    public float radius = 3f; // how far around the circle
    public float height = 1.5f; // vertical position
    public GameObject shooter;

    void Start()
    {
        // Get the camera forward direction, but ignore tilt
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0; // keep circle flat
        cameraForward.Normalize();

        // Calculate center point of circle in front of camera
        //Vector3 circleCenter = Camera.main.transform.position + cameraForward * radius;
        Vector3 circleCenter = shooter.transform.position;


        // Spawn targets evenly spaced around a circle
        for (int i = 0; i < targetCount; i++)
        {
            float angle = i * Mathf.PI * 2f / targetCount;
            Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, height, Mathf.Sin(angle) * radius);
            Vector3 spawnPos = circleCenter + offset;

            Instantiate(targetPrefab, spawnPos, Quaternion.LookRotation(circleCenter - spawnPos));
        }
    }
}
