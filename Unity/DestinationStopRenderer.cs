using CSLShowCommuterDestination.Graph;
using System.Collections.Generic;
using UnityEngine;

namespace CSLShowCommuterDestination.Unity
{
    public class DestinationStopRenderer : MonoBehaviour
    {
        public GameObject JourneyRendererPrefab;

        public IEnumerable<DestinationJourney> Journeys;
        private IEnumerable<DestinationJourney> lastRenderedJourneys;

        public Color StopColor;
        private Color lastRenderedStopColor;

        private readonly List<GameObject> journeyRenderers = new List<GameObject>();

        public static GameObject Create(GameObject stopRendererPrefab, GameObject journeyRendererPrefab, DestinationStop stop, Color color)
        {
            var gameObject = Instantiate(stopRendererPrefab, stop.position, Quaternion.identity);

            var renderer = gameObject.AddComponent<DestinationStopRenderer>();

            gameObject.transform.position = stop.position;
            renderer.JourneyRendererPrefab = journeyRendererPrefab;
            renderer.Journeys = stop.GetJourneys();
            renderer.StopColor = color;

            return gameObject;
        }

        void Update()
        {
            if (UpdateRequired() == false)
            {
                return;
            }

            RenderStop();
        }

        void OnDestroy()
        {
            Debug.Log("DestinationStopRenderer destroyed");

            ClearRenderers();
        }

        private bool UpdateRequired()
        {
            return (
                Journeys != lastRenderedJourneys
                || StopColor != lastRenderedStopColor
            );
        }

        private void RenderStop()
        {
            if (Journeys == null || StopColor == null)
            {
                return;
            }

            ClearRenderers();

            foreach (var journey in Journeys)
            {
                var renderer = DestinationJourneyRenderer.Create(JourneyRendererPrefab, journey, StopColor);

                renderer.transform.parent = gameObject.transform;

                journeyRenderers.Add(renderer);
            }

            lastRenderedJourneys = Journeys;
            lastRenderedStopColor = StopColor;

            var capsule = transform.Find("DestinationStopCapsule");
            capsule.GetComponent<MeshRenderer>().material.color = StopColor;
        }

        private void ClearRenderers()
        {
            foreach (var renderer in journeyRenderers)
            {
                Destroy(renderer);
            }

            journeyRenderers.Clear();
        }
    }
}
