using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour  where T: Component
{
    [SerializeField] private T _objectPrefab;
    private List<T> _poolObjects = new List<T>();

    public T TakeFromPool()
    {
        T poolObject;
        if (_poolObjects.Count == 0)
        {
            poolObject = Instantiate(_objectPrefab, transform);
        }
        else
        {
            poolObject = _poolObjects[0];
            poolObject.gameObject.SetActive(true);
            _poolObjects.RemoveAt(0);
        }
        return poolObject;
    }

    public void ReturnToPool(T poolObject)
    {
        poolObject.gameObject.SetActive(false);
        poolObject.transform.parent = transform;
        _poolObjects.Add(poolObject);
    }

    public void ReturnToPool(List<T> poolObjects)
    {
        for(int i = 0; i < poolObjects.Count; i++)
        {
            ReturnToPool(poolObjects[i]);
        }
        poolObjects.Clear();
    }
}
