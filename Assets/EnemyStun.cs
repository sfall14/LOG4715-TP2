using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStun : MonoBehaviour {

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlayerControler>().stompAbility)
        {
            this.GetComponentInParent<EnnemiBehaviour>().Stunned();
            other.gameObject.GetComponent<PlayerControler>().stompAbility = false;
        }else if (other.gameObject.tag == "Player" && !other.gameObject.GetComponent<PlayerControler>().stompAbility)
        {
            other.gameObject.GetComponent<PlayerControler>().isDamaged = true;
        }
    } 
}
