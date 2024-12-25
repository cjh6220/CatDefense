using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using Cysharp.Threading.Tasks;
using System.Linq;
using Object = UnityEngine.Object;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class AssetManager : MonoBehaviour
{
    private enum Label
    {
        Preload,
        Scene,
    }

    static readonly string Bundle   = "Bundle";
    static readonly string Preload  = "Preload";

    static private Dictionary<string, Object>   _assets;
    static private List<string>                 _primaryList;
    static public long TotalDownloadSize { set; get; } = 0L;

    private bool initialize = false;

    private void OnDestroy()
    {
        Release();

        _assets.Clear();
        _primaryList.Clear();
    }

    static public async UniTask Initialize()
    {
        _assets         = new Dictionary<string, Object>();
        _primaryList    = new List<string>();

        await InitializeAssets();
    }

    static public T GetAsset<T>(string key) where T : Object
    {
        if (_assets.TryGetValue(key, out var obj))
        {
            return obj as T;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Get Assets
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    static public async UniTask<T> GetAssetAsync<T>(string key) where T : Object
    {
        T result = null;

        if (_assets.TryGetValue(key, out var obj))
        {
            result = obj as T;
        }
        else
        {
            result = await LoadAssetAsync<T>(key);
            return result;
        }

        return result is T ? result : default;
    }

    static public async UniTask<T> GetAssetAsync<T>(string key, System.Threading.CancellationTokenSource source) where T : Object
    {
        T result = null;

        if (_assets.TryGetValue(key, out var obj))
        {
            result = obj as T;
        }
        else
        {
            try
            {
                result = await Addressables.LoadAssetAsync<T>(key).ToUniTask().AttachExternalCancellation(source.Token);
            }
            catch(OperationCanceledException)
            {
                return null;
            }
        }

        return result is T ? result : default;
    }

    static public async UniTask<T> CreateAsset<T>(string key) where T : Object
    {
        T result = null;
        if(_assets.TryGetValue(key, out var obj))
        {
            object get  = Instantiate(obj);
            result      = get as T;
        }
        else
        {
            result = await Addressables.InstantiateAsync(key) as T;
        }

        return result is T ? result : default;
    }

    static public async UniTask GetAssetAsync<T>(string key, UnityAction<T> isComplete) where T : Object
    {
        T t = await GetAssetAsync<T>(key);

        isComplete.Invoke(t);
    }

    /// <summary>
    /// Initialize Addressable & Check for update
    /// </summary>
    /// <returns></returns>
    static private async UniTask InitializeAssets()
    {
        await Addressables.InitializeAsync();
        await CheckForUpdateByCatalog( await Addressables.CheckForCatalogUpdates() );
    }

    #region [ Check for download ]

    /// <summary>
    /// CHECK UPDATE
    /// </summary>
    /// <param name="updateList"></param>
    /// <returns></returns>
    private static async UniTask CheckForUpdateByCatalog( List<string> updateList )
    {
        Debug.Log("CHECKING UPDATE BY ASSET");

        if( 0 < updateList.Count )
        {
            await Addressables.UpdateCatalogs(updateList);
        }

        await SetDownloadSize();
    }

    /// <summary>
    /// SET DOWNLOAD SIZE
    /// </summary>
    /// <returns></returns>
    private static async UniTask SetDownloadSize()
    {
        var result      = await Addressables.LoadResourceLocationsAsync(Bundle);
        _primaryList    = result.Select(l => l.PrimaryKey).ToList();

        TotalDownloadSize = await Addressables.GetDownloadSizeAsync(_primaryList);
    }
    #endregion

    #region [ Load ]
    static private async UniTask<T> LoadAssetAsync<T>(string key) where T : Object
    {
        if (true == _assets.ContainsKey(key))
        {
            return _assets[key] as T;
        }

        T load  = null;
        load    = await Addressables.LoadAssetAsync<T>(key);

        if (true == _assets.ContainsKey(key))
        {
            load =_assets[key] as T;
        }
        else
        {
            if (null != load)
                _assets.Add(key, load);

        }

        return load;

    }

    static public UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle StartPreload()
    {
        var handle = Addressables.LoadResourceLocationsAsync(Preload, typeof(Object));
        return handle;
    }


    static public async UniTask<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle> LoadByLabel( UnityAction<int> cb )
    {
        var assets      = await Addressables.LoadResourceLocationsAsync(Preload, typeof(Object));
        int maxAssets   = assets.Count;

        return Addressables.LoadAssetsAsync<Object>(assets, (result) =>
        {
            if (_assets.ContainsKey(result.name))
            {
                
            }
            else
            {
                _assets.Add(result.name, result);
                ResourcePoolManager.AddPreload(result.name, result);
            }

            cb.Invoke(maxAssets);
        });
    }

    #endregion

    

    #region [ Release ]
    static public void Release()
    {
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }
    #endregion
}