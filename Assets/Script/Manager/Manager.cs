using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Manager : Singleton<Manager>
{
    private async void Awake() 
    {
        DontDestroyOnLoad(this);
        await InitManager();
    }

    async UniTask InitManager()
    {
        //await AssetManager.Initialize();
        InitAssetManager(transform);
        InitResourcePool(transform);
        SceneManager.LoadScene("BattleScene");
    }

    private async void InitAssetManager( Transform parent )
    {
        // AssetManager 오브젝트를 루트로 생성하고 DontDestroyOnLoad 적용
        GameObject asset = new GameObject("[AssetManager]");
        
        asset.AddComponent<AssetManager>();
        asset.transform.SetParent(parent);
        await AssetManager.Initialize();

        Debug.Log("StartScene: AssetManager initialized.");
    }

    private void InitResourcePool(Transform parent)
    {
        // ResourcePoolManager 오브젝트를 루트로 생성하고 DontDestroyOnLoad 적용
        GameObject resource = new GameObject("[ResourcePoolManager]");

        resource.AddComponent<ResourcePoolManager>();
        resource.transform.SetParent(parent);

        Debug.Log("StartScene: ResourcePoolManager initialized.");
    }
}
