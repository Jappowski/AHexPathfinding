using UnityEngine;

namespace Presentation.Camera {
    public class CameraSmoothFollow : MonoBehaviour {
       

        [Header("Follow")]
        [SerializeField] Vector3 offset = new (0f, 10f, -10f);
        [SerializeField] float positionSmoothTime = 0.15f;
        [SerializeField] float maxSpeed = 100f;

        [Header("Look")]
        [SerializeField] bool lookAtTarget = true;
        [SerializeField] float rotationSmoothTime = 0.12f;

        Transform target;
        Vector3 posVel;
        float yawVel;

        public void SetTarget(Transform newTarget) => target = newTarget;

        void LateUpdate() {
            if (target == null)
                return;
            
            var targetPosition = target.position + offset;

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref posVel,
                positionSmoothTime,
                maxSpeed,
                Time.deltaTime
            );

            if (!lookAtTarget)
                return;

            var dir = target.position - transform.position;
            if (dir.sqrMagnitude < 0.000001f)
                return;

            var desiredRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                desiredRot,
                1f - Mathf.Exp(-Time.deltaTime / rotationSmoothTime)
            );
        }
    }
}
