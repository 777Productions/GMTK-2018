using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IcicleSpawner : MonoBehaviour
{

    public GameObject[] iciclePrefabs;

    public float spawnWidth = 20;

    [Tooltip("Probability of an icicle spawning every second.")]
    [Range(0, 1)]
    public float spawnRate = 0.1f;

    public float minSpawnSize = 0.5f;
    public float maxSpawnSize = 2.0f;

    public float minInitialVelocity = 0;
    public float maxInitialVelocity = 1;

    public Text warningText;

    private int playersInRange = 0;

    private bool hasWarned = false;

    // Update is called once per frame
    void Update()
    {
        if (playersInRange > 0 && Random.Range(0.0f, 1.0f) <= spawnRate * Time.deltaTime)
        {
            SpawnIcicle();
        }
    }

    private void SpawnIcicle()
    {
        Vector2 spawnPosition = GetSpawnPosition();
        Vector2 initialVelocity = GetSpawnVelocity();
        float size = GetSpawnSize();

        GameObject icicle = Instantiate(iciclePrefabs[Random.Range(0, iciclePrefabs.Length)], spawnPosition, Quaternion.identity, this.transform);
        icicle.transform.localScale = size * Vector3.one;
        icicle.GetComponent<Rigidbody2D>().velocity = initialVelocity;
    }

    private Vector2 GetSpawnPosition()
    {
        float xPos = Random.Range(-spawnWidth / 2.0f, spawnWidth / 2.0f);

        Vector2 position = new Vector2(xPos, transform.position.y);

        return position;
    }

    private Vector2 GetSpawnVelocity()
    {
        Vector2 velocity = new Vector2(0.0f, Random.Range(minInitialVelocity, maxInitialVelocity));
        return velocity;
    }

    private float GetSpawnSize()
    {
        float size = Random.Range(minSpawnSize, maxSpawnSize);
        return size;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();
        if (playerController)
        {
            playersInRange += 1;
            if (!hasWarned)
            {
                hasWarned = true;
                StartCoroutine(flashWarning());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();
        if (playerController)
        {
            playersInRange -= 1;
        }
    }

    private IEnumerator flashWarning()
    {
        warningText.enabled = true;

        warningText.color = new Color(warningText.color.r, warningText.color.g, warningText.color.b, 0);

        for (int flashes = 5; flashes > 0; flashes--)
        {
            while (warningText.color.a < 1.0f)
            {
                warningText.color = new Color(warningText.color.r, warningText.color.g, warningText.color.b, warningText.color.a + (Time.deltaTime / 0.25f));
                yield return null;
            }
            while (warningText.color.a > 0.0f)
            {
                warningText.color = new Color(warningText.color.r, warningText.color.g, warningText.color.b, warningText.color.a - (Time.deltaTime / 0.255f));
                yield return null;
            }
        }

        warningText.enabled = false;
    }
}
