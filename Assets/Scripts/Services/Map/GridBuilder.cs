using System.Threading;
using System.Threading.Tasks;
using Domain.AssetManagement;
using Services.AssetManagement;
using UnityEngine;

namespace Domain.Grid {
    public class GridBuilder {
        const float PROP_SPAWN_PROBABILITY = 0.5f;
        const float TILE_SURFACE_Y = 0.3f;
        const int YIELD_TILES_COUNT = 512;
        const int YIELD_PROPS_COUNT = 1024;

        readonly HexLayout hexLayout;
        readonly HexGridData hexGridData;
        readonly IGameAssetProvider assetProvider;

        public GridBuilder(HexLayout hexLayout, HexGridData hexGridData, IGameAssetProvider assetProvider) {
            this.hexLayout = hexLayout;
            this.hexGridData = hexGridData;
            this.assetProvider = assetProvider;
        }

        public async Task BuildGrid(CancellationToken ct) {
            var waterTile = await assetProvider.LoadAsync<GameObject>(AssetKey.WaterTile, ct);
            var terrainTile = await assetProvider.LoadAsyncByLabel<GameObject>(AssetKey.TerrainTiles, ct);
            var waterTileParent = new GameObject("WaterTiles");
            var terrainTileParent = new GameObject("TerrainTile");

            for (var r = 0; r < hexGridData.Height; r++) {
                ct.ThrowIfCancellationRequested();
                for (var q = 0; q < hexGridData.Width; q++) {
                    var i = q + r * hexGridData.Width;
                    // Yield every N elements to not block the main thread (mobile)
                    if (i != 0 && i % YIELD_TILES_COUNT == 0) {
                        await Task.Yield();
                        ct.ThrowIfCancellationRequested();
                    }

                    var coord = new HexCoord(q, r);
                    var isTerrain = hexGridData.Get(coord) == HexTileType.Terrain;
                    var prefab = isTerrain
                        ? terrainTile[Random.Range(0, terrainTile.Count)]
                        : waterTile;
                    var position = GridUtils.HexToWorld(hexLayout, coord);

                    Object.Instantiate(prefab, position, Quaternion.identity, isTerrain ? terrainTileParent.transform : waterTileParent.transform);
                }
            }

            await Task.Yield();
        }

        public async Task PopulateTerrainWithProps(CancellationToken ct) {
            var props = await assetProvider.LoadAsyncByLabel<GameObject>(AssetKey.Props, ct);
            var parent = new GameObject("Props");
            for (var r = 0; r < hexGridData.Height; r++) {
                ct.ThrowIfCancellationRequested();
                for (var q = 0; q < hexGridData.Width; q++) {
                    var i = q + r * hexGridData.Width;
                    // Yield every N elements to not block the main thread (mobile)
                    if (i != 0 && i % YIELD_PROPS_COUNT == 0) {
                        await Task.Yield();
                        ct.ThrowIfCancellationRequested();
                    }

                    var coord = new HexCoord(q, r);
                    if (hexGridData.Get(coord) != HexTileType.Terrain)
                        continue;

                    if (Random.value > PROP_SPAWN_PROBABILITY)
                        continue;

                    var prop = props[Random.Range(0, props.Count)];
                    var position = GridUtils.HexToWorld(hexLayout, coord);
                    position.y += TILE_SURFACE_Y;
                    Object.Instantiate(prop, position, Quaternion.identity, parent.transform);
                }
            }
        }
    }
}