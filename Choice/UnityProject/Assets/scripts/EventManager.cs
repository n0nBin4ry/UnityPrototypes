using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    int numRounds = 0; //the number of rounds that have gone by
    public string activateKey = "space"; //trigger button for testing purposes
    public GameObject killZoneObj; //the killzone
    KillZone kZone; //the killzone script on our killzone

    public List<GameObject> toSave = new List<GameObject>(); //actors to save

    // Start is called before the first frame update
    void Start()
    {
        kZone = killZoneObj.GetComponent<KillZone>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(activateKey)){
            OnActivate();
        }
    }

    void OnActivate(){ //calls when we activate. Chooses what activation event we want to happen. Right now just calls default
        DefaultDestroy();
    }
 
    void DefaultDestroy(){ //default destroy action. will only work if there are actors in the killzone
        if(kZone.toDestroy.Count != 0){
            kZone.DestroyActors();
            numRounds++;
        }

       
        
    }

    public void ReturnAll(){ //returns all saved actors to original position
         for(int i = 0; i < toSave.Count; i++){
            Debug.Log("got here " + toSave.Count );
            toSave[i].GetComponent<ActorBehavior>().ReturnToOriginalPosition();
        }
    }
}
