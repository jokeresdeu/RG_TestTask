using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FinishLine : MonoBehaviour
{
    public event Action FinishLineCrossed = delegate { };
    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player == null)
            return;

        FinishLineCrossed();
    }
}
