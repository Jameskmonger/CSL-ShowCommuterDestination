using CSLShowCommuterDestination.Graph;
using System.Collections.Generic;
using UnityEngine;

namespace CSLShowCommuterDestination.Display
{
    public class DestinationGraphRenderer : MonoBehaviour
    {
        private DestinationGraph graph = null;
        private readonly List<GameObject> stopRenderers = new List<GameObject>();

        public static void Destroy()
        {
            var instance = GameObject.Find("DestinationGraphRenderer");

            if (instance == null)
            {
                return;
            }

            Destroy(instance);
        }

        public static GameObject Create(DestinationGraph graph)
        {
            Destroy();

            var gameObject = new GameObject("DestinationGraphRenderer", typeof(DestinationGraphRenderer));

            gameObject.GetComponent<DestinationGraphRenderer>().SetGraph(graph);

            return gameObject;
        }

        void Awake()
        {
        }

        void Start()
        {
            RenderGraph();
        }

        void Update()
        {
        }

        void OnDestroy()
        {
            Debug.Log("DestinationGraphRenderer destroyed");

            ClearRenderers();
        }

        private void SetGraph(DestinationGraph graph)
        {
            this.graph = graph;

            RenderGraph();
        }

        private void RenderGraph()
        {
            if (graph == null)
            {
                return;
            }

            ClearRenderers();

            foreach (var stop in graph.stops) {
                stopRenderers.Add(DestinationStopRenderer.Create(stop));
            }
        }

        private void ClearRenderers()
        {
            foreach (var renderer in stopRenderers)
            {
                Destroy(renderer);
            }

            stopRenderers.Clear();
        }
    }
}
