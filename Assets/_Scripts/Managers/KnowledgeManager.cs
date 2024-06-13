using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgeManager : MonoBehaviour
{
    bool isCheckKnowledge = true;
    bool isResetKnowledge, isStartReset = false;
    Knowledge currentKnowledge;

    [Header("Knowledge Marker Management")]
    [SerializeField] GameObject KnowledgeMarker;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] List<GameObject> spawnLocations;
    [SerializeField] float maxDistanceAboveGround = 1f;
    float x, y, z;
    GameObject NewKnowledgeObject;
    GameObject selectedArea;
    LocationManager lm;

    [Header("Knowledge Info Management")]
    [SerializeField] GameObject MiniInfo;
    [SerializeField] TextMeshProUGUI TextLocation, TextTimer;
    [SerializeField] GameObject ButtonKnowledgeHint;

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

    #region Knowledge Check Handler
    // Update is called once per frame
    void Update()
    {
        if (UserManager.Instance.User != null)
        {
            if (isCheckKnowledge)
            {
                CheckKnowledge();
                isCheckKnowledge = false;
            }

            else
            {
                if (UserManager.Instance.User.knowledgeHasSpawn)
                {
                    if (IsMoreThanAnHour() && !isStartReset)
                    {
                        Debug.Log("Current knowledge is time over...");
                        ResetKnowledge();
                        isStartReset = true;
                    }
                }
            }
        }
    }

    void CheckKnowledge()
    {
        Debug.Log("Is checking knowledge now...");
        if (UserManager.Instance.User.knowledgeHasSpawn)
        {
            Debug.Log("Knowledge is found...");

            if (IsMoreThanAnHour())
            {
                Debug.Log("But time is over...");

                ResetKnowledge();
            }
            else
            {
                currentKnowledge = DataSystem.Instance.Knowledge[UserManager.Instance.User.currentKnowledge.id];
                selectedArea = spawnLocations[UserManager.Instance.User.currentKnowledge.areaId];
                lm = selectedArea.GetComponent<LocationManager>();

                if (UserManager.Instance.User.currentKnowledge.isAnswered)
                {
                    Debug.Log("Current knowledge has answered...");

                    string contentNotification = "Knowledge sebelumnya telah dikerjakan!|Tunggu beberapa saat sebelum knowledge baru muncul... ";
                    navigation.ToggleNotification(contentNotification);

                    SetPanelText();
                }
                else
                {
                    Debug.Log("Respawn current knowledge...");

                    SpawnKnowledge(false);
                }
            }
        }
        else
        {
            Debug.Log("There is no Knowledge...");
            GenerateKnowledge();
        }
    }

    public bool IsMoreThanAnHour()
    {
        DateTime now = DateTime.Now;

        TimeSpan timeSpan = now - UserManager.Instance.User.currentKnowledge.startingAt;

        TextTimer.text = "Timer: " + timeSpan.ToString(@"mm\:ss");

        return timeSpan.TotalHours > 1;
    }
    #endregion

    #region KnowledgeMarker Handler
    public void GenerateKnowledge()
    {
        Debug.Log("Is generating knowledge now...");
        if (DataSystem.Instance.Knowledge != null)
        {
            int randomIndex = UnityEngine.Random.Range(0, DataSystem.Instance.Knowledge.Count);

            int areaId = UnityEngine.Random.Range(0, spawnLocations.Count);
            selectedArea = spawnLocations[areaId];
            lm = selectedArea.GetComponent<LocationManager>();

            UserManager.Instance.User.currentKnowledge = new()
            {
                id = DataSystem.Instance.Knowledge[randomIndex].id,
                areaId = areaId,
                startingAt = DateTime.Now
            };

            UserManager.Instance.User.knowledgeHasSpawn = true;

            UserManager.Instance.Save();

            currentKnowledge = DataSystem.Instance.Knowledge[randomIndex];

            isResetKnowledge = false;
            isStartReset = false;

            SpawnKnowledge(true);
        }
    }

    void SpawnKnowledge(bool isNew)
    {
        NewKnowledgeObject = Instantiate(KnowledgeMarker);
        NewKnowledgeObject.transform.SetParent(selectedArea.transform, false);

        if (!isNew)
        {
            Vector3 savedPosition = new Vector3(
                UserManager.Instance.User.currentKnowledge.x,
                UserManager.Instance.User.currentKnowledge.y,
                UserManager.Instance.User.currentKnowledge.z
            );

            NewKnowledgeObject.transform.position = savedPosition;
            OnKnowledgeSpawned();

            UserManager.Instance.Save();
        }
        else TransformNewKnowledge();
    }

    void ResetKnowledge()
    {
        if (NewKnowledgeObject != null) Destroy(NewKnowledgeObject);

        string contentNotification = "Knowledge sebelumnya telah hilang!|Tunggu beberapa saat lagi akan muncul Knowledge baru";
        navigation.ToggleNotification(contentNotification);

        if (!isResetKnowledge)
        {
            Invoke(nameof(GenerateKnowledge), 5.0f);
            isResetKnowledge = true;
        }
    }
    #endregion

    #region Randomize Knowledge Position Handler
    void TransformNewKnowledge()
    {
        Collider areaCollider = selectedArea.GetComponent<Collider>();
        Vector3 randomPosition = GetRandomPositionInsideCollider(areaCollider);

        if (IsAboveGround(randomPosition, out Vector3 groundPosition))
        {
            if (randomPosition.y - groundPosition.y <= maxDistanceAboveGround)
            {
                NewKnowledgeObject.transform.position = randomPosition;
            }
            else
            {
                NewKnowledgeObject.transform.position = new Vector3(randomPosition.x, groundPosition.y + maxDistanceAboveGround, randomPosition.z);
            }

            UserManager.Instance.User.currentKnowledge.x = NewKnowledgeObject.transform.position.x;
            UserManager.Instance.User.currentKnowledge.y = NewKnowledgeObject.transform.position.y;
            UserManager.Instance.User.currentKnowledge.z = NewKnowledgeObject.transform.position.z;

            OnKnowledgeSpawned();
        }
        else
        {
            TransformNewKnowledge();
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
    #endregion

    #region Knowledge UI Handler
    void OnKnowledgeSpawned()
    {
        KnowledgeController controller = NewKnowledgeObject.GetComponent<KnowledgeController>();
        if (controller) controller.navigation = navigation;

        SetPanelText();

        string contentNotification = "Knowledge Baru Muncul!|Ayo jelajahi " + lm.location.name + " dan kumpulkan semua Knowledge!";
        navigation.ToggleNotification(contentNotification);
    }

    void SetPanelText()
    {
        TextTitle.text = TextOtherTitle.text = "Tahukah Kamu #" + currentKnowledge.id + "?";

        TextAbout.text = TextQuestion.text = currentKnowledge.question.question;

        TextOptions[0].text = currentKnowledge.question.option[0];
        TextOptions[1].text = currentKnowledge.question.option[1];

        TextLocation.text = lm.location.name;
    }

    public void ToggleInfo()
    {
        MiniInfo.SetActive(!MiniInfo.activeSelf);
        ButtonKnowledgeHint.SetActive(!ButtonKnowledgeHint.activeSelf);
    }

    #endregion

    #region Quiz Handler

    public void TogglePanelQuiz()
    {
        foreach (GameObject Panel in Panels) Panel.SetActive(!Panel.activeSelf);
    }

    public void OnClickOption(int index)
    {
        if (UserManager.Instance.User.currentKnowledge != null)
        {
            foreach (GameObject panel in Panels) panel.SetActive(false);
            // Debug.Log(currentKnowledge.question.option[index] == currentKnowledge.question.answer);

            bool isCorrect = currentKnowledge.question.option[index] == currentKnowledge.question.answer;

            TextResult.text = isCorrect ? txtCorrect : txtIncorrect;
            ImageResult.sprite = SpriteResult[isCorrect ? 0 : 1];
            TextKnowledge.text = isCorrect ? currentKnowledge.explanation : txtMessage;

            ResultPanel.SetActive(true);

            OnSaveKnowledge(isCorrect);
        }
    }

    public void OnSaveKnowledge(bool isCorrect)
    {
        SaveKnowledge newSaved = new SaveKnowledge
        {
            id = currentKnowledge.id,
            isCollected = isCorrect,
            collectedAt = DateTime.Now.ToString()
        };

        int index = -1;

        if (UserManager.Instance.User != null && DataSystem.Instance.Knowledge != null) index = UserManager.Instance.User.listOfSaveKnowledge.FindIndex(knowledge => knowledge.id == newSaved.id);

        if (index != -1)
        {
            UserManager.Instance.User.currentKnowledge.isAnswered = true;

            bool check = UserManager.Instance.User.listOfSaveKnowledge[index].isCollected;

            if (!check)
            {
                UserManager.Instance.User.listOfSaveKnowledge[index] = newSaved;
                UserManager.Instance.Save();
            }
        }

        if (NewKnowledgeObject != null) Destroy(NewKnowledgeObject);
    }

    #endregion
}
