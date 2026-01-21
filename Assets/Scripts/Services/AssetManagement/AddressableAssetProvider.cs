using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.AssetManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Services.AssetManagement {
    public class AddressableAssetProvider : IAssetProvider, IGameAssetProvider {
        const string INIT_KEY = "init";
        
        readonly Dictionary<AssetKey, object> assetMap;
        readonly Dictionary<object, AsyncOperationHandle> handles = new();
        readonly Dictionary<AssetLabelReference, IList> labelCache = new();

        public AddressableAssetProvider(AddressableRefConfig refConfig) {
            assetMap = new() {
                { AssetKey.WaterTile, refConfig.waterTile },
                { AssetKey.TerrainTiles, refConfig.terrainTilesLabel },
                { AssetKey.WaterBackground, refConfig.waterBackground },
            };
        }

        public async Task InitializeAsync(CancellationToken ct) {
            var initHandle = Addressables.InitializeAsync();
            handles.Add(INIT_KEY, initHandle);

            using (ct.Register(() => {
                             if (initHandle.IsValid())
                                 Addressables.Release(initHandle); 
                             handles.Remove(INIT_KEY);
                   })) {
                await initHandle.Task;
            }
        }

        public async Task<T> LoadAsync<T>(AssetKey key, CancellationToken ct) where T : UnityEngine.Object {
            var resolved = ResolveKey(key);
            if (resolved is not AssetReferenceGameObject reference)
                throw new InvalidOperationException($"Invalid asset key for object-based loading: {key}");

            return await LoadAsync<T>(reference, ct);
        }

        public async Task<IList<T>> LoadAsyncByLabel<T>(AssetKey key, CancellationToken ct) where T : UnityEngine.Object {
            var resolved = ResolveKey(key);
            if (resolved is not AssetLabelReference label)
                throw new InvalidOperationException($"Invalid asset key for label-based loading: {key}");

            return await LoadAllAsyncByLabel<T>(label, ct);
        }

        public async Task<T> LoadAsync<T>(AssetReferenceGameObject reference, CancellationToken ct) {
            if (handles.TryGetValue(reference, out var cached) && cached.IsValid())
            {
                await cached.Task;
                if (cached.Status == AsyncOperationStatus.Succeeded)
                    return (T)cached.Result;

                throw new Exception($"Addressables load failed for {reference.RuntimeKey}");
            }

            var handle = reference.LoadAssetAsync<T>();
            handles[reference] = handle;

            using (ct.Register(() => {
                       if (handle.IsValid()) Addressables.Release(handle);
                       handles.Remove(reference);
                   })) { 
                return await handle.Task;
            }
        }

        public async Task<IList<T>> LoadAllAsyncByLabel<T>(AssetLabelReference label, CancellationToken ct) {
            if (handles.TryGetValue(label, out var cachedHandle) && cachedHandle.IsValid())
            {
                await cachedHandle.Task;
                if (cachedHandle.Status == AsyncOperationStatus.Succeeded &&
                    labelCache.TryGetValue(label, out var list))
                    return (IList<T>)list;

                throw new Exception($"Addressables load failed for {label.RuntimeKey}");
            }

            var results = new List<T>();
            var handle = Addressables.LoadAssetsAsync<T>(label, results.Add);
            handles[label] = handle;
            labelCache[label] = results;

            using (ct.Register(() => {
                       if (handle.IsValid()) Addressables.Release(handle);
                       handles.Remove(label);
                       labelCache.Remove(label);
                   })) {
                await handle.Task;
                return results;
            }
        }

        public void Release(AssetKey key) {
            Release(ResolveKey(key));
        }

        public void Release(object key) {
            if (!handles.TryGetValue(key, out var handle) || !handle.IsValid())
                return;

            Addressables.Release(handle);
            handles.Remove(key);
        }

        public void ReleaseAll() {
            foreach (var keyValue in handles) {
                if (keyValue.Value.IsValid())
                    Addressables.Release(keyValue.Value);
            }

            handles.Clear();
            labelCache.Clear();
        }

        object ResolveKey(AssetKey key) => assetMap.TryGetValue(key, out var value)
            ? value
            : throw new KeyNotFoundException();
    }
}