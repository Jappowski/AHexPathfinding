using System;
using Domain.Grid;
using Presentation.Grid;
using UnityEngine;

namespace Presentation.Input {
    public class InputManager : MonoBehaviour {
        public event Action<HexCoord> OnMouseClicked;

        [SerializeField] Camera camera;
        
        void Awake() {
            if (camera == null)
                camera = Camera.main;
        }

        void Update() {
            if (!UnityEngine.Input.GetMouseButtonDown(0))
                return;
            
            var ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit))
                return;
            
            var tileView = hit.collider.GetComponent<TileView>();
            if (tileView == null)
                return;

            OnMouseClicked?.Invoke(tileView.Coord);
        }
    }
}