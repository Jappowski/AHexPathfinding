using System.Collections.Generic;
using Domain.Grid;
using UnityEngine;

namespace Presentation.Visuals {
    public class PathVisualizer : MonoBehaviour {
        [SerializeField] float yOffset = 0.05f;
        [SerializeField] LineRenderer lineRenderer;

        void Awake() {
            lineRenderer.positionCount = 0;
        }

        public void ShowPath(IReadOnlyList<HexCoord> path, HexLayout layout) {
            if (path == null || path.Count == 0) {
                lineRenderer.positionCount = 0;
                return;
            }

            lineRenderer.positionCount = path.Count;
            for (var i = 0; i < path.Count; i++) {
                var position = GridUtils.HexToWorld(layout, path[i]);
                position.y += yOffset;

                lineRenderer.SetPosition(i, position);
            }
        }
        
        public void Clear() {
            lineRenderer.positionCount = 0;
        }
    }
}