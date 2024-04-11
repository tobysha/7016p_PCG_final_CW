using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraFollow : MonoBehaviour
{
    public float padding = 1.0f;
    public MeshRenderer targetMeshRenderer;
    private void Update()
    {
        GameObject targetObject = GameObject.Find("CaveMesh");
        if (targetObject == null)
        {
            Debug.LogWarning("no gameObject£¡");
            return;
        }
        targetMeshRenderer = targetObject.GetComponent<MeshRenderer>();

        if (targetMeshRenderer == null)
        {
            Debug.LogWarning("no targets£¡");
            return;
        }


        Bounds bounds = targetMeshRenderer.bounds;


        float cameraDistance = bounds.size.y / (2 * Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView / 2));

   
        transform.position = new Vector3(bounds.center.x, bounds.center.y, -cameraDistance);


    }

}
