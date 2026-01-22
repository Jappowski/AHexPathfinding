using Domain.Grid;
using UnityEngine;

namespace Presentation.Grid {
    public class TileView : MonoBehaviour {
        [SerializeField] public HexCoord Coord;
        public HexTileType Type { get; private set; }

        public void Init(HexCoord coord, HexTileType type) {
            Coord = coord;
            Type = type;
        }
    }
}