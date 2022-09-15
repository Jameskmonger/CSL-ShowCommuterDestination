using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CSLShowCommuterDestination.Content
{
    public class PrefabLoader : MonoBehaviour
    {
        IEnumerator Start()
        {
            var assetsUri = "file:///" + Mod.ModPath.Replace("\\", "/") + "/destinationgraphrenderer";
            Debug.Log("~ CommuterDestinationPrefabLoader loading from: " + assetsUri);

            var www = new WWW(assetsUri);

            yield return www;

            var assetBundle = www.assetBundle;

            CheckAssetBundle(assetBundle, assetsUri);

            var assets = assetBundle.LoadAllAssets().OfType<GameObject>().ToArray();

            Debug.Log("~ CommuterDestinationPrefabLoader loaded " + assets.Length + " assets");

            var graphRenderer = GetPrefab(assets, "DestinationGraphRenderer");
            var stopRenderer = GetPrefab(assets, "DestinationStopRenderer");
            var journeyRenderer = GetPrefab(assets, "DestinationJourneyRenderer");

            Mod.Prefab_DestinationGraphRenderer = graphRenderer;
            Mod.Prefab_DestinationStopRenderer = stopRenderer;
            Mod.Prefab_DestinationJourneyRenderer = journeyRenderer;

            assetBundle.Unload(false);

            Debug.Log("~ CommuterDestinationPrefabLoader loaded");
        }

        private GameObject GetPrefab(GameObject[] gameObjects, string assetName)
        {
            var prefab = gameObjects.FirstOrDefault(a => a.name == assetName);

            CheckPrefab(prefab);

            prefab.name = assetName + "_Prefab";

            prefab.transform.parent = gameObject.transform;

            return prefab;
        }

        private void CheckAssetBundle(AssetBundle assetBundle, string assetsUri)
        {
            if (assetBundle == null)
            {
                Debug.Log("AssetBundle with URI '" + assetsUri + "' could not be loaded");
            }
#if (false)
            else
            {
                Debug.Log("Mod Assets URI: " + assetsUri);
                foreach (string asset in assetBundle.GetAllAssetNames())
                {
                    Debug.Log("Asset: " + asset);
                }
            }
#endif
        }

        private void CheckPrefab(GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.Log("Prefab not loaded");
            }
            else
            {
                Debug.Log("Loaded prefab: " + prefab.name);
            }
        }
    }
}
