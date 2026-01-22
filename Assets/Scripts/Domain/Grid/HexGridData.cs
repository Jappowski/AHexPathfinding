using System;

namespace Domain.Grid {
    public class HexGridData {
        public int Width { get; }
        public int Height { get; }
        
        readonly HexTileType[] tiles;
        
        public HexGridData(int width, int height, HexTileType[] tiles) {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));
            
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            if (tiles == null)
                throw new ArgumentNullException(nameof(tiles));

            Width = width;
            Height = height;
            this.tiles = tiles;
        }

        public HexCoord[] GetAllWaterTiles() {
            var waterTiles = new HexCoord[tiles.Length];
            for (int i = 0; i < tiles.Length; i++) {
                if (tiles[i] == HexTileType.Water) {
                    waterTiles[i] = new HexCoord(i % Width, i / Width);
                }
            }
            
            return waterTiles;
        }

        public HexTileType Get(int q, int r) {
            if (!InBounds(q, r))
                throw new ArgumentOutOfRangeException($"Out of bounds: ({q},{r})");

            return tiles[r * Width + q];
        }
        
        public bool InBounds(int q, int r) => q >= 0 && q < Width && r >= 0 && r < Height;
        
        public HexTileType Get(HexCoord coord) => Get(coord.q, coord.r);
    }
}