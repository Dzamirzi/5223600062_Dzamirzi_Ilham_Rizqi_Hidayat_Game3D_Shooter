using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class Despawn : MonoBehaviour
{
    public float delayToDespawn = 3;

    private void OnEnable()
    {
        StartCoroutine(IEDelayToDespawn());
        IEnumerator IEDelayToDespawn()
        {
            yield return new WaitForSeconds(delayToDespawn);
            LeanPool.Despawn(this);
        }
    }
}