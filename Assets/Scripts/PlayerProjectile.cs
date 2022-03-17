using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    private Decoy decoy;
    public GameObject target;
    private float projectileSpeed = 10;
    private EnemyHealthManager targetHealth;
    void Start()
    {
        // decoy = FindObjectOfType<Decoy>();
        // player = FindObjectOfType<PlayerHealthManager>();
        target = FindObjectOfType<EnemyController>().gameObject;
        
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition += transform.forward*Time.deltaTime*projectileSpeed;
        
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
            if((transform.position - target.transform.position).magnitude <= 1.5) 
            {
                targetHealth = target.GetComponent<EnemyHealthManager>();
                targetHealth.DamageTaken(20);
                Destroy(this.gameObject);
            }
        }

        
    }
}
