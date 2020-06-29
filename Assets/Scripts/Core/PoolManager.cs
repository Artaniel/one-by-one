using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    void Awake()
    {
        instance = this;
        toPoolBuffer = new List<PoolObject>();
        pools = new Dictionary<string, LinkedList<PoolObject>>();
    }

    public struct PoolObject
    {
        public GameObject obj;
        public float timeLife;

        public string name { get => obj.name; }

        public PoolObject(GameObject Obj, float TimeLife)
        {
            obj = Obj;
            timeLife = TimeLife;
        }
    }

    public static GameObject GetPool(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        var pools = instance.pools;
        if (!pools.ContainsKey(prefab.name))
        {
            pools[prefab.name] = new LinkedList<PoolObject>();
        }
        PoolObject result;
        //print(pools[prefab.name].Count);
        if (pools[prefab.name].Count > 0)
        {
            //Debug.Log($"Retrieve from pool: {prefab.name}");
            result = pools[prefab.name].First.Value;
            pools[prefab.name].RemoveFirst();
            result.obj.transform.position = pos;
            result.obj.transform.rotation = rot;
            result.obj.SetActive(true);
            return result.obj;
        }
        else
        {
            //Debug.Log($"Creating new object: {prefab.name}");
            result.obj = GameObject.Instantiate(prefab, pos, rot);
            result.obj.name = prefab.name;
            return result.obj;
        }
    }

    public static void ReturnToPool(PoolObject target, float timer)
    {
        target.timeLife = timer;
        instance.toPoolBuffer.Add(target);
    }

    public static void ReturnToPool(GameObject target, float timer)
    {
        instance.toPoolBuffer.Add(new PoolObject(target, timer));
    }

    public static void ReturnToPool(GameObject target)
    {
        ReturnToPool(target, 0);
    }

    public static void ClearPool()
    {
        var pools = instance.pools;
        foreach (var item in pools.Values)
        {
            item.Clear();
        }
        pools.Clear();
    }

    void LateUpdate()
    {
        for (int i = 0; i < toPoolBuffer.Count; i++)
        {
            PoolObject poolItem = toPoolBuffer[i];
            poolItem.timeLife -= Time.deltaTime;
            toPoolBuffer[i] = poolItem;

            if (toPoolBuffer[i].timeLife < 0)
            {
                pools[toPoolBuffer[i].name].AddFirst(toPoolBuffer[i]);
                toPoolBuffer[i].obj.SetActive(false);
                toPoolBuffer.RemoveAt(i);
                i--;
            }
        }
    }

    private List<PoolObject> toPoolBuffer;
    private Dictionary<string, LinkedList<PoolObject>> pools;
}
