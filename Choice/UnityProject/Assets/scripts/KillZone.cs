using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
     public List<GameObject> toDestroy = new List<GameObject>(); //list of objects to destroy, or to do something else with. 

    public GameObject cover; //temp red square that fades up over actors inside of the killzone as they are destroyed

  
    EventManager eventManager;
    public GameObject eventManagerObj;
    private void Start() {
        eventManager = eventManagerObj.GetComponent<EventManager>();
    }
    //keeps track of what actors are inside of the killzone
    private void OnTriggerEnter2D(Collider2D other) { 
        if(other.tag == "actor"){
            toDestroy.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "actor"){
            toDestroy.Remove(other.gameObject);
        }
    }
    
    public void DestroyActors(){ //destroys all actors inside of the killzone - our default action
       StartCoroutine("OnDestroyActors");
    }

    IEnumerator OnDestroyActors(){ //increases cover opacity until it covers the actors then destroys actors + plays sfx
        Color c = cover.GetComponent<SpriteRenderer>().color; 
        while(c.a < 1.0f){
            c.a += 1 * Time.deltaTime;
            cover.GetComponent<SpriteRenderer>().color = c;
            c = cover.GetComponent<SpriteRenderer>().color;
            yield return null;
        }
        
        this.GetComponent<AudioSource>().Play();
        while(toDestroy.Count > 0){
            GameObject g = toDestroy[0];
            toDestroy.Remove(g);
            Destroy(g);
        }

      eventManager.ReturnAll(); 
       

        cover.GetComponent<SpriteRenderer>().color = new Color(c.r, c.b, c.g, 0);
    }
}
