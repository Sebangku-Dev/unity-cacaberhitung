using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Caca : MonoBehaviour
{
    [SerializeField] Animator animator;

    async void Start()
    {
        while (true)
        {
            int randNum = Random.Range(0, 2);
            animator.SetTrigger(randNum == 0 ? "Jump" : "Confuse");
            await Task.Delay(6000);
        }
    }


}
