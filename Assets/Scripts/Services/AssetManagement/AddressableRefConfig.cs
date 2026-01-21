using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Services.AssetManagement {
    [CreateAssetMenu(menuName = "Config/AddressableRefConfig")]
    public class AddressableRefConfig : ScriptableObject {
            public AssetReferenceGameObject waterTile;
            public AssetReferenceGameObject waterBackground;
            public AssetLabelReference terrainTilesLabel;
    }
}