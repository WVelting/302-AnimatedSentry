using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    public int playerHealth = 100;
    private EnemyController player;
    void Start()
    {
        player = GetComponent<EnemyController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerHealth <= 0) player.EnemyDie();
    }

    public int DamageTaken(int damage){

        playerHealth -= damage;
        return playerHealth;
    }
}
