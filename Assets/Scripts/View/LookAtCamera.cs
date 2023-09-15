using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    void Awake()
    {
        _camera = Camera.main;
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(_camera.transform.forward);
    }
}
