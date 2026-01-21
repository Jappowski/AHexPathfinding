using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.AssetManagement;

namespace Services.AssetManagement {
    public interface IGameAssetProvider {
        Task<T> LoadAsync<T>(AssetKey key, CancellationToken ct) where T : UnityEngine.Object;
        Task<IList<T>> LoadAsyncByLabel<T>(AssetKey key, CancellationToken ct) where T : UnityEngine.Object;
        void Release(AssetKey key);
    }
}