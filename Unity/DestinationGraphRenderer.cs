using CSLShowCommuterDestination.Graph;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CSLShowCommuterDestination.Unity
{
    public class DestinationGraphRenderer : MonoBehaviour
    {
        public GameObject StopRendererPrefab;
        public GameObject JourneyRendererPrefab;

        public DestinationGraph Graph;
        private DestinationGraph lastRenderedGraph;

        private readonly List<GameObject> stopRenderers = new List<GameObject>();

        // order the colours randomly to start with
        private static List<Color> colors = new List<Color>() {
            new Color32(0xFF, 0xFF, 0x26, 0xFF),
            new Color32(0x9C, 0xF8, 0xA0, 0xFF),
            new Color32(0x16, 0x68, 0x1A, 0xFF),
            new Color32(0xFF, 0xA5, 0x00, 0xFF),
            new Color32(0xFF, 0x42, 0x21, 0xFF),
            new Color32(0xBF, 0x63, 0x98, 0xFF),
            new Color32(0x4B, 0x15, 0xAA, 0xFF),
            new Color32(0xD9, 0xB5, 0xF8, 0xFF),
            new Color32(0x23, 0x00, 0xFF, 0xFF),
            new Color32(0x36, 0x78, 0xE0, 0xFF),
            new Color32(0x46, 0x74, 0xBF, 0xFF),
            new Color32(0x50, 0xA6, 0xB1, 0xFF)
        };

        public static void Destroy()
        {
            var instance = GameObject.Find("DestinationGraphRenderer");

            if (instance == null)
            {
                return;
            }

            Destroy(instance);
        }

        public static GameObject Create(GameObject graphRendererPrefab, GameObject stopRendererPrefab, GameObject journeyRendererPrefab, DestinationGraph graph)
        {
            Destroy();

            var gameObject = Instantiate(graphRendererPrefab, Vector3.zero, Quaternion.identity);

            gameObject.name = "DestinationGraphRenderer";

            var renderer = gameObject.AddComponent<DestinationGraphRenderer>();
            renderer.StopRendererPrefab = stopRendererPrefab;
            renderer.JourneyRendererPrefab = journeyRendererPrefab;
            renderer.Graph = graph;

            return gameObject;
        }

        void Awake()
        {
        }

        void Start()
        {
            //RenderGraph();
        }

        void Update()
        {
            if (UpdateRequired() == false)
            {
                return;
            }

            RenderGraph();
        }

        void OnDestroy()
        {
            Debug.Log("DestinationGraphRenderer destroyed");

            ClearRenderers();
        }

        private bool UpdateRequired()
        {
            if (Graph != lastRenderedGraph)
            {
                return true;
            }

            return false;
        }

        private void RenderGraph()
        {
            if (Graph == null)
            {
                return;
            }

            ClearRenderers();

            var colorsToUse = GetShuffledColors();

            foreach (var stop in Graph.stops)
            {
                Color color;

                if (stop.color.HasValue)
                {
                    color = stop.color.Value;
                } else
                {
                    color = colorsToUse[colorsToUse.Count - 1];
                    colorsToUse.RemoveAt(colorsToUse.Count - 1);

                    if (colorsToUse.Count == 0)
                    {
                        colorsToUse = GetShuffledColors();
                    }
                }

                var renderer = DestinationStopRenderer.Create(StopRendererPrefab, JourneyRendererPrefab, stop, color);

                renderer.transform.parent = gameObject.transform;

                stopRenderers.Add(renderer);
            }

            lastRenderedGraph = Graph;
        }

        private List<Color> GetShuffledColors()
        {
            return colors.OrderBy(a => System.Guid.NewGuid()).ToList();
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
