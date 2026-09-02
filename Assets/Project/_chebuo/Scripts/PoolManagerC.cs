using UnityEngine;
using UnityEngine.Pool;

public class PoolManagerC : MonoBehaviour
{
    ObjectPool<GameObject> pool;
    public GameObject prefab;
    void Awake()=>pool=new ObjectPool<GameObject>(OnCreatePooledObject, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject);

    GameObject OnCreatePooledObject()
    {
        return Instantiate(prefab);
    }
    void OnGetFromPool(GameObject obj)
    {
        obj.SetActive(true);
    }

    void OnReleaseToPool(GameObject obj)
    {
        obj.SetActive(false);
    }

    void OnDestroyPooledObject(GameObject obj)
    {
        Destroy(obj);
    }

    public GameObject GetGameObject(GameObject prefab,Vector3 pos,Quaternion rot)
    {
        this.prefab=prefab;
        GameObject obj=pool.Get();
        Transform tf=obj.transform;
        tf.position=pos;
        tf.rotation=rot;

        return obj;
    }
    public void ReleaseGameObject(GameObject obj)
    {
        pool.Release(obj);
    }
}