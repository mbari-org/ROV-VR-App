using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour
{
    public float m_DefaultLength = 5.0f;
    public GameObject m_Dot;
    public VRInputModule m_InputModule;

    private LineRenderer m_LineRenderer = null;

    private void Awake()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        UpdateLine();
    }

    private void UpdateLine()
    {
        // Use default or distance
        PointerEventData data = m_InputModule.GetData();
        // If we aren't currently hitting anything with our graphic raycaster then the distance from pointer to canvas will be 0
        // If this it is 0, we want the value to be set to the default length. If not, then adjust pointer length as necessary (using ternary operator)
        float targetLength = data.pointerCurrentRaycast.distance == 0 ? m_DefaultLength : data.pointerCurrentRaycast.distance; 
        // float targetLength = m_DefaultLength; // Default
      
        // Raycast 
        RaycastHit hit = CreateRaycast(targetLength);

        // Default end position (where to put dot if no hits)
        Vector3 endPosition = transform.position + (transform.forward * targetLength);
        
        // Or based on hit 
        if (hit.collider != null)
            endPosition = hit.point;

        // Set position of the dot
        m_Dot.transform.position = endPosition;

        // Set positions of linerenderer
        m_LineRenderer.SetPosition(0, transform.position);
        m_LineRenderer.SetPosition(1, endPosition);

    }

    private RaycastHit CreateRaycast(float length)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, m_DefaultLength);

        return hit;
    }
}
