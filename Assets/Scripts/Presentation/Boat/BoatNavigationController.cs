using System;
using System.Threading;
using Domain.Grid;
using Presentation.Input;
using Presentation.Visuals;
using Services.Navigation;
using UnityEngine;

namespace Presentation.Boat {
    public class BoatNavigationController : MonoBehaviour {
        [SerializeField] InputManager inputManager;
        [SerializeField] PathVisualizer pathVisualizer;
        
        Pathfinder pathfinder;
        CancellationTokenSource movementCancellationToken;
        BoatComponent boat;
        HexLayout layout;

        public void Init(HexGridData grid, BoatComponent boat, HexLayout layout) {
            pathfinder = new Pathfinder(grid);
            this.boat = boat;
            this.layout = layout;
            inputManager.OnMouseClicked += HandleMouseClick;
        }

        void OnDestroy() {
           CancelAndDisposeToken(movementCancellationToken);
            if (inputManager != null)
                inputManager.OnMouseClicked -= HandleMouseClick;
        }

        async void HandleMouseClick(HexCoord goal) {
            CancelAndDisposeToken(movementCancellationToken);
            movementCancellationToken = new CancellationTokenSource();

            try {
                var start = boat.CurrentCoord;
                var path = await pathfinder.FindPathAsync(start, goal, movementCancellationToken.Token);
                pathVisualizer.ShowPath(path, layout);
                
                if (path.Count > 0)
                    await boat.MoveAlongPathAsync(path.ToArray(), movementCancellationToken.Token);
            }
            catch (OperationCanceledException) { }
            catch (Exception e) {
                Debug.LogException(e);
            }
        }

        void CancelAndDisposeToken(CancellationTokenSource ct) {
            movementCancellationToken?.Cancel();
            movementCancellationToken?.Dispose();
        }
    }
}