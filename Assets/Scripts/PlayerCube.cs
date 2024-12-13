using UnityEngine;
using UnityEngine.UI; // Dodajemy, jeœli bêdziemy korzystaæ z UI

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f; // Prêdkoœæ ruchu
    public float turnSpeed = 100f; // Prêdkoœæ obrotu
    public float pushForce = 5f; // Si³a popychania przejechanego obiektu
    public float destroyDelay = 3f; // Czas do usuniêcia obiektu po kolizji

    public AudioSource engineAudio; // Referencja do komponentu AudioSource
    public AudioClip engineSound; // Plik dŸwiêkowy silnika

    public float maxSpeed = 100f;    // Maksymalna prêdkoœæ pojazdu
    public float minPitch = 1f;      // Minimalny ton dŸwiêku silnika
    public float maxPitch = 3f;      // Maksymalny ton dŸwiêku silnika
    public float minVolume = 0.1f;   // Minimalna g³oœnoœæ dŸwiêku silnika
    public float maxVolume = 1f;     // Maksymalna g³oœnoœæ dŸwiêku silnika

    public ParticleSystem smokeEffect; // Referencja do systemu cz¹steczek dymu

    public int points = 0; // Zmienna przechowuj¹ca liczbê punktów
    public Text pointsText; // Referencja do UI Text, w którym bêd¹ wyœwietlane punkty

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Pobieramy Rigidbody obiektu

        if (engineAudio == null)
        {
            engineAudio = GetComponent<AudioSource>();
        }

        // Ustawienie dŸwiêku silnika
        engineAudio.clip = engineSound;
        engineAudio.loop = true; // DŸwiêk silnika bêdzie w pêtli
        engineAudio.Play();      // Rozpoczynamy odtwarzanie dŸwiêku silnika

        // Sprawdzamy, czy system dymu jest przypisany
        if (smokeEffect == null)
        {
            Debug.LogError("Smoke effect not assigned! Please assign it in the Inspector.");
        }

        // Jeœli nie mamy UI Text, wyœwietlimy komunikat b³êdu
        if (pointsText == null)
        {
            Debug.LogError("Points UI Text not assigned! Please assign it in the Inspector.");
        }
        else
        {
            UpdatePointsUI(); // Na pocz¹tku ustawiamy UI punktów
        }
    }

    void FixedUpdate()
    {
        // Ruch do przodu i do ty³u
        float moveInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDirection);

        // Obrót w lewo i prawo, tylko gdy pojazd siê porusza
        if (Mathf.Abs(moveInput) > 0.01f) // Dodajemy warunek dla minimalnego ruchu
        {
            float turnInput = Input.GetAxis("Horizontal");
            Quaternion turnRotation = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        // Obliczanie prêdkoœci pojazdu
        float speed = rb.velocity.magnitude;

        // Normalizowanie prêdkoœci do zakresu 0-1
        float speedNormalized = Mathf.InverseLerp(0, maxSpeed, speed);

        // Zmienianie pitch dŸwiêku w zale¿noœci od prêdkoœci
        engineAudio.pitch = Mathf.Lerp(minPitch, maxPitch, speedNormalized);

        // Zmienianie g³oœnoœci dŸwiêku w zale¿noœci od prêdkoœci
        engineAudio.volume = Mathf.Lerp(minVolume, maxVolume, speedNormalized);

        // Jeœli prêdkoœæ pojazdu jest wiêksza ni¿ 0, uruchamiamy efekt dymu
        if (speed > 0.1f && !smokeEffect.isPlaying)
        {
            smokeEffect.Play(); // Rozpoczynamy emisjê dymu
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
            // Sprawdzamy, czy obiekt nie zosta³ wczeœniej trafiony
            Pedestrian pedestrian = collision.gameObject.GetComponent<Pedestrian>();
            if (pedestrian != null && !pedestrian.hasBeenHit)
            {
                // Zwiêkszamy liczbê punktów
                points++;
                Debug.Log("+1 punkt za przejechanie pieszego!");
                pedestrian.hasBeenHit = true; // Oznaczamy obiekt jako trafiony

                // Pobranie Rigidbody obiektu i zastosowanie si³y popychaj¹cej
                Rigidbody pedestrianRb = collision.gameObject.GetComponent<Rigidbody>();
                if (pedestrianRb != null)
                {
                    Vector3 pushDirection = collision.transform.position - transform.position;
                    pushDirection.y = 0; // Si³a popychania tylko w p³aszczyŸnie XZ
                    pedestrianRb.AddForce(pushDirection.normalized * pushForce, ForceMode.Impulse);
                }

                // Usuniêcie obiektu po okreœlonym czasie
                Destroy(collision.gameObject, destroyDelay);

                // Aktualizacja UI punktów
                UpdatePointsUI();
            }
        }
    }

    // Funkcja do aktualizacji UI
    void UpdatePointsUI()
    {
        if (pointsText != null)
        {
            pointsText.text = "Punkty: " + points.ToString(); // Wyœwietlamy punkty na ekranie
        }
    }
}
