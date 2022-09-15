using CSLShowCommuterDestination.Graph;
using System.Collections;
using UnityEngine;

namespace CSLShowCommuterDestination.Unity
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(LineRenderer))]
    public class DestinationJourneyRenderer : MonoBehaviour
    {
        public Color StopColor;
        private Color lastRenderedStopColor;

        public int Popularity;
        private int lastRenderedPopularity;

        private Vector3 lastRenderedOrigin;
        private Vector3 lastRenderedDestination;

        private MeshRenderer meshRenderer;
        private LineRenderer lineRenderer;

        public static GameObject Create(GameObject prefab, DestinationJourney journey, Color color)
        {
            var gameObject = Instantiate(prefab, journey.destination, Quaternion.identity);

            var renderer = gameObject.AddComponent<DestinationJourneyRenderer>();
            renderer.Popularity = journey.popularity;
            renderer.StopColor = color;

            return gameObject;
        }

        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        void Update()
        {
            if (UpdateRequired() == false)
            {
                return;
            }

            this.RenderJourney();
        }

        private bool UpdateRequired()
        {
            return (
                StopColor != lastRenderedStopColor
                || Popularity != lastRenderedPopularity
                || gameObject.transform.parent.position != lastRenderedOrigin
                || gameObject.transform.position != lastRenderedDestination
            );
        }

        private void RenderJourney()
        {
            if (lineRenderer == null || Popularity == 0 || StopColor == null)
            {
                return;
            }

            lineRenderer.SetPositions(new Vector3[] { gameObject.transform.parent.position, gameObject.transform.position });


            //var SCALE_FACTOR = 4f;
            //var SCALE_FACTOR_LINE = 0.6f;
            //var MINIMUM_SIZE_LINE = 15;
            //var MINIMUM_SIZE_SPHERE = 40;

            /// todo these need further tweaking

            var SCALE_FACTOR = 4f;
            var SCALE_FACTOR_LINE = 0.3f;
            var MINIMUM_SIZE_LINE = 1;
            var MINIMUM_SIZE_SPHERE = 10;

            var popularitySize = ((Popularity - 1) * SCALE_FACTOR) + MINIMUM_SIZE_SPHERE;

            lineRenderer.startWidth = (popularitySize * SCALE_FACTOR_LINE) + MINIMUM_SIZE_LINE;
            lineRenderer.endWidth = (popularitySize * SCALE_FACTOR_LINE) + MINIMUM_SIZE_LINE;

            var destinationSphereSize = popularitySize;

            gameObject.transform.localScale = new Vector3(destinationSphereSize, destinationSphereSize, destinationSphereSize);

            var blobColor = StopColor;
            var lineColor = Color.Lerp(blobColor, IsDark(blobColor) ? Color.white : Color.black, 0.3f);

            lineRenderer.material.color = lineColor;
            meshRenderer.material.color = blobColor;

            lastRenderedPopularity = Popularity;
            lastRenderedOrigin = gameObject.transform.parent.position;
            lastRenderedDestination = gameObject.transform.position;
            lastRenderedStopColor = StopColor;
        }

        private bool IsDark(Color color)
        {
            return (color.r + color.g + color.b) / 3 < 0.5;
        }
    }
}
