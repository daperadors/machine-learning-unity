using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject m_Cloud;
    [SerializeField] private Transform m_CloudTransform;
    [SerializeField] public GameObject m_LastCloud;

    public GameObject newCloud;
    void Start()
    {
        PouController.OnNewJump += GenerateCloud;  
    }

    private void GenerateCloud(GameObject cloudTouched)
    {
        if(cloudTouched.name != "Tocada")
        {
            //StartCoroutine(deleteCloud());
            newCloud = Instantiate(m_Cloud, m_CloudTransform);
            float randomY = Random.Range(2.5f, 3f);
            float randomX = Random.Range(-2f, 2f);
            newCloud.transform.position= new Vector3(randomX, gameObject.transform.position.y+ randomY, 0);
            m_LastCloud = newCloud;
        }
    }
    /*IEnumerator deleteCloud() { 
        yield return new WaitForSeconds(5);
        Destroy(m_LastCloud);
    }*/
}
