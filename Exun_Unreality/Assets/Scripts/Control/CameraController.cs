using System.Collections.Generic;
using System.Linq;
using Game.Networking;
using UnityEngine;

namespace Game.Control
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance;
        public CharacterManager[] targets;
        
        [SerializeField] private Vector3 offset = Vector3.zero;
        [SerializeField] private float smooth = .4f;

        private Vector3 velocity = Vector3.zero;

        [SerializeField] private float minZoom = 40f;
        [SerializeField] private float maxZoom = 10f;
        [SerializeField] private float zoomLimiter = 10f;

        private Camera cam = null;

        private void Awake()
        {
            if (Instance != null) Destroy(this);
            Instance = this;
        }

        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
            cam = GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            targets = GameManager.players.Values.ToArray();
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
                centerPoint = targets[0].transform.position;
            else
                centerPoint = bounds.center;
            
            Vector3 newPos = centerPoint + offset;
            transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smooth);
        }
        
        private Bounds GetBounds()
        {
            var bounds = new Bounds(targets[0].transform.position, Vector3.zero);
            foreach (var target in targets)
            {
                bounds.Encapsulate(target.transform.position);
            }

            return bounds;
        }
    }
}
