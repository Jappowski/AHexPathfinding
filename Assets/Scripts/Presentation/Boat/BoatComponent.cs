using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Grid;
using UnityEngine;

namespace Presentation.Boat {
    public class BoatComponent : MonoBehaviour {
        [Header("Movement")]
        [SerializeField] float moveSpeed = 4f;           // units/sec
        [SerializeField] float arriveDistance = 0.05f;   // units

        [Header("Rotation")]
        [SerializeField] float turnSpeedDeg = 360f;      // deg/sec
        [SerializeField] float turnEpsilonDeg = 1f;      // deg

        MaterialPropertyBlock materialBlock;
        [SerializeField] TrailRenderer trailRenderer;
        [SerializeField, Range(0, 1f)] float foamSpeed;
        [SerializeField] string phaseProperty;
        Renderer fakerenderer;

        public HexCoord CurrentCoord { get; private set; }

        // void Awake() {
        //     materialBlock = new MaterialPropertyBlock();
        //     fakerenderer = trailRenderer;
        // }
        //
        // void LateUpdate() {
        //     trailRenderer.emitting = foamSpeed > 0.02f;
        //     fakerenderer.GetPropertyBlock(materialBlock);
        //     materialBlock.SetFloat(phaseProperty, foamSpeed);
        //     fakerenderer.SetPropertyBlock(materialBlock);
        // }

        public void SetCurrentCoord(HexCoord coord) {
            CurrentCoord = coord;
        }

        public async Task MoveAlongPathAsync(Vector3[] path, HexLayout layout, CancellationToken cancellationToken) {
            if (path == null || path.Length == 0)
               return;

            foreach (var point in path) {
                cancellationToken.ThrowIfCancellationRequested();

                await RotateTowardsAsync(point, cancellationToken);
                //foamSpeed = 1;
                await MoveToAsync(point, cancellationToken);
                //foamSpeed = 0;
                CurrentCoord = GridUtils.WorldToHex(layout, point);
            }
        }
        
        async Task RotateTowardsAsync(Vector3 targetPos, CancellationToken ct) {
            var dir = targetPos - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.000001f)
                return;

            var targetRot = Quaternion.LookRotation(dir.normalized, Vector3.up);

            while (true) {
                ct.ThrowIfCancellationRequested();

                var angle = Quaternion.Angle(transform.rotation, targetRot);
                if (angle <= turnEpsilonDeg) {
                    transform.rotation = targetRot; // snap
                    return;
                }

                var maxStep = turnSpeedDeg * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, maxStep);

                await Task.Yield();
            }
        }

        async Task MoveToAsync(Vector3 targetPos, CancellationToken ct) {
            while (true) {
                ct.ThrowIfCancellationRequested();

                var current = transform.position;
                var toTarget = targetPos - current;
                toTarget.y = 0f;

                var dist = toTarget.magnitude;
                if (dist <= arriveDistance)
                {
                    transform.position = new Vector3(targetPos.x, current.y, targetPos.z);
                    return;
                }

                var step = moveSpeed * Time.deltaTime;
                var move = toTarget.normalized * Mathf.Min(step, dist);

                transform.position = new Vector3(current.x + move.x, current.y, current.z + move.z);

                await Task.Yield();
            }
        }
    }
}