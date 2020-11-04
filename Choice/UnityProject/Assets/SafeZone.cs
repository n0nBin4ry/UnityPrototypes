using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    EventManager eventManager;
    public GameObject eventManagerObj;
      private void Start() {
          eventManager = eventManagerObj.GetComponent<EventManager>();
      }
      private void OnTriggerEnter2D(Collider2D other) { //when actors are inside of the safe zone they are added to toSave
        if(other.tag == "actor"){
            eventManager.toSave.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "actor"){
            eventManager.toSave.Remove(other.gameObject);
        }
    }

    

}
