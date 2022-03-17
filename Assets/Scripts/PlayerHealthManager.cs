using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    public int playerHealth = 100;
    private PlayerMovement player;
    void Start()
    {
        player = GetComponent<PlayerMovement>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerHealth == 0) player.PlayerDie();
    }

    public int DamageTaken(int damage){

        playerHealth -= damage;
        return playerHealth;
    }
}
