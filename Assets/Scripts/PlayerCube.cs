using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f; // Pr�dko�� ruchu
    public float turnSpeed = 100f; // Pr�dko�� obrotu
    public float pushForce = 5f; // Si�a popychania przejechanego obiektu
    public float destroyDelay = 3f; // Czas do usuni�cia obiektu po kolizji

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Pobieramy Rigidbody obiektu
    }

    void FixedUpdate()
    {
        // Ruch do przodu i do ty�u
        float moveInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDirection);

        // Obr�t w lewo i prawo, tylko gdy pojazd si� porusza
        if (Mathf.Abs(moveInput) > 0.01f) // Dodajemy warunek dla minimalnego ruchu
        {
            float turnInput = Input.GetAxis("Horizontal");
            Quaternion turnRotation = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Sprawdzamy, czy uderzono w obiekt o nazwie "Cube"
        if (collision.gameObject.name == "Cube")
        {
            // Sprawdzamy, czy obiekt nie zosta� wcze�niej trafiony
            Pedestrian pedestrian = collision.gameObject.GetComponent<Pedestrian>();
            if (pedestrian != null && !pedestrian.hasBeenHit)
            {
                Debug.Log("+1 punkt za przejechanie pieszego!");
                pedestrian.hasBeenHit = true; // Oznaczamy obiekt jako trafiony

                // Pobranie Rigidbody obiektu i zastosowanie si�y popychaj�cej
                Rigidbody pedestrianRb = collision.gameObject.GetComponent<Rigidbody>();
                if (pedestrianRb != null)
                {
                    Vector3 pushDirection = collision.transform.position - transform.position;
                    pushDirection.y = 0; // Si�a popychania tylko w p�aszczy�nie XZ
                    pedestrianRb.AddForce(pushDirection.normalized * pushForce, ForceMode.Impulse);
                }

                // Usuni�cie obiektu po okre�lonym czasie
                Destroy(collision.gameObject, destroyDelay);
            }
        }
    }
}
