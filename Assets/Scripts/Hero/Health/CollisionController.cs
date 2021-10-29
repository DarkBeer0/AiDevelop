using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CollisionController : MonoBehaviour
{
    public HealthBar health;
    
    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.tag == "Zombie")
        {
            
                health.onTakeDamage(10);
        
        }
    }
}
