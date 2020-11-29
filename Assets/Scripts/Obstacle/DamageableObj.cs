using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class DamageableObj : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerCtlr p = other.gameObject.GetComponent<PlayerCtlr>();
            p.IPDamage();
        }
    }

}
