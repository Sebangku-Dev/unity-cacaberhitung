using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnowledgeManager : MonoBehaviour
{
    [SerializeField] GameObject KnowledgeMarker;
    public LayerMask groundLayer;
    public List<GameObject> spawnLocations;
    public float maxDistanceAboveGround = 1f;
    float x, y, z;
    GameObject NewKnowledgeObject;

    [SerializeField] GameObject[] Panels;

    [SerializeField] NavigationSystem navigation;

    // Start is called before the first frame update
    void Start()
    {
        if (DataSystem.Instance.currentKnowledge != null)
        {
            if ((DateTime.Now - DataSystem.Instance.currentKnowledge.startingAt.Value).TotalHours > 1)
            {
                GenerateKnowledge();
            }
            else
            {
                SpawnKnowledge();
            }
        }
        else
        {
            GenerateKnowledge();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (DataSystem.Instance.currentKnowledge != null)
        {
            if ((DateTime.Now - DataSystem.Instance.currentKnowledge.startingAt.Value).TotalHours > 1)
            {
                ResertKnowledge();
            }
        }
    }

    public void GenerateKnowledge()
    {
        if (DataSystem.Instance.Knowledge != null)
        {
            int randomIndex = UnityEngine.Random.Range(0, DataSystem.Instance.Knowledge.Count);
            DataSystem.Instance.currentKnowledge = new()
            {
                currentKnowledge = DataSystem.Instance.Knowledge[randomIndex],
                startingAt = DateTime.Now
            };

            SpawnKnowledge();
        }
    }

    void SpawnKnowledge()
    {
        // Pilih area secara acak
        GameObject selectedArea = spawnLocations[UnityEngine.Random.Range(0, spawnLocations.Count)];

        // Buat marker sebagai child dari area yang dipilih
        NewKnowledgeObject = Instantiate(KnowledgeMarker);
        NewKnowledgeObject.transform.SetParent(selectedArea.transform, false);

        // Tentukan posisi acak di dalam collider area
        Collider areaCollider = selectedArea.GetComponent<Collider>();
        Vector3 randomPosition = GetRandomPositionInsideCollider(areaCollider);

        // Cek apakah posisi acak berada di atas permukaan ground
        if (IsAboveGround(randomPosition, out Vector3 groundPosition))
        {
            // Pastikan marker tidak lebih dari maxDistanceAboveGround di atas ground
            if (randomPosition.y - groundPosition.y <= maxDistanceAboveGround)
            {
                NewKnowledgeObject.transform.position = randomPosition;
            }
            else
            {
                // Jika terlalu tinggi, set posisi marker ke groundPosition + maxDistanceAboveGround
                NewKnowledgeObject.transform.position = new Vector3(randomPosition.x, groundPosition.y + maxDistanceAboveGround, randomPosition.z);
            }

            KnowledgeController controller = NewKnowledgeObject.GetComponent<KnowledgeController>();
            if (controller) controller.navigation = navigation;

            navigation.ToggleNotification();
        }
        else
        {
            // Jika tidak, ulangi proses untuk mendapatkan posisi yang valid
            Destroy(NewKnowledgeObject);
            SpawnKnowledge();
        }
    }

    Vector3 GetRandomPositionInsideCollider(Collider collider)
    {
        Vector3 minBounds = collider.bounds.min;
        Vector3 maxBounds = collider.bounds.max;

        Vector3 randomPosition = new Vector3(
            UnityEngine.Random.Range(minBounds.x, maxBounds.x),
            UnityEngine.Random.Range(minBounds.y, maxBounds.y),
            UnityEngine.Random.Range(minBounds.z, maxBounds.z)
        );

        // Pastikan posisi berada di dalam collider
        if (collider.bounds.Contains(randomPosition))
        {
            return randomPosition;
        }
        else
        {
            return GetRandomPositionInsideCollider(collider);
        }
    }

    bool IsAboveGround(Vector3 position, out Vector3 groundPosition)
    {
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            groundPosition = hit.point;
            return hit.collider != null;
        }
        groundPosition = Vector3.zero;
        return false;
    }

    void ResertKnowledge()
    {
        Destroy(NewKnowledgeObject);
        SpawnKnowledge();
    }

    public void TogglePanelQuiz()
    {
        foreach (GameObject Panel in Panels) Panel.SetActive(!Panel.activeSelf);
    }
}
