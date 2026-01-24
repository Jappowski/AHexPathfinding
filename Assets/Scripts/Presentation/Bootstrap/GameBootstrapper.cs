using System;
using System.Threading;
using Domain.Grid;
using Presentation.Boat;
using Presentation.Camera;
using Services.AssetManagement;
using Services.Map;
using Services.Player;
using UnityEngine;

namespace Presentation.Bootstrap {
    public class GameBootstrapper : MonoBehaviour {
        [SerializeField] TextAsset map;
        [SerializeField] AddressableRefConfig addressableRefConfig;
        [SerializeField] BoatNavigationController boatNavigationController;
        [SerializeField] CameraSmoothFollow cameraSmoothFollow;
        [SerializeField] HexLayout hexLayout;
        
        AddressableAssetProvider assetProvider;
        CancellationTokenSource cts;

       async void Start() {
            var mapParser = new MapParser();
            var hexGridData = mapParser.Parse(map);
            cts = new CancellationTokenSource();
            assetProvider = new AddressableAssetProvider(addressableRefConfig);
            var gridBuilder = new GridBuilder(hexLayout, hexGridData, assetProvider);
            var playerSpawner = new PlayerSpawner(assetProvider, hexGridData, hexLayout);
            try {
                await assetProvider.InitializeAsync(cts.Token);
                await gridBuilder.BuildGrid(cts.Token);
                await gridBuilder.PopulateTerrainWithProps(cts.Token);
                var player = await playerSpawner.SpawnPlayer(cts.Token);
                boatNavigationController.Init(hexGridData, player, hexLayout);
                cameraSmoothFollow.SetTarget(player.transform);
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