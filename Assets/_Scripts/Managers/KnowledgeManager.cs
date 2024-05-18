using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgeManager : MonoBehaviour
{
    bool isCheckKnowledge = true;

    [Header("Knowledge Marker Management")]
    [SerializeField] GameObject KnowledgeMarker;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] List<GameObject> spawnLocations;
    [SerializeField] float maxDistanceAboveGround = 1f;
    float x, y, z;
    GameObject NewKnowledgeObject;

    [Header("Knowledge Panel Management")]
    [SerializeField] GameObject[] Panels;
    [SerializeField] NavigationSystem navigation;
    [SerializeField] TextMeshProUGUI TextTitle;
    [SerializeField] TextMeshProUGUI TextAbout;

    [Header("Knowledge Question Management")]
    [SerializeField] TextMeshProUGUI TextOtherTitle, TextQuestion;
    [SerializeField] TextMeshProUGUI[] TextOptions;

    [Header("Knowledge Result Management")]
    [SerializeField] GameObject ResultPanel;
    [SerializeField] Image ImageResult;
    [SerializeField] Sprite[] SpriteResult;
    [SerializeField] TextMeshProUGUI TextResult, TextKnowledge;
    [TextArea]
    [SerializeField] string txtCorrect, txtIncorrect, txtMessage;

    // Start is called before the first frame update
    void Start()
    {
        if (DataSystem.Instance.User == null)
        {
            DataSystem.Instance.saveManager.SaveData();
        }
        else
        {
            CheckKnowledge();
        }
    }

    void CheckKnowledge()
    {
        // Debug.Log("is checking knowledge now...");
        if (DataSystem.Instance.User.currentKnowledge != null)
        {
            if ((DateTime.Now - DataSystem.Instance.User.currentKnowledge.startingAt.Value).TotalHours > 1)
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
        if (DataSystem.Instance.User != null)
        {
            if (isCheckKnowledge)
            {
                CheckKnowledge();
                isCheckKnowledge = false;
            }
            if (DataSystem.Instance.User.currentKnowledge != null)
            {
                if ((DateTime.Now - DataSystem.Instance.User.currentKnowledge.startingAt.Value).TotalHours > 1)
                {
                    ResertKnowledge();
                }
            }
        }
    }

    public void GenerateKnowledge()
    {
        // Debug.Log("is generating knowledge now...");
        if (DataSystem.Instance.Knowledge != null)
        {
            int randomIndex = UnityEngine.Random.Range(0, DataSystem.Instance.Knowledge.Count);
            DataSystem.Instance.User.currentKnowledge = new()
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

            SetPanelText();

            LocationManager lm = selectedArea.GetComponent<LocationManager>();
            string contentNotification = "Knowledge Baru Muncul!|Ayo jelajahi " + lm.location.name + " dan kumpulkan semua Knowledge!";
            navigation.ToggleNotification(contentNotification);
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

    void SetPanelText()
    {
        TextTitle.text = TextOtherTitle.text = "Tahukah Kamu #" + DataSystem.Instance.User.currentKnowledge.currentKnowledge.id + "?";
        TextAbout.text = TextQuestion.text = DataSystem.Instance.User.currentKnowledge.currentKnowledge.question.question;
        TextOptions[0].text = DataSystem.Instance.User.currentKnowledge.currentKnowledge.question.option[0];
        TextOptions[1].text = DataSystem.Instance.User.currentKnowledge.currentKnowledge.question.option[1];
    }

    public void TogglePanelQuiz()
    {
        foreach (GameObject Panel in Panels) Panel.SetActive(!Panel.activeSelf);
    }

    public void OnClickOption(int index)
    {
        if (DataSystem.Instance.User.currentKnowledge != null)
        {
            foreach (GameObject panel in Panels) panel.SetActive(false);
            Debug.Log(DataSystem.Instance.User.currentKnowledge.currentKnowledge.question.option[index] == DataSystem.Instance.User.currentKnowledge.currentKnowledge.question.answer);

            bool isCorrect = DataSystem.Instance.User.currentKnowledge.currentKnowledge.question.option[index] == DataSystem.Instance.User.currentKnowledge.currentKnowledge.question.answer;

            TextResult.text = isCorrect ? txtCorrect : txtIncorrect;
            ImageResult.sprite = SpriteResult[isCorrect ? 0 : 1];
            TextKnowledge.text = isCorrect ? DataSystem.Instance.User.currentKnowledge.currentKnowledge.explanation : txtMessage;

            ResultPanel.SetActive(true);

            OnSaveKnowledge();
        }
    }

    public void OnSaveKnowledge()
    {
        SaveKnowledge newSaved = new SaveKnowledge
        {
            id = DataSystem.Instance.User.currentKnowledge.currentKnowledge.id,
            isCollected = true
        };

        int index = -1;

        if (DataSystem.Instance.User != null && DataSystem.Instance.Knowledge != null) index = DataSystem.Instance.User.listOfSaveKnowledge.FindIndex(knowledge => knowledge.id == newSaved.id);

        if (index != -1)
        {
            bool check = DataSystem.Instance.User.listOfSaveKnowledge[index].isCollected;

            if (!check)
            {
                DataSystem.Instance.User.listOfSaveKnowledge[index].isCollected = newSaved.isCollected;
                DataSystem.Instance.saveManager.SaveData();
            }
        }
    }
}
