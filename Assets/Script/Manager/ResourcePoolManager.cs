using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.U2D;
using System;
using System.Threading;
using System.Linq;

public class PoolObject
{
    public bool Use { private set; get; }
    public GameObject PoolGo { private set; get; }

    public void SetGameobject(GameObject go)
    {
        PoolGo = go;
    }
    public void SetUse(bool use)
    {
        Use = use;
    }
    public bool Available()
    {
        return Use == false && PoolGo.activeSelf == false;
    }

    public void Despawn()
    {
        GameObject.Destroy(PoolGo);
    }
}

[System.Serializable]
public class Pools
{
    [SerializeField, ReadOnly]string name;
    readonly string Tag = "Optimization";
    public string   Name { private set; get; }
    public bool     Optimization { private set; get; }
    
    GameObject      OwnGo;

    List<PoolObject> objectList = new List<PoolObject>();

    public Pools( string resName )
    {
        name        = resName;
        Name        = resName;
        objectList  = new List<PoolObject>();
        OwnGo       = null;
    }

    public void Preload(GameObject go, Transform parent)
    {
        OwnGo = go;

        //== GAMEOBJECT
        GameObject newGo = GameObject.Instantiate(OwnGo, parent);
        newGo.name       = Name;
        newGo.SetActive(false);

        //== POOL
        PoolObject newPool = new PoolObject();
        newPool.SetGameobject(newGo);
        newPool.SetUse(false);

        objectList.Add(newPool);
    }

    public async UniTask<GameObject> Get( CancellationTokenSource source )
    {
        PoolObject pool = objectList.Find(element => element.Available() == true);
        if (null == pool)
        {
            try
            {
                if (OwnGo == null)
                {
                    OwnGo           = await AssetManager.GetAssetAsync<GameObject>(Name, source);
                    Optimization    = OwnGo.tag.Equals(Tag);
                }

                GameObject newGo    = GameObject.Instantiate(OwnGo);
                PoolObject newPool  = new PoolObject();
                newGo.name          = Name;

                newPool.SetGameobject(newGo);
                newPool.SetUse(true);
                
                objectList.Add(newPool);
                pool = newPool;
            }
            catch(Exception)
            {
                return null;
            }
        }
        else
        {
            pool.SetUse(true);
        }

        return pool.PoolGo;
    }
   
    public bool Despawn(GameObject go)
    {
        PoolObject ob = objectList.Find(element => element.PoolGo == go);
        if(ob == null)
        {
            return false;
        }
        else
        {
            ob.SetUse(false);
            return true;
        }
    }

    public void TryOptimization()
    {
        if(Optimization == false)
        {
            return;
        }

        int cnt = objectList.Count;

        for (int i = 0; i < objectList.Count; i++) 
        {
            cnt -= 1;
            PoolObject obj = objectList[cnt];

            if(obj.Available())
            {
                obj.Despawn();
                objectList.Remove(obj);
            }
        }
    }

    public void Reset()
    {
        for(int i = 0; i < objectList.Count; i++)
        {
            objectList[i].Despawn();
        }

        objectList.Clear();
        objectList = null;
    }
}

public class ResourcePoolManager : MonoBehaviour
{
    public enum Atlas
    {
        Wing,
        UI,
        Text,
        Skin,
        Skill,
        Shop,
        Profile,
        Pet,
        Monster,
        Halo,
        GodWeapon,
        Gem,
        Equipment,
        CharacterStat,
        Banner,
        Background,
        CutScene,
    }

    private static ResourcePoolManager instance;

    [SerializeField, ReadOnly] private List<Pools> poolList = new List<Pools>();
    private Dictionary<string, SpriteAtlas> atlasList   = new Dictionary<string, SpriteAtlas>();
    private List<CancellationTokenSource> sourceList    = new List<CancellationTokenSource>();

    static public Dictionary<string, SpriteAtlas> AtlasList => instance.atlasList;

    private void Awake()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        poolList.Clear();
        instance = null;
    }

    private void Initialize()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    static public void LoadAtlas()
    {
        string[] atlas      = Enum.GetNames(typeof(Atlas));
        List<UniTask> tasks = new List<UniTask>();

        for(int i = 0; i < atlas.Length; i++)
        {
            string key          = $"atlas_{atlas[i]}";
            SpriteAtlas result  = AssetManager.GetAsset<SpriteAtlas>(key);

            if (result == null)
            {
                Debug.LogError($"{key} is not Load");
                continue;
            }
            else
            {
                if(instance.atlasList.ContainsKey(key))
                {
                    continue;
                }
                else
                {
                    instance.atlasList.Add(key, result);
                }
            }
        }
    }

    static public Sprite GetSprite( Atlas atlas, string resName )
    {
        string res = $"atlas_{atlas}";
        if (false == instance.atlasList.TryGetValue(res, out var asset))
        {
            DebugUtil.Log($"{res} : {resName} is null!!!");
            return null;
        }

        return asset.GetSprite( resName );
    }
    static public Sprite GetSprite(string atlas, string resName)
    {
        if (false == instance.atlasList.TryGetValue(atlas, out var asset))
        {
            DebugUtil.Log($"{atlas} : {resName} is null!!!");
            return null;
        }

        return asset.GetSprite(resName);
    }

    //== PRELOAD
    #region PRELOAD
    static public void AddPreload(string name, UnityEngine.Object res)
    {
        if (res is GameObject == false)
        {
            return;
        }

        bool contain = instance.poolList.Any(l => l.Name == name);
        if(contain)
        {
            return;
        }
        else
        {
            Pools pool = new Pools(name);
            pool.Preload(res as GameObject, instance.transform);

            instance.poolList.Add(pool);
        }
    }
    #endregion

    //== LOAD
    #region
    static public async UniTask<T> GetAsync<T>( string resName , bool active = true, Transform parent = null) where T : UnityEngine.Object
    {
        Pools pool = instance.poolList.Find( element => string.Equals(resName, element.Name) == true );
        if ( null == pool )
        {
            string name = resName;
            pool        = new Pools(name);

            instance.poolList.Add(pool);
        }

        Transform trans                 = parent ?? instance.transform;
        CancellationTokenSource source  = new CancellationTokenSource();
        try
        {
            instance.sourceList.Add(source);

            GameObject obj = await pool.Get(source);

            if(instance.sourceList.Contains(source))
            {
                instance.sourceList.Remove(source);
            }

            obj.transform.SetParent(trans);
            obj.SetActive(active);

            if( null == obj )
            {
                return null;
            }
            else
            {
                T component = null;
           
                obj.TryGetComponent(out component);

                return component;
            }
        }
        catch(Exception)
        {
            if (instance.sourceList.Contains(source))
            {
                instance.sourceList.Remove(source);
            }

            return null;
        }
    }

    static public async UniTask GetAsync<T>(string resName, bool active = true, Transform parent = null , UnityAction<T> isComplete = null ) where T : UnityEngine.Object
    {
        T t = await GetAsync<T>(resName, active, parent);

        if (null != isComplete && t != null)
            isComplete.Invoke(t);
    }

    static public async UniTask<GameObject> GetAsync( string resName, bool active, Transform parent )
    {
        Pools pool = instance.poolList.Find(element => string.Equals(resName, element.Name) == true);
        if (null == pool)
        {
            string name = resName;
            pool        = new Pools(name);

            instance.poolList.Add(pool);
        }

        CancellationTokenSource source = new CancellationTokenSource();
        try
        {
            instance.sourceList.Add(source);

            Transform trans = parent ?? instance.transform;
            GameObject obj  = await pool.Get(source);

            if(instance.sourceList.Contains(source))
            {
                instance.sourceList.Remove(source);
            }

            obj.transform.SetParent(trans);
            obj.SetActive(active);

            return obj;
        }
        catch(Exception)
        {
            if (instance.sourceList.Contains(source))
            {
                instance.sourceList.Remove(source);
            }

            return null;
        }

    }

    static public async UniTask GetAsync(string resName, bool active, Transform parent, UnityAction<GameObject> isComplete)
    {
        var go = await GetAsync(resName, active, parent);
        
        if(null != isComplete && go != null)
            isComplete.Invoke(go);
    }
    #endregion

    //== LOAD BY TOKEN
    #region LOAD BY TOKEN
    static public async UniTask GetAsync<T>(string resName, CancellationTokenSource source, bool active = true, Transform parent = null, UnityAction<T> isComplete = null) where T : UnityEngine.Object
    {
        T t = await GetAsync<T>(resName, source, active, parent);

        if (null != isComplete && t != null)
            isComplete.Invoke(t);
    }

    static public async UniTask<T> GetAsync<T>(string resName, CancellationTokenSource source, bool active = true, Transform parent = null) where T : UnityEngine.Object
    {
        Pools pool = instance.poolList.Find(element => string.Equals(resName, element.Name) == true);
        if (null == pool)
        {
            string name = resName;
            pool = new Pools(name);

            instance.poolList.Add(pool);
        }

        Transform trans = parent ?? instance.transform;
        try
        {
            instance.sourceList.Add(source);
            GameObject obj = await pool.Get(source);

            obj.transform.SetParent(trans);
            obj.SetActive(active);

            if(instance.sourceList.Contains(source))
            {
                instance.sourceList.Remove(source);
            }

            if (null == obj)
            {
                return null;
            }
            else
            {
                T component = null;

                obj.TryGetComponent(out component);

                return component;
            }
        }
        catch(Exception)
        {
            if (instance.sourceList.Contains(source))
            {
                instance.sourceList.Remove(source);
            }

            return null;
        }
    }
    static public async UniTask GetAsync(string resName, CancellationTokenSource source, bool active, Transform parent, UnityAction<GameObject> isComplete)
    {
        var go = await GetAsync(resName, source, active, parent);

        if (null != isComplete && go != null)
            isComplete.Invoke(go);
    }

    static public async UniTask<GameObject> GetAsync(string resName, CancellationTokenSource source, bool active, Transform parent)
    {
        Pools pool = instance.poolList.Find(element => string.Equals(resName, element.Name) == true);
        if (null == pool)
        {
            string name = resName;
            pool = new Pools(name);

            instance.poolList.Add(pool);
        }

        Transform trans = parent ?? instance.transform;
        try
        {
            instance.sourceList.Add(source);

            GameObject obj = await pool.Get(source);

            obj.transform.SetParent(parent);
            obj.SetActive(active);

            if (instance.sourceList.Contains(source))
            {
                instance.sourceList.Remove(source);
            }

            return obj;
        }
        catch(Exception)
        {
            if (instance.sourceList.Contains(source))
            {
                instance.sourceList.Remove(source);
            }

            return null;
        }
    }

    #endregion

    static public async UniTask<T> GetAnimator<T>(string animator) where T : UnityEngine.Object
    {
        T obj = await AssetManager.GetAssetAsync<T>(animator);

        return obj;
    }

    //== DESPAWN
    #region DESPAWN
    static public void Despawn( GameObject res )
    {
        if(res == null)
        {
            return;
        }

        string name = res.name;
        Pools pool  = instance.poolList.Find(element => element.Name == name);

        if(pool == null)
        {
            GameObject.Destroy(res);
            Debug.LogError($"{res.name} Object is not contain in pool");
            return;
        }

        bool despawn = pool.Despawn(res);
        if(despawn)
        {
            res.transform.SetParent(instance.transform);
            res.SetActive(false);
        }
        else
        {
            GameObject.Destroy(res);
        }
    }

    static public void ResetPool()
    {
        if(instance == null)
        {
            DebugUtil.LogError($"Resource Pool Manager instance is null");
            return;
        }

        if(instance.sourceList != null)
        {
            for(int i = 0; i < instance.sourceList.Count; i++)
            {
                CancellationTokenSource source = instance.sourceList[i];
                source.Cancel();
                source.Dispose();
            }

            instance.sourceList.Clear();
        }

        for(int i = 0; i < instance.poolList.Count; i++)
        {
            Pools pool = instance.poolList[i];
            pool.Reset();
        }

        instance.poolList.Clear();
    }

    static public void OptimizationPool()
    {
        if (instance == null)
        {
            DebugUtil.LogError($"Resource Pool Manager instance is null");
            return;
        }

        var poolList = instance.poolList;
        for (int i = 0; i < poolList.Count; i++) 
        {
            var pool = poolList[i];
            pool.TryOptimization();
        }
    }
    #endregion
}
