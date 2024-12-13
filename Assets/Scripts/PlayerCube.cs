using UnityEngine;
using UnityEngine.UI; // Dodajemy, je�li b�dziemy korzysta� z UI

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f; // Pr�dko�� ruchu
    public float turnSpeed = 100f; // Pr�dko�� obrotu
    public float pushForce = 5f; // Si�a popychania przejechanego obiektu
    public float destroyDelay = 3f; // Czas do usuni�cia obiektu po kolizji

    public AudioSource engineAudio; // Referencja do komponentu AudioSource
    public AudioClip engineSound; // Plik d�wi�kowy silnika

    public float maxSpeed = 100f;    // Maksymalna pr�dko�� pojazdu
    public float minPitch = 1f;      // Minimalny ton d�wi�ku silnika
    public float maxPitch = 3f;      // Maksymalny ton d�wi�ku silnika
    public float minVolume = 0.1f;   // Minimalna g�o�no�� d�wi�ku silnika
    public float maxVolume = 1f;     // Maksymalna g�o�no�� d�wi�ku silnika

    public ParticleSystem smokeEffect; // Referencja do systemu cz�steczek dymu

    public int points = 0; // Zmienna przechowuj�ca liczb� punkt�w
    public Text pointsText; // Referencja do UI Text, w kt�rym b�d� wy�wietlane punkty

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Pobieramy Rigidbody obiektu

        if (engineAudio == null)
        {
            engineAudio = GetComponent<AudioSource>();
        }

        // Ustawienie d�wi�ku silnika
        engineAudio.clip = engineSound;
        engineAudio.loop = true; // D�wi�k silnika b�dzie w p�tli
        engineAudio.Play();      // Rozpoczynamy odtwarzanie d�wi�ku silnika

        // Sprawdzamy, czy system dymu jest przypisany
        if (smokeEffect == null)
        {
            Debug.LogError("Smoke effect not assigned! Please assign it in the Inspector.");
        }

        // Je�li nie mamy UI Text, wy�wietlimy komunikat b��du
        if (pointsText == null)
        {
            Debug.LogError("Points UI Text not assigned! Please assign it in the Inspector.");
        }
        else
        {
            UpdatePointsUI(); // Na pocz�tku ustawiamy UI punkt�w
        }
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

        // Obliczanie pr�dko�ci pojazdu
        float speed = rb.velocity.magnitude;

        // Normalizowanie pr�dko�ci do zakresu 0-1
        float speedNormalized = Mathf.InverseLerp(0, maxSpeed, speed);

        // Zmienianie pitch d�wi�ku w zale�no�ci od pr�dko�ci
        engineAudio.pitch = Mathf.Lerp(minPitch, maxPitch, speedNormalized);

        // Zmienianie g�o�no�ci d�wi�ku w zale�no�ci od pr�dko�ci
        engineAudio.volume = Mathf.Lerp(minVolume, maxVolume, speedNormalized);

        // Je�li pr�dko�� pojazdu jest wi�ksza ni� 0, uruchamiamy efekt dymu
        if (speed > 0.1f && !smokeEffect.isPlaying)
        {
            smokeEffect.Play(); // Rozpoczynamy emisj� dymu
        }
        else if (speed <= 0.1f && smokeEffect.isPlaying)
        {
            smokeEffect.Stop(); // Zatrzymujemy dym, gdy pojazd stoi
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
                // Zwi�kszamy liczb� punkt�w
                points++;
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

                // Aktualizacja UI punkt�w
                UpdatePointsUI();
            }
        }
    }

    // Funkcja do aktualizacji UI
    void UpdatePointsUI()
    {
        if (pointsText != null)
        {
            pointsText.text = "Punkty: " + points.ToString(); // Wy�wietlamy punkty na ekranie
        }
    }
}
