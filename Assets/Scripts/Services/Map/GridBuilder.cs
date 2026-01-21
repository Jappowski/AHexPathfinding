using System.Threading;
using System.Threading.Tasks;
using Domain.AssetManagement;
using Services.AssetManagement;
using UnityEngine;

namespace Domain.Grid {
    public class GridBuilder {
        HexLayout hexLayout;
        HexGridData hexGridData;
        IGameAssetProvider assetProvider;
        
        public GridBuilder(HexLayout hexLayout, HexGridData hexGridData, IGameAssetProvider assetProvider) {
            this.hexLayout = hexLayout;
            this.hexGridData = hexGridData;
            this.assetProvider = assetProvider;
        }
        
        public async Task BuildGrid(CancellationToken ct) {
            var waterTile = await assetProvider.LoadAsync<GameObject>(AssetKey.WaterTile, ct);
            var terrainTile = await assetProvider.LoadAsyncByLabel<GameObject>(AssetKey.TerrainTiles, ct);

            for (int r = 0; r < hexGridData.Height; r++) {
                for (int q = 0; q < hexGridData.Width; q++) {
                    var coord = new HexCoord(q, r);
                    var prefab = hexGridData.Get(coord) == HexTileType.Terrain ? terrainTile[Random.Range(0, terrainTile.Count)] : waterTile ;
                    var position = GridUtils.HexToWorld(hexLayout, coord);

                    Object.Instantiate(prefab, position, Quaternion.identity);
                   
                }
            }
            await Task.Yield();
        }
    }
}