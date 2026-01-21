using Domain.Grid;
using UnityEngine;

namespace Presentation.DebugTools {
    public class HexMathTester : MonoBehaviour {
        [SerializeField] float _hexRadius = 1f;
        [SerializeField] Transform _marker;

        HexLayout _layout;

        void Awake() {
           
        }

        void Update() {
            if (!Input.GetMouseButtonDown(0)
               ) return;

            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 1000f))
                return;

            var coord = GridUtils.WorldToHex(_layout, hit.point);
            var snapped = GridUtils.HexToWorld(_layout, coord);

            Debug.Log($"Hit {hit.point} -> hex {coord} -> world {snapped}");

            if (_marker)
                _marker.position = snapped + Vector3.up * 0.1f;
        }
    }
}