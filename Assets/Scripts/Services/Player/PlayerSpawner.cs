using System.Threading;
using System.Threading.Tasks;
using Domain.AssetManagement;
using Domain.Grid;
using Presentation.Boat;
using Services.AssetManagement;
using UnityEngine;

namespace Services.Player {
    public class PlayerSpawner {
        IGameAssetProvider assetProvider;
        HexGridData hexGridData;
        HexLayout hexLayout;
        
        public PlayerSpawner(IGameAssetProvider assetProvider, HexGridData hexGridData, HexLayout hexLayout) {
            this.assetProvider = assetProvider;
            this.hexGridData = hexGridData;
            this.hexLayout = hexLayout;     
        }
        
        public async Task<BoatComponent> SpawnPlayer(CancellationToken cancellationToken) {
            var potentialSpawnTiles = hexGridData.GetAllWaterTiles();
            var spawnTile = potentialSpawnTiles[Random.Range(0, potentialSpawnTiles.Length)];
            var spawnPosition = GridUtils.HexToWorld(hexLayout, spawnTile);
            var boatAsset = await assetProvider.LoadAsync<GameObject>(AssetKey.Boat, cancellationToken);
            var boatInstance = Object.Instantiate(boatAsset, spawnPosition, Quaternion.identity);
            var boatComponent = boatInstance.GetComponent<BoatComponent>();
            boatComponent.SetCurrentCoord(spawnTile);
            return boatComponent;
        }
    }
}