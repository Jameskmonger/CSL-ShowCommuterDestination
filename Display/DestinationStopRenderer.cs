using CSLShowCommuterDestination.Graph;
using System.Collections.Generic;
using UnityEngine;

namespace CSLShowCommuterDestination.Display
{
    public class DestinationStopRenderer : MonoBehaviour
    {
        private DestinationStop stop = null;
        private readonly List<GameObject> journeyRenderers = new List<GameObject>();

        public static GameObject Create(DestinationStop stop)
        {
            var gameObject = new GameObject("DestinationStopRenderer", typeof(DestinationStopRenderer));

            gameObject.GetComponent<DestinationStopRenderer>().SetStop(stop);

            return gameObject;
        }

        void Awake()
        {
        }

        void Start()
        {
            RenderStop();
        }

        void Update()
        {
        }

        void OnDestroy()
        {
            Debug.Log("DestinationStopRenderer destroyed");

            ClearRenderers();
        }

        private void SetStop(DestinationStop stop)
        {
            this.stop = stop;

            RenderStop();
        }

        private void RenderStop()
        {
            if (stop == null)
            {
                return;
            }

            ClearRenderers();

            foreach (var journey in stop.GetJourneys()) {
                journeyRenderers.Add(DestinationJourneyRenderer.Create(journey));
            }
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
