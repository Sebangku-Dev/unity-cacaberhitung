using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BounceAnimation : BaseAnimation, IAnimate
{
    [SerializeField] Vector3 delta;
    [SerializeField] private float jumpAmount = 1000f;

    private Rigidbody2D rb;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();

        if (isAnimateOnLoad) Load();
    }


    public void Close()
    {
        throw new System.NotImplementedException();
    }

    public async void Load()
    {
        for (int i = 0; i < 5; i++)
        {
            await Bounce();
        }
    }

    public async Task Bounce()
    {
        await Task.Delay(1500);
        rb.AddRelativeForce(Vector3.up * jumpAmount * Time.deltaTime);
        await Task.Delay(1500);
    }
}
