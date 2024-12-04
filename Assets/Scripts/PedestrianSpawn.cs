using UnityEngine;

public class PedestrianSpawn : MonoBehaviour
{
    public GameObject pedestrianPrefab; // Prefab pieszych
    public Vector3 spawnAreaSize = new Vector3(10f, 0f, 10f); // Wymiary obszaru spawn
    public float spawnInterval = 3f; // Czas miêdzy kolejnymi spawnowaniami

    private float spawnTimer;

    void Update()
    {
        // Licznik czasu
        spawnTimer += Time.deltaTime;

        // Jeœli czas spawn przekroczony, generuj pieszego
        if (spawnTimer >= spawnInterval)
        {
            SpawnPedestrian();
            spawnTimer = 0f; // Resetujemy licznik
        }
    }

    void SpawnPedestrian()
    {
        // Losowa pozycja wewn¹trz obszaru spawn
        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
            spawnAreaSize.y,
            Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f)
        );

        // Generowanie nowego pieszego
        Instantiate(pedestrianPrefab, transform.position + spawnPosition, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        // Rysowanie obszaru spawn w edytorze Unity
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
