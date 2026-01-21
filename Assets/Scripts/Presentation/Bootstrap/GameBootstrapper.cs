using System.Threading;
using Domain.Grid;
using Services.Addressables;
using Services.Map;
using UnityEngine;

namespace Presentation.Bootstrap {
    public class GameBootstrapper : MonoBehaviour {
        [SerializeField] TextAsset map;
        [SerializeField] HexLayout hexLayout;
        
       async void Start() {
            var mapParser = new MapParser();
            var hexGridData = mapParser.Parse(map);
            var assetProvider = new AddressableAssetProvider();
            await assetProvider.InitializeAsync();
            var gridBuilder = new GridBuilder(hexLayout, hexGridData, assetProvider);
            await gridBuilder.BuildGrid(CancellationToken.None);
        }
    }
}