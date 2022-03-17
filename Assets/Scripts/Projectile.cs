using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Decoy decoy;
    private PlayerHealthManager player;
    private float projectileSpeed = 10;
    void Start()
    {
        decoy = FindObjectOfType<Decoy>();
        player = FindObjectOfType<PlayerHealthManager>();
        
        
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition += Vector3.forward*Time.deltaTime*projectileSpeed;
        
        if(decoy)
        {
            
            if((transform.position - decoy.transform.position).magnitude <= 1.5) 
            {
                Destroy(decoy.gameObject);
                Destroy(this.gameObject);
            }
        }

        if(!decoy)
        {
            if((transform.position - player.transform.position).magnitude <= 1.5) 
            {
                player.DamageTaken(20);
                Destroy(this.gameObject);
            }
        }

        
    }
}
