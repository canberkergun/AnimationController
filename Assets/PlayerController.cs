using System;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerController
{
    public Vector2 Input { get; private set; }
    public Vector2 Speed { get; private set; }
    public bool Crouching { get; private set; }
    public bool isJumped { get; set; }

    public event Action Crouched;
    public event Action Jumped;
    public event Action Walked;

    public Rigidbody rb;
    public int speed;
    public float jumpForce = 10f;
    private void Update()
    {
        // Simulate player input (Replace with actual input handling)
        float xValue = UnityEngine.Input.GetAxis("Horizontal");
        float yValue = 0;

        if (!isJumped) 
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) || UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                isJumped = true;
                Jumped?.Invoke();
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
        
        
        Input = new Vector2(xValue, 0);
        
        
        if (rb.velocity.x < 4f && rb.velocity.x > -4f)
        {
            rb.AddForce(Input * (Time.deltaTime * speed), ForceMode.Impulse);
        }
        
        if (xValue != 0)
        {
            Walked.Invoke();
        }
        
        Crouching = UnityEngine.Input.GetKey(KeyCode.LeftControl);

        if (Crouching)
        {
            Crouched.Invoke();
        }

        Speed = rb.velocity;
        
        //Debug.Log("Input= " + Input.x + "\nSpeed= "+ Speed.x);
    }
}