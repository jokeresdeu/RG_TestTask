using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SingleBlock : MonoBehaviour
{
    private int _arrayIndex;

    public event Action<int> CollidedWithPlayer = delegate { };

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player == null)
            return;

        Vector2 collisionDirection = (transform.position - other.transform.position);
        if (Mathf.Abs(collisionDirection.x) > 0.8 || Mathf.Abs(collisionDirection.y) > 0.5)
            return;

        CollidedWithPlayer(_arrayIndex);
    }

    public void SetIndex(int index)
    {
        _arrayIndex = index;
    }
}
