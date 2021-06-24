using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CSLShowCommuterDestination.Content
{
    public class DestinationRenderer : MonoBehaviour
    {
        //public Vector3 position = new Vector3(-750, 150, -620);
        public Vector3? position = null;
        int popularity = 0;

        float theta_scale = 0.005f;        //Set lower to add more points
        int size; //Total number of points in circle
        float radius;
        LineRenderer lineRenderer;


        void Awake()
        {
            Debug.Log("DTR awake");
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Diffuse"));
            lineRenderer.material.color = Color.magenta;
            lineRenderer.startWidth = 6f;
            lineRenderer.endWidth = 6f;
        }

        void Start()
        {
        }

        void Update()
        {
            if (!this.position.HasValue || !StopDestinationInfoPanel.instance || !StopDestinationInfoPanel.instance.isVisible)
            {
                return;
            }

            var cameraSize = Singleton<CameraController>.instance.m_currentSize;

            size = (int)((2.0f * Mathf.PI) / theta_scale) + 1;
            lineRenderer.positionCount = size;
            radius = (10 + ((popularity - 1) * 6)) * (cameraSize * StopDestinationInfoPanel.instance.m_heightScaleFactor * 0.1f);

            float theta = 0f;
            for (int i = 0; i < size; i++)
            {
                theta += (2.0f * Mathf.PI * theta_scale);
                float x = radius * Mathf.Cos(theta);
                float z = radius * Mathf.Sin(theta);
                x += position.Value.x;
                z += position.Value.z;
                Vector3 pos = new Vector3(x, position.Value.y, z);
                lineRenderer.SetPosition(i, pos);
            }
        }

        public void SetPosition(Vector3 newPos, int popularity)
        {
            this.position = new Vector3(newPos.x, newPos.y + 100, newPos.z);
            this.popularity = popularity;
        }
    }
}
