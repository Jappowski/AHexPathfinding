using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Services.Addressables;
using UnityEngine;

namespace Domain.Grid {
    public class GridBuilder {
        HexLayout hexLayout;
        HexGridData hexGridData;
        IAssetProvider assetProvider;
        
        public GridBuilder(HexLayout hexLayout, HexGridData hexGridData, IAssetProvider assetProvider) {
            this.hexLayout = hexLayout;
            this.hexGridData = hexGridData;
            this.assetProvider = assetProvider;
        }
        
        public async Task BuildGrid(CancellationToken ct) {
            var waterTile = await assetProvider.LoadAsyncByLabel<GameObject>("WaterTile", ct);
            var terrainTile = await assetProvider.LoadAsyncByLabel<GameObject>("TerrainTile", ct);
            
            for (int r = 0; r < hexGridData.Height; r++) {
                for (int q = 0; q < hexGridData.Width; q++) {
                    var coord = new HexCoord(q, r);
                    var prefab = hexGridData.Get(coord) == HexTileType.Terrain ? terrainTile.First() : waterTile.First();
                    var position = GridUtils.HexToWorld(hexLayout, coord);

                    Object.Instantiate(prefab, position, Quaternion.identity);
                   
                }
            }
            await Task.Yield();
        }
    }
}