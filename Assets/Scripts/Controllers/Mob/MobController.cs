using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MobController : MonoBehaviour
{

    [Header("Stats")]
	[Space]
    public MobScriptable mobScriptable;                                     // Mob stats
	public int hitPoints = 3;					                            // Current hit points
	public float mobCurrentSpeed = 3f;					                    // Current Mob speed
	public float mobCurrentCleanSpeed = 3f;					                // Current Mob speed clean speed (>= 0)


    [Header("Params")]
	[Space]
    public GameObject self;							                        // Self
    [SerializeField] private GameObject view;							    // View
    [SerializeField] private Animator animator;							    // Animator
    public MobStatusController mobStatus;							    // Animator
    public GameObject detectionCollider;					                // Mob detection collider
    [SerializeField] private bool isFlippingY;							    // Does its view needs to be flipped
    [SerializeField] private bool isLookingAtTarget;					    // Is it looking at the player
    public bool isSelectableByPlayer = false;					            // Can the mob be selected by player
    public bool isInAttackCooldown;					                        // Is Mob in attack cooldown
    public GameObject mobCurrentTarget;					                    // Target gameobject
    public Vector3 mobCurrentTargetPosition;					            // Target position

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
    private bool isSelectedByPlayer;                                        // Is mob moving currenrtly selected by player
    public bool isFollowingPlayerGoTo;                                      // Is mob currently moving towards player order
    public bool isWaitingforTarget;                                         // Is mob waiting with no target


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hitPoints = mobScriptable.baseHitPoints;
        playerGameObject = GameObject.FindWithTag("Player");
        mobCurrentSpeed = mobScriptable.speed;

        // Setting up player attackables and target
        if(LayerMask.LayerToName(gameObject.layer) == "Enemy")
        {
            mobCurrentTarget = playerGameObject;
        }

        // Setting up status management
        mobStatus = gameObject.GetComponent<MobStatusController>();

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
        // Check if Mob has target and sending him towards
        if((mobCurrentTarget != null || mobCurrentTargetPosition != null) && mobScriptable.canMoveTowardsTarget)
        {
            GoToCurrentTarget();
        }
        // Reseting player as target if no target for enemy Mobs
        if(LayerMask.LayerToName(gameObject.layer) == "Enemy")
        {
            if(mobCurrentTarget == null)
            {
                mobCurrentTarget = playerGameObject;
            }
        }
        // Setting waiting if player mob has no target
        if(LayerMask.LayerToName(gameObject.layer) == "PlayerMob")
        {
            isWaitingforTarget = (mobCurrentTarget == null);
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
        // Set Mob speed at 0 if its inferior to 1
        mobCurrentCleanSpeed = mobCurrentSpeed < 0 ? 0 : mobCurrentSpeed;

        if(mobCurrentTarget != null && !isFollowingPlayerGoTo)      // Move towards given GameObject
        {
            var cleanMobCurrentTargetPosition = new Vector3(mobCurrentTarget.transform.position.x,mobCurrentTarget.transform.position.y,0);
            self.transform.position = Vector3.MoveTowards(self.transform.position, cleanMobCurrentTargetPosition, mobCurrentCleanSpeed * Time.deltaTime);
            animator.SetTrigger("Moving");
        }
        else if(isFollowingPlayerGoTo)                              //Move towards given positon
        {
            var cleanMobCurrentTargetPosition = new Vector3(mobCurrentTargetPosition.x,mobCurrentTargetPosition.y,0);
            self.transform.position = Vector3.MoveTowards(self.transform.position, cleanMobCurrentTargetPosition, mobCurrentCleanSpeed * Time.deltaTime);
            animator.SetTrigger("Moving");
        }
    }

    public void SetNewCurrentTarget(GameObject newCurrentTarget)
    {
        mobCurrentTarget = newCurrentTarget;
        isFollowingPlayerGoTo = false;
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
    public void ActivateSelectedByPlayer(bool selected)
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

    public void SetNewCurrentTargetTransform(Vector3 targetPosition)
    {
        isFollowingPlayerGoTo = true;
        mobCurrentTargetPosition = targetPosition;
    }
    #endregion

    #region Manage mob attack
    // Mob attack trigger
    private IEnumerator OnTriggerStay2D(Collider2D collider)
    {
        var factionList = mobStatus.isCharmed ? mobStatus.charmedAttackableFactionsList : mobScriptable.attackableFactionsList;
        foreach(string faction in factionList)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer(faction) && !isInAttackCooldown)
            {
                collider.GetComponent<MobController>().TakeHit(mobScriptable.damage);
                isInAttackCooldown = true;
                yield return new WaitForSeconds(mobScriptable.meleeAttackCooldown);
                ResetMobAttackCooldown();
            }
        }
    }	

    private void ResetMobAttackCooldown()
    {
        isInAttackCooldown = false;
    }
    #endregion

    #region Managing VFX
    public void UI_DisplayDamage(int damage)
    {
        var damageGameobject = Instantiate(uiDamageVFX,new Vector2(uiTextDamageLocation.position.x,uiTextDamageLocation.position.y),Quaternion.identity);
        damageGameobject.transform.SetParent(ui_spellDamageCanvasTransform.transform);
        damageGameobject.GetComponent<UITextDamageController>().SetTextDamage(damage);
    }
    #endregion
}
