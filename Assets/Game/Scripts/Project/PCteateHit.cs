using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCteateHit : MonoBehaviour
{
    public Transform downPos;
    public Transform upPos;
    public Transform leftPos;
    public Transform rightPos;

    public GameObject hitPrefab;

    public Transform createPos;

    public void OnCreateHitEffect(int type)
    {
        GameObject obj = null;
        switch (type)
        {
            case 1:
                 obj = SimplePool.Spawn(hitPrefab, downPos.position,Quaternion.identity);
                obj.transform.parent = createPos;
                break;
            case 2:
                 obj = SimplePool.Spawn(hitPrefab, upPos.position, Quaternion.identity);
                obj.transform.parent = createPos;
                break;
            case 3:
                obj = SimplePool.Spawn(hitPrefab, leftPos.position, Quaternion.identity);
                obj.transform.parent = createPos;
                break;
            case 4:
                 obj = SimplePool.Spawn(hitPrefab, rightPos.position, Quaternion.identity);
                obj.transform.parent = createPos;
                break;
        }
        obj.SetActive(true);
    }
}
