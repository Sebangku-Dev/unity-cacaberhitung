using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;

    private void Start()
    {
        // Use below if you want to hard code it (not best practice)
        // anim = transform.Find("Door Mesh").gameObject.GetComponent<Animator>(); // Get door mesh
    }

    private void OnTriggerEnter(Collider other)
    {
        // btnOpenDoor?.SetActive(true);
        if (other.CompareTag("Player"))
            OpenDoor();
    }

    private void OnTriggerExit(Collider other)
    {
        // btnOpenDoor?.SetActive(false);
        if (other.CompareTag("Player"))
            CloseDoor();
    }

    private void OpenDoor()
    {
        doorAnimator.SetTrigger("open");
    }

    private void CloseDoor()
    {
        doorAnimator.SetTrigger("close");
    }
}
