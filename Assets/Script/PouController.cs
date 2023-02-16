using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

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

    public delegate void NewJump(GameObject cloud);
    public static NewJump OnNewJump;
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_InitialPosition = transform.position;
    }
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(horizontalInput, 0f, 0f);
        movement *= speed * Time.fixedDeltaTime;
        transform.position += movement;
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
        sensor.AddObservation(distance);

        //Posicion nube actual
        distance = new Vector2(m_GameManager.m_LastCloud.transform.position.x, m_GameManager.m_LastCloud.transform.position.y);
        sensor.AddObservation(distance);

        //Posicion nube siguiente
        distance = new Vector2(m_GameManager.newCloud.transform.position.x, m_GameManager.newCloud.transform.position.y);
        sensor.AddObservation(distance);

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        int direcion = actions.DiscreteActions[0] -1;
        m_Rigidbody.velocity = Vector2.right * direcion * speed;
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
        if (collision.gameObject.tag == "Plataforma") m_Rigidbody.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);

        if (collision.gameObject.tag == "Nube")
        {
            if (collision.gameObject.name == "Tocada") AddReward(-5f);
            else AddReward(0.1f);

            BoxCollider2D platformCollider = collision.gameObject.GetComponent<BoxCollider2D>();
            Vector2 collisionPoint = collision.GetContact(0).point;
            if (collisionPoint.y > platformCollider.bounds.max.y - 0.1f)
            {
                m_Rigidbody.velocity= Vector2.zero;
                m_Rigidbody.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
                OnNewJump?.Invoke(collision.gameObject);
                collision.gameObject.name = "Tocada";
            }
        }


    }
}
