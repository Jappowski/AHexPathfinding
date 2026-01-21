using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Services.Addressables {
    public interface IAssetProvider {
        public Task InitializeAsync();
        public Task<IList<T>> LoadAsyncByLabel<T>(string path, CancellationToken ct);
    }
}