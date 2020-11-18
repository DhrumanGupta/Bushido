using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Game.Control
{
    public class CameraController : MonoBehaviour
    {
        private Transform[] targets = null;
        
        [SerializeField] private Vector3 offset = Vector3.zero;
        [SerializeField] private float smooth = .4f;

        private Vector3 velocity = Vector3.zero;

        [SerializeField] private float minZoom = 40f;
        [SerializeField] private float maxZoom = 10f;
        [SerializeField] private float zoomLimiter = 10f;

        private Camera cam = null;

        private void Start()
        {
            Application.targetFrameRate = 60;
            cam = GetComponent<Camera>();
            
            var objectsFound = GameObject.FindGameObjectsWithTag("Player");
            targets = new Transform[objectsFound.Length];

            for (int i = 0; i < objectsFound.Length; i++)
            {
                targets[i] = objectsFound[i].transform;
            }
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (targets.Length == 0) return;

            var bounds = GetBounds();

            Move(bounds);
            Zoom(bounds);
        }

        private void Zoom(Bounds bounds)
        {
            float greatestDistance = bounds.size.x;

            float newZoom = Mathf.Lerp(maxZoom, minZoom, greatestDistance / zoomLimiter);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime * 1.5f);
        }

        private void Move(Bounds bounds)
        {
            Vector3 centerPoint = Vector3.zero;
            
            if (targets.Length == 1) 
                centerPoint = targets[0].position;
            else
                centerPoint = bounds.center;
            
            Vector3 newPos = centerPoint + offset;
            transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smooth);
        }
        
        private Bounds GetBounds()
        {
            var bounds = new Bounds(targets[0].position, Vector3.zero);
            foreach (var target in targets)
            {
                bounds.Encapsulate(target.position);
            }

            return bounds;
        }
    }
}
