using System;
using System.Threading;
using Domain.Grid;
using Services.AssetManagement;
using Services.Map;
using UnityEngine;

namespace Presentation.Bootstrap {
    public class GameBootstrapper : MonoBehaviour {
        [SerializeField] TextAsset map;
        [SerializeField] AddressableRefConfig addressableRefConfig;
        [SerializeField] HexLayout hexLayout;
        
        AddressableAssetProvider assetProvider;
        CancellationTokenSource cts;

       async void Start() {
            var mapParser = new MapParser();
            var hexGridData = mapParser.Parse(map);
            cts = new CancellationTokenSource();
            assetProvider = new AddressableAssetProvider(addressableRefConfig);
            var gridBuilder = new GridBuilder(hexLayout, hexGridData, assetProvider);
           
            try {
                await assetProvider.InitializeAsync(cts.Token);
                await gridBuilder.BuildGrid(cts.Token);
            }
            catch (OperationCanceledException _) {
                Debug.Log("Operation cancelled");
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
       }

        void OnDestroy() {
            cts.Cancel();
            assetProvider.ReleaseAll();
            cts.Dispose();
        }
    }
}