using UnityEngine;

public class CubeMover : MonoBehaviour
{
    public float moveSpeed = 10f; // Prêdkoœæ poruszania siê
    public float jumpForce = 5f; // Si³a skoku

    private Rigidbody rb;

    void Start()
    {
        // Pobranie komponentu Rigidbody
        rb = GetComponent<Rigidbody>();
    }

   

    void Update()
    {
        // Skok (jeœli gracz naciœnie spacjê)
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // Sprawdzanie, czy kostka dotyka ziemi
    private bool IsGrounded()
    {
        // Raycast poni¿ej Cube, by sprawdziæ, czy dotyka ziemi
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            rb.AddForce(Vector3.up * 1000 + Vector3.forward * 20);
        }        
    }
}
