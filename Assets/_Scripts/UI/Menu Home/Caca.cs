using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caca : MonoBehaviour
{
    [SerializeField] Animator animator;
    void Start()
    {
        animator.SetBool("isWalking", true);
    }

}
