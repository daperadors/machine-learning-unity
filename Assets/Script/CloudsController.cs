using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsController : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DeleteCloud());
    }

    IEnumerator DeleteCloud()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
