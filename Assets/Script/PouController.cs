using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.UIElements;

public class PouController : Agent
{

    //Agent

    public delegate void RestetSimulation();
    public event RestetSimulation OnResetSimulation;

    //Logica
    [Range(0f, 20f)]
    [SerializeField] private float m_JumpForce;
    private Rigidbody2D m_Rigidbody;

    [SerializeField] private float speed = 5f;
    [SerializeField] private Vector3 m_InitialPosition;
    [SerializeField] GameManager m_GameManager;
    private float horizontalInput;

    [SerializeField] public Transform m_MuerteTransform;
    [SerializeField] Transform m_NubesTransform;
    [SerializeField] Transform m_PlataformaTransform;
    [SerializeField] GameObject m_NubePrincipal;

    public delegate void NewJump(GameObject cloud);
    public static NewJump OnNewJump;
    public delegate void ObstacleEvasion();
    public event ObstacleEvasion OnObstacleEvasion;
    public delegate void PouDead();
    public static PouDead OnPouDead;
    private bool floorTouched = false;
    Vector2 cameraVector2;
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        cameraVector2= Camera.main.transform.localPosition;
        m_InitialPosition = transform.position;
    }
    /*void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(horizontalInput, 0f, 0f);
        movement *= speed * Time.fixedDeltaTime;
        transform.position += movement;
    }*/
    private void FixedUpdate()
    {
        if (!floorTouched) return;
        RequestDecision();
    }
    private void ResetPlayer()
    {
        transform.position = m_InitialPosition;
        m_Rigidbody.velocity = Vector3.zero;

        OnResetSimulation?.Invoke();
    }
    public override void OnEpisodeBegin()
    {
        ResetPlayer();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector2 distance;

        //Posicion player actual
        distance = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        sensor.AddObservation(distance - cameraVector2);

        //Posicion nube actual
        distance = new Vector2((m_GameManager.m_LastCloud != null) ? m_GameManager.m_LastCloud.transform.position.x : 0, (m_GameManager.m_LastCloud != null) ?  m_GameManager.m_LastCloud.transform.position.y : 0);
        sensor.AddObservation(distance - cameraVector2);
        //Posicion nube siguiente
        distance = new Vector2((m_GameManager.newCloud!=null) ? m_GameManager.newCloud.transform.position.x : 0, (m_GameManager.newCloud!=null)? m_GameManager.newCloud.transform.position.y:0);
        sensor.AddObservation(distance - cameraVector2);

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        int direcion = actions.DiscreteActions[0] -1;
        m_Rigidbody.velocity = new Vector2(direcion * speed, m_Rigidbody.velocity.y);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int direction = 1;
        if (Input.GetKey(KeyCode.A)) direction -= 1;
        if (Input.GetKey(KeyCode.D)) direction += 1;

        ActionSegment<int> actions = actionsOut.DiscreteActions;
        actions[0] = direction;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Plataforma")
        {
            AddReward(-5f);
            if (m_NubesTransform.childCount==0)
            {
                GameObject nube = Instantiate(m_NubePrincipal, m_NubesTransform);
                nube.transform.position = new Vector2(m_PlataformaTransform.position.x, m_PlataformaTransform.position.y + 1.5f);
            }
            m_Rigidbody.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
            if (!floorTouched) floorTouched = true;
        }

        if (collision.gameObject.tag == "Nube")
        {
            if (collision.gameObject.name == "Tocada") AddReward(-10f);
            else
            {
                OnObstacleEvasion?.Invoke();
                AddReward(0.1f);
            }

            BoxCollider2D platformCollider = collision.gameObject.GetComponent<BoxCollider2D>();
            Vector2 collisionPoint = collision.GetContact(0).point;
            if (collisionPoint.y > platformCollider.bounds.max.y - 0.1f)
            {
                m_Rigidbody.velocity= Vector2.zero;
                m_Rigidbody.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
                // OnNewJump?.Invoke(collision.gameObject);
                m_GameManager.GenerateCloud(collision.gameObject);
                collision.gameObject.name = "Tocada";
            }
        }


    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Muerte")
        {
            DeleteWhenDead();
            EndEpisode();
            floorTouched = false;
            OnPouDead?.Invoke();
        }
    }
   /* private void OnCollisionStay2D(Collision2D collision)
    {
        print("ssss");
    }*/
    private void DeleteWhenDead()
    {
        for (int i = 0; i < m_MuerteTransform.childCount; ++i)
            Destroy(m_MuerteTransform.GetChild(i).gameObject);

        for (int i = 0; i < m_NubesTransform.childCount; ++i)
            Destroy(m_NubesTransform.GetChild(i).gameObject);
    }
}
