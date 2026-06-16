using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    private static ObjectPoolManager _instance;
    public static ObjectPoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject poolObj = new GameObject("ObjectPoolManager");
                _instance = poolObj.AddComponent<ObjectPoolManager>();
                DontDestroyOnLoad(poolObj);

                // 动态创建时，初始化我们的核心字典
                _instance.InitDictionary();
            }
            return _instance;
        }
    }

    [System.Serializable]
    public class PoolConfig
    {
        public string poolKey;       // 池子的名字，FlashEffect,GhostTrail
        public string resourcesPath; // 放在 Resources 文件夹下的路径
        public int initCount = 10;   // 预加载数量
    }

    // 这里硬编码配置你所有的池子，无论在哪个场景开局，都能自动识别加载！
    private List<PoolConfig> poolConfigs = new List<PoolConfig>()
    {
        new PoolConfig { poolKey = "FlashEffect", resourcesPath = "ItemFlashEffect", initCount = 10 },
        new PoolConfig { poolKey = "Ghost_Trail", resourcesPath = "GhostTrail", initCount = 8 } 
        // 加对象池子，直接在下面加
    };

    // 每个名字对应一个属于自己的物体列表
    private Dictionary<string, List<GameObject>> poolDictionary;
    // 缓存加载进来的预制体，避免重复去Resources里读盘
    private Dictionary<string, GameObject> prefabCache;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 初始化字典并预加载所有配置的物体
    private void InitDictionary()
    {
        if (poolDictionary != null) return; // 防止 get{} 和 Awake 重复调用

        poolDictionary = new Dictionary<string, List<GameObject>>();
        prefabCache = new Dictionary<string, GameObject>();

        foreach (var config in poolConfigs)
        {
            //去 Resources 文件夹加载预制体
            GameObject prefab = Resources.Load<GameObject>(config.resourcesPath);
            if (prefab == null)
            {
                Debug.LogError($"[ObjectPoolManager] 找不到预制体！请确保 Resources 文件夹下有：{config.resourcesPath}");
                continue;
            }

            prefabCache.Add(config.poolKey, prefab);
            poolDictionary.Add(config.poolKey, new List<GameObject>());

            // 预先生成初始数量
            for (int i = 0; i < config.initCount; i++)
            {
                CreateNewObject(config.poolKey);
            }
        }
    }

    // 内部创建逻辑
    private GameObject CreateNewObject(string key)
    {
        if (!prefabCache.ContainsKey(key)) return null;

        GameObject obj = Instantiate(prefabCache[key], transform);
        obj.SetActive(false);
        poolDictionary[key].Add(obj);
        return obj;
    }

    // 取出逻辑
    public GameObject GetPooledObject(string key, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(key))
        {
            Debug.LogError($"[ObjectPoolManager] 试图获取一个不存在的池子：{key}");
            return null;
        }

        List<GameObject> list = poolDictionary[key];

        // 遍历查找闲置对象
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null && !list[i].activeInHierarchy)
            {
                GameObject obj = list[i];
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                return obj;
            }
        }

        //动态扩容
        GameObject newObj = CreateNewObject(key);
        if (newObj != null)
        {
            newObj.transform.position = position;
            newObj.transform.rotation = rotation;
            newObj.SetActive(true);
        }
        return newObj;
    }
}