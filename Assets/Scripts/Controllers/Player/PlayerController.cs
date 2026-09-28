using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
	[Space]
	[SerializeField] private int baseHitPoints = 10;							// Base hit points
	[SerializeField] public int hitPoints = 10;						            // Current hit points

    [Header("Params")]
	[Space]
    [SerializeField] private GameObject self;							        // Self

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hitPoints = baseHitPoints;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int TakeHit(int damage)
    {
        hitPoints = hitPoints - damage;
        Debug.Log("----Player took " + damage + "damages");
        Debug.Log("----Player has " + hitPoints + "hp remaining");
        IsPlayerDead();
        return hitPoints;
    }

    private void IsPlayerDead()
    {
        if(hitPoints <= 0)
        {
            Debug.Log("----Player is dead");
            //Destroy(self);
            self.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log("----Player collided");
    }
}
