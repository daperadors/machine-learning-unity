using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class PouController : Agent
{
    [Range(0f, 20f)]
    [SerializeField] private float m_JumpForce;
    private Rigidbody2D m_Rigidbody;
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Plataforma")
            m_Rigidbody.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);


    }
}
