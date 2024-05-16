using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnowledgeController : MonoBehaviour
{
    [SerializeField] GameObject BookObject;
    public float rotationSpeed = 90.0f;

    [SerializeField] public NavigationSystem navigation;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        BookObject.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player")) navigation.TogglePanel(2);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player")) navigation.TogglePanel(2);
    }
}
