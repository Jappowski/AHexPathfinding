using System;

namespace Domain.Grid {
    public class HexGridData {
        public int Width { get; }
        public int Height { get; }
        
        readonly HexTileType[] _tiles;
        
        public HexGridData(int width, int height, HexTileType[] tiles) {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));
            
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            if (tiles == null)
                throw new ArgumentNullException(nameof(tiles));

            Width = width;
            Height = height;
            _tiles = tiles;
        }

        public bool InBounds(int q, int r) => q >= 0 && q < Width && r >= 0 && r < Height;

        public HexTileType Get(int q, int r) {
            if (!InBounds(q, r))
                throw new ArgumentOutOfRangeException($"Out of bounds: ({q},{r})");

            return _tiles[r * Width + q];
        }
        
        public HexTileType Get(HexCoord coord) => Get(coord.Q, coord.R);
    }
}