using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] float deviation;

    private void Update()
    {
        float x = (Input.mousePosition.x / Screen.height) * 2 - 1;
        float y = (Input.mousePosition.y / Screen.height) * 2 - 1;

        transform.localPosition = new Vector3(x,y) * deviation + Vector3.forward*transform.localPosition.z;
    }
}
