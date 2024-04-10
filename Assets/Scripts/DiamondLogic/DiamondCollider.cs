using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log("something");
        if(collider.CompareTag("Thief"))
        {
            this.gameObject.SetActive(false);
        }
    }
}
