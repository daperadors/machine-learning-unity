using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject m_Cloud;
    [SerializeField] public GameObject m_Muerte;
    [SerializeField] public Transform m_MuerteTransform;
    [SerializeField] private Transform m_CloudTransform;
    [SerializeField] private Transform m_SueloTransform;
    [SerializeField] public GameObject m_LastCloud;
    [SerializeField] public TextMeshProUGUI m_ScoreText;
    [SerializeField] public TextMeshProUGUI m_MaxScoreText;
    private int m_Score = 0;
    private int m_MaxScore = 0;

    public GameObject newCloud = null;
    void Start()
    {
        //PouController.OnNewJump += GenerateCloud;
        PouController.OnPouDead += PouDead;
    }

    public void GenerateCloud(GameObject cloudTouched)
    {
        if(cloudTouched.name != "Tocada")
        {
            m_Score++;
            m_ScoreText.text = "Score: " + m_Score;

            GameObject muerte = Instantiate(m_Muerte, m_MuerteTransform);
            muerte.transform.position= new Vector2(cloudTouched.transform.position.x, cloudTouched.transform.position.y-2);
            //StartCoroutine(deleteMuerte());
            newCloud = Instantiate(m_Cloud, m_CloudTransform);
            float randomY = Random.Range(2.5f, 3f);
            float randomX = Random.Range(-3f, 3f);
            newCloud.transform.position= new Vector3(m_SueloTransform.position.x+randomX, gameObject.transform.position.y+ randomY, 0);
            m_LastCloud = newCloud;
        }
    }
    private void PouDead()
    {
        if(m_MaxScore < m_Score)
        {
            m_MaxScore= m_Score;
            m_MaxScoreText.text = "Max score: " + m_MaxScore;
        }
        m_Score = 0;
        m_ScoreText.text = "Score: " + m_Score;
    }
    /*IEnumerator deleteMuerte() { 
        yield return new WaitForSeconds(5);
        Destroy(m_LastCloud);
    }*/
}
