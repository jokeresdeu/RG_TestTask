using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    private Rigidbody[] _parts;
    private Vector3[] _partsStartPostion;
    private ObstaclePartsPool _objectPool;
    private bool _inited;
    // Start is called before the first frame update
    public void Init(ObstaclePartsPool objectPool)
    {
        if (_inited)
            return;
        _inited = true;
        _objectPool = objectPool;
        _parts = GetComponentsInChildren<Rigidbody>();
        _partsStartPostion = new Vector3[_parts.Length];
        for(int i = 0; i< _parts.Length; i++)
        {
            _partsStartPostion[i] = _parts[i].transform.localPosition;
        }
    }

    public void Activate()
    {
        for(int i = 0; i < _parts.Length; i++)
        {
            _parts[i].AddForce(new Vector3(Random.Range(-5,5), Random.Range(-5, 5), Random.Range(10, 20)), ForceMode.Impulse);
        }
        Invoke("ReturnToPool", 2f);
    }

    private void ReturnToPool()
    {
        for(int i =0; i< _parts.Length; i++)
        {
            _parts[i].transform.localPosition = _partsStartPostion[i];
            _parts[i].velocity = Vector3.zero;
        }
        _objectPool.ReturnToPool(this);
    }

   
}
