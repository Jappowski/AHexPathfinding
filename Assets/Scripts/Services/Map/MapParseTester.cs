using System;
using Services.Map;
using UnityEngine;

namespace Domain.Grid {
    public class MapParseTester : MonoBehaviour {
        [SerializeField] TextAsset _map;

        void Start() {
            var data = new MapParser().Parse(_map);
            Debug.Log($"Parsed map: {data.Width}x{data.Height}");
        }
    }
}