using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Services.AssetManagement {
    [CreateAssetMenu(menuName = "Config/AddressableRefConfig")]
    public class AddressableRefConfig : ScriptableObject {
            public AssetReferenceGameObject waterTile;
            public AssetReferenceGameObject waterBackground;
            public AssetReferenceGameObject boat;

            public AssetLabelReference terrainTilesLabel;
            public AssetLabelReference propsLabel;
    }
}