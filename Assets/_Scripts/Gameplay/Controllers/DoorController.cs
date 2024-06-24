using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private GameObject relatedBarrier;

    private void Start()
    {
        // Use below if you want to hard code it (not best practice)
        // anim = transform.Find("Door Mesh").gameObject.GetComponent<Animator>(); // Get door mesh
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !relatedBarrier.activeSelf)
        {
            OpenDoor();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            CloseDoor();
    }

    private void OpenDoor()
    {
        doorAnimator.SetBool("isOpen", true);
    }

    private void CloseDoor()
    {
        doorAnimator.SetBool("isOpen", false);
    }
}
