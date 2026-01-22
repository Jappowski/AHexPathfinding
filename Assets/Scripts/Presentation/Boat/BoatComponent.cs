using System.Threading;
using System.Threading.Tasks;
using Domain.Grid;
using UnityEngine;

namespace Presentation.Boat {
    public class BoatComponent : MonoBehaviour {
        public HexCoord CurrentCoord { get; set; }
        
        public Task MoveAlongPathAsync(HexCoord[] path, CancellationToken cancellationToken) {
            throw new System.NotImplementedException();
        }
        
        public void SetCurrentCoord(HexCoord coord) {
            CurrentCoord = coord;
        }
    }
}