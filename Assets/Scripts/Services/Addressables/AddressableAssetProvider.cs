using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Addressables {
    public class AddressableAssetProvider : IAssetProvider {
        
        public async Task InitializeAsync() {
            await UnityEngine.AddressableAssets.Addressables.InitializeAsync().Task;
        }
        public async Task<IList<T>> LoadAsyncByLabel<T>(string path, CancellationToken ct) {
            var results = new List<T>();
            var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync<T>(
                path,
                asset => results.Add(asset)
            );

            await handle.Task;
            return results;
        }
    }
}