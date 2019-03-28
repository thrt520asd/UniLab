using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EventMgr : MonoBehaviour
{
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            var hit = GetRayResult();
            for (int i = 0; i < hit.Length; i++)
            {
                IPointerDown down = hit[i].collider.GetComponent<IPointerDown>();
                down.OnPointerDown(hit[i]);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            var hit = GetRayResult();
            for (int i = 0; i < hit.Length; i++)
            {
                IPointerUp down = hit[i].collider.GetComponent<IPointerUp>();
                down.OnPointerUp(hit[i]);
            }
        }
    }

    private RaycastHit[] GetRayResult()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hit = Physics.RaycastAll(ray);
        return hit;
    }

    void OnDrawGizmos()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
            if (hit.collider != null)
            {
                Debug.DrawLine(Camera.main.transform.position , hit.point);
            }
        }

    }
}
