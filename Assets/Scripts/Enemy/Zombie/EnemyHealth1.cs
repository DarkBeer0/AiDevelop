using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth1 : MonoBehaviour
{
    // Start is called before the first frame update
    public float health;
    public Slider slider;
    public float startHealth;

    public void onTakeDamage(int damage)
    {
        Debug.Log(damage);
        health = health - damage;
        slider.value = health;
        Debug.Log(health);
    }
}
