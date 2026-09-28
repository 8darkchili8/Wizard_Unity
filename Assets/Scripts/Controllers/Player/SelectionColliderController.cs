using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SelectionColliderController : MonoBehaviour
{

    public List<GameObject> selectedMobs = new List<GameObject>();             // Selected mob list

    private GameObject playerGameObject;                                        // Player GameObject
    private PlayerController playerController;                                  // Player Controller

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerGameObject = GameObject.FindWithTag("Player");
        playerController = playerGameObject.GetComponentInChildren<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<GameObject> ReturnSelectedMobs()
    {

        var returnSelectedMobs = selectedMobs;
        selectedMobs = new List<GameObject>();   
        return returnSelectedMobs;
    }

    #region Setting selected GameObject
    // Adds units entering the collider in the list
    public void OnTriggerEnter2D(Collider2D collider)
    {
        // Mob selection
        if (!selectedMobs.Contains(collider.gameObject) && (collider.gameObject.layer == LayerMask.NameToLayer("Enemy") || collider.gameObject.layer == LayerMask.NameToLayer("PlayerMob")))
        {   
            if(collider.gameObject.GetComponent<MobController>().mobScriptable.isSelectable)
            {
                selectedMobs.Add(collider.gameObject);
            }
        }
    }

    // Removes units exiting the collider grom the list
    public void OnTriggerExit2D(Collider2D collider)
    {
        // Mob deselection
        if(selectedMobs.Contains(collider.gameObject) && playerController.isSelecting)
        {
            selectedMobs.Remove(collider.gameObject);
        }
    }
    #endregion
}
