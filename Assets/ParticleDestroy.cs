using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("DestroyGameObject");
    }

    IEnumerator DestroyGameObject()
    {
        yield return new WaitForSecondsRealtime(0.6f);
        Destroy(gameObject);
    }
}
