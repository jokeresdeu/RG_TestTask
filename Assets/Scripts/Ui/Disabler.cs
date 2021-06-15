using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disabler : MonoBehaviour
{
    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
