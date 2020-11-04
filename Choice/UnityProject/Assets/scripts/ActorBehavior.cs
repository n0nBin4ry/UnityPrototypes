using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//behavior for each of the little square "actors"

public class ActorBehavior : MonoBehaviour
{

    bool inZone = false;

    Vector3  originalPosition; 
    Vector3 originalScale;

    private bool isDragging; //is it being dragged by the mouse
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = this.transform.position;   
        originalScale = this.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDragging == true){ //check if mouse button is down and if we should be dragging this actor around
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.Translate(mousePosition);
        }
    }

    //detects when we leave/enter a zone
    private void OnTriggerEnter2D(Collider2D other) { 
        if(other.tag == "zone"){
            inZone = true;
        }
    }

   private void OnTriggerExit2D(Collider2D other) { 
        if(other.tag == "zone"){
            inZone = false;
        }
    }
    

    public void ReturnToOriginalPosition(){ //lerps actor back to starting position
        StartCoroutine("Return");
        
    }

    IEnumerator Return(){
        while(this.transform.position != originalPosition){
           
            this.transform.position = Vector3.Lerp(this.transform.position, originalPosition, .2f);
            yield return null;
           
        } 
           yield break;
    }

     public  void OnMouseDown() {
        isDragging = true;
    }

    public void OnMouseUp() {
        isDragging = false;
        if(!inZone){
            Return(); //go back to starting position unless we're in a zone 
        }
        this.GetComponent<AudioSource>().Play(); //plays audio clip on set down
    }
}
