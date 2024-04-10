using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefMovement : MonoBehaviour
{
    public float m_Speed = 12f;                 // How fast the tank moves forward and back.
    public float m_TurnSpeed = 180f;

    private Rigidbody m_rigidbody;
    private float m_MovementInputValue;       
    private float m_TurnInputValue;
    private float m_AIMovement; // The current value of the AI's movement input.

    private float m_AITurn; // The current value of the AI's turn input.
    // Start is called before the first frame update
    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        m_MovementInputValue = m_AIMovement;
        m_TurnInputValue = m_AITurn;
    }
    private void FixedUpdate()
    {
        // Adjust the rigidbodies position and orientation in FixedUpdate.
        Move();
        Turn();
    }
    private void Move()
    {
        // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

        // Apply this movement to the rigidbody's position.
        m_rigidbody.MovePosition(m_rigidbody.position + movement);
    }


    private void Turn()
    {
        // Determine the number of degrees to be turned based on the input, speed and time between frames.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        // Make this into a rotation in the y axis.
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        // Apply this rotation to the rigidbody's rotation.
        m_rigidbody.MoveRotation(m_rigidbody.rotation * turnRotation);
    }
    public void AIMove(float move)
    {
        // Clamp the value to [-1,1] 
        m_AIMovement = (move > 1) ? 1 : (move < -1) ? -1 : move;
    }

    public void AITurn(float turn)
    {
        // Clamp the value to [-1,1] 
        m_AITurn = (turn > 1) ? 1 : (turn < -1) ? -1 : turn;
    }
}
