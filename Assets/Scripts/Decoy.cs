using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoy : MonoBehaviour
{
    private float lifeSpan = 5;
    private int health = 1;
    void Start()
    {
        
    }

    void Update()
    {
        if(health <= 0) Destroy(this.gameObject);
        lifeSpan -= Time.deltaTime;
        if(lifeSpan <= 0) Destroy(this.gameObject);
    }

    public int TakeDamage(int damage)
    {
        health -= damage;
        return health;
        
    }
}
