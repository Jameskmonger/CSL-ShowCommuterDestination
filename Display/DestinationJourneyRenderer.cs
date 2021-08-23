using CSLShowCommuterDestination.Graph;
using UnityEngine;

namespace CSLShowCommuterDestination.Display
{
    [RequireComponent(typeof(LineRenderer))]
    public class DestinationJourneyRenderer : MonoBehaviour
    {
        private DestinationJourney journey;
        private LineRenderer lineRenderer;

        public static GameObject Create(DestinationJourney journey)
        {
            var gameObject = new GameObject("DestinationJourneyRenderer", typeof(DestinationJourneyRenderer));

            gameObject.GetComponent<DestinationJourneyRenderer>().SetJourney(journey);

            return gameObject;
        }

        void Awake()
        {
        }

        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();

            lineRenderer.material = new Material(Shader.Find("Diffuse"));
            lineRenderer.material.color = Color.magenta;
            lineRenderer.useWorldSpace = true;

            this.RenderJourney();
        }

        void Update()
        {
        }

        private void SetJourney(DestinationJourney journey)
        {
            this.journey = journey;

            this.RenderJourney();
        }

        private void RenderJourney()
        {
            if (lineRenderer == null || journey == null)
            {
                return;
            }

            var heightOffset = new Vector3(0, 50f, 0);

            lineRenderer.SetPositions(new Vector3[] { journey.origin.position + heightOffset, journey.destination + heightOffset });
            lineRenderer.startWidth = (journey.popularity * 2f) + 3;
            lineRenderer.endWidth = (journey.popularity * 2f) + 3;
        }
    }
}
