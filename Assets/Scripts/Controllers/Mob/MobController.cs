using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobController : MonoBehaviour
{

    [Header("Stats")]
	[Space]
    public MobScriptable mobScriptable;                     // Mob stats
	public int hitPoints = 3;					            // Current hit points


    [Header("Params")]
	[Space]
    [SerializeField] private GameObject self;							    // Self
    [SerializeField] private GameObject view;							    // View
    [SerializeField] private Animator animator;							    // Animator
    [SerializeField] private bool isFlippingY;							    // Does its view needs to be flipped
    [SerializeField] private bool isLookingAtTarget;					    // Is it looking at the player
    [SerializeField] private Collider2D detectionCollider;					// Mob detection collider
    public GameObject mobCurrentTarget;					                    // Target position

    [Header("UI")]
	[Space]
    [SerializeField] private Transform uiTextDamageLocation;			    // Where to spawn the damage VFX
    [SerializeField] private GameObject uiDamageVFX;			            // Damage VFX to spawn



    // Player info
    private GameObject playerGameObject;                                    // Player GameObject
    private PlayerController playerController;                              // Player Controller
    // Spell info
    private GameObject spellManager;                                        // Spell manager GameObject
    private Transform ui_spellDamageCanvasTransform;                        // Spell damage canvas transform
    // State
    private bool isMovingToMainTarget;                                      // Is mob moving towards its main target
    private bool isMovingToSecondary;                                       // Is mob moving towards its secondary target
    private bool isSelectedByPlayer;                                        // Is mob moving towards its secondary target


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hitPoints = mobScriptable.baseHitPoints;
        playerGameObject = GameObject.FindWithTag("Player");

        // Set starting target as player for enemy Mobs
        if(mobScriptable.faction == "Enemy")
        {
            mobCurrentTarget = playerGameObject;
        }

        // Setting up damage display
        spellManager = GameObject.FindWithTag("SpellManager");
        ui_spellDamageCanvasTransform = spellManager.transform.Find("UI_Damage").GetChild(0);
        playerController = playerGameObject.GetComponentInChildren<PlayerController>();
        
        if(self.transform.position.x > playerGameObject.transform.position.x && isFlippingY)
        {
            transform.localScale = new Vector3(-1,1,1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(self.transform.position != playerGameObject.transform.position && mobScriptable.canMoveTowardsTarget)
        {
            GoToCurrentTarget();
        }
        // Reseting player as taget if no target fo enemy Mobs
        if(mobScriptable.faction == "Enemy")
        {
            if(mobCurrentTarget == null)
            {
                mobCurrentTarget = playerGameObject;
            }
        }

        if(isLookingAtTarget)
        {
            // [TO UPDATE] only aims at player
            AimAtTarget(playerGameObject.transform.position);
        }
    }

    #region Managing mob movement
    private void GoToCurrentTarget()
    {   
        // [TO UPDATE] only goes to player (causing error if no player)
        if(mobCurrentTarget != null)
        {
            self.transform.position = Vector3.MoveTowards(self.transform.position, mobCurrentTarget.transform.position, mobScriptable.speed * Time.deltaTime);
            animator.SetTrigger("Moving");
        }
    }

    public void SetNewCurrentTarget(GameObject newCurrentTarget)
    {
        mobCurrentTarget = newCurrentTarget;
    }

    private void AimAtTarget(Vector3 targetPosition)
    {
        targetPosition.z = 0f;

        targetPosition.x = targetPosition.x - self.transform.position.x;
        targetPosition.y = targetPosition.y - self.transform.position.y;
        float angle = Mathf.Atan2(targetPosition.y, targetPosition.x) * Mathf.Rad2Deg;
        view.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    #endregion

    #region Managing mob life
    public int TakeHit(int damage)
    {
        hitPoints = hitPoints - damage;
        UI_DisplayDamage(damage);
        IsEnemyDead();

        return hitPoints;
    }

    private void IsEnemyDead()
    {
        if(hitPoints <= 0)
        {
            DestroySelf();
        }
    }

    public void DestroySelf()
    {
        animator.SetTrigger("Dying");
        Destroy(self);
    }
    #endregion

    #region Managing mob selection
    public void IsSelectedByPlayer(bool selected)
    {
        isSelectedByPlayer = selected;

        if(isSelectedByPlayer)
        {
            // [TO DO] Add visuals when mob is selected
        }
        else
        {
            // [TO DO] Remove visuals when mob is unselected
        }
    }

    public void OrderTargetChange(Transform newCurrentTarget)
    {
        //SetNewCurrentTarget();
    }
    #endregion

    // Mob attack trigger
    private void OnTriggerEnter2D(Collider2D collider)
    {
        foreach(string layerName in mobScriptable.attackableFactionsList)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer(layerName))
            {
                collider.GetComponent<MobController>().TakeHit(mobScriptable.damage);
            }
        }
    }

    #region Managing VFX
    public void UI_DisplayDamage(int damage)
    {
        var damageGameobject = Instantiate(uiDamageVFX,new Vector2(uiTextDamageLocation.position.x,uiTextDamageLocation.position.y),Quaternion.identity);
        damageGameobject.transform.SetParent(ui_spellDamageCanvasTransform.transform);
        damageGameobject.GetComponent<UITextDamageController>().SetTextDamage(damage);
    }
    #endregion
}
