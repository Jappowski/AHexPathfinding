using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Grid;
using Domain.Utills;

namespace Services.Navigation {
    public class Pathfinder {
        readonly HexGridData gridData;
        
        public Pathfinder(HexGridData gridData) {
            this.gridData = gridData;
        }

        public Task<List<HexCoord>> FindPathAsync(HexCoord start, HexCoord end, CancellationToken cancellationToken) {
            return Task.Run(() => FindPath(start, end, cancellationToken), cancellationToken);
        }

        List<HexCoord> FindPath(HexCoord start, HexCoord goal, CancellationToken cancellationToken) {
            if (!IsWalkable(start) || !IsWalkable(goal))
                return new List<HexCoord>();
            
            var open = new MinPriorityQueue<HexCoord>();
            open.Enqueue(start, 0); 
            
            var cameFrom = new Dictionary<HexCoord, HexCoord>();
            var gScore = new Dictionary<HexCoord, int> { [start] = 0 };
            var closed = new HashSet<HexCoord>();

            while (open.Count > 0) {
                cancellationToken.ThrowIfCancellationRequested();
                
                var current = open.Dequeue();
                if (closed.Contains(current))
                    continue;

                closed.Add(current);
                if (current.Equals(goal))
                    return Reconstruct(cameFrom, current);
                
                var currentGScore = gScore[current];
                for (var i = 0; i < GridUtils.AxialDirs.Length; i++) {
                    var next = GridUtils.Add(current, GridUtils.AxialDirs[i]);
                    if (!InBounds(next) || !IsWalkable(next))
                        continue;
                    
                    var tentativeGScore = currentGScore + 1; // g = parent.g + 1
                    if (!gScore.TryGetValue(next, out var gScoreOfNext) || tentativeGScore < gScoreOfNext) {
                        cameFrom[next] = current;
                        gScore[next] = tentativeGScore;
                        
                        var h = GridUtils.HexDistance(next, goal);
                        var f = tentativeGScore + h; // f = g + h
                        
                        open.Enqueue(next, f);
                    }
                }
            }
            
            return new List<HexCoord>();
        }

        bool IsWalkable(HexCoord coord) => gridData.Get(coord) == HexTileType.Water;
        bool InBounds(HexCoord coord) => gridData.InBounds(coord.q, coord.r);
        static List<HexCoord> Reconstruct(Dictionary<HexCoord, HexCoord> cameFrom, HexCoord current) {
            var path = new List<HexCoord> { current };
            
            while (cameFrom.TryGetValue(current, out var previous)) {
                path.Add(previous);
                current = previous;
            }

            path.Reverse();
            return path;
        }
    }
}