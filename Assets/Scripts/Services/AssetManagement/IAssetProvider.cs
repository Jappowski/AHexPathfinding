using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Services.AssetManagement {
    public interface IAssetProvider {
        public Task InitializeAsync(CancellationToken ct);

        public Task<T> LoadAsync<T>(AssetReferenceGameObject reference, CancellationToken ct);

        public Task<IList<T>> LoadAllAsyncByLabel<T>(AssetLabelReference label, CancellationToken ct);

        public void Release(object key);

        public void ReleaseAll();
    }
}