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
	public float mobCurrentCleanSpeed = 3f;					                    // Current Mob speed


    [Header("Params")]
	[Space]
    [SerializeField] private GameObject self;							    // Self
    [SerializeField] private GameObject view;							    // View
    [SerializeField] private Animator animator;							    // Animator
    [SerializeField] private bool isFlippingY;							    // Does its view needs to be flipped
    [SerializeField] private bool isLookingAtTarget;					    // Is it looking at the player
    [SerializeField] private Collider2D detectionCollider;					// Mob detection collider
    public GameObject mobCurrentTarget;					                    // Target position
    public Vector3 mobCurrentTargetPosition;					            // Target position

    [Header("Status")]
	[Space]
    public bool isChilled;                                                  // Is Mob chilled
    public bool isIced;                                                  // Is Mob iced

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
        // Check if Mob has target and sending him towards
        if((mobCurrentTarget != null || mobCurrentTargetPosition != null) && mobScriptable.canMoveTowardsTarget)
        {
            GoToCurrentTarget();
        }
        // Reseting player as target if no target for enemy Mobs
        if(mobScriptable.faction == "Enemy")
        {
            if(mobCurrentTarget == null)
            {
                mobCurrentTarget = playerGameObject;
            }
        }
        // Setting waiting if player mob has no target
        if(mobScriptable.faction == "PlayerMob")
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
        //Debug.Log(mobCurrentSpeed);

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

    #region Mob status management
    public bool GetMobStatusImmunity(string status)
    {
        var isImmune = false;
        foreach(string statusImmunity in mobScriptable.statusImmunityList)
        {
            if(statusImmunity == status)
            {
                isImmune =  true;
            } 
        }
        return isImmune;
    }

    public void ApplyStatusList(List<string> statusList, bool isApplied = true)
    {
        foreach(string status in statusList)
        {
            ApplyStatus(status, isApplied);
        }
    }

    public void ApplyStatus(string status, bool isApplied)
    {
        switch(status)
        {
            case "Chill":
                StartCoroutine(ApplyStatusChill(isApplied));
                break;
            case "Ice":
                StartCoroutine(ApplyStatusIce(isApplied));
                break;
            default:
                break;
        }
    }

    public IEnumerator ApplyStatusChill(bool isApplied)
    {
        // Check if Mob is immune
        if(!GetMobStatusImmunity("Chill")) 
        {
            Debug.Log("---- isApplied : " + isApplied + " - isChilled : " + isChilled + " - isIced : " + isIced);
            if(isApplied)
            {
                if(!isChilled && !isIced)
                {
                    mobCurrentSpeed -= mobScriptable.chillSpeedMalus; 
                    isChilled = isApplied;
                    yield return new WaitForSeconds(mobScriptable.chillMalusTime);
                    StartCoroutine(ApplyStatusChill(false));
                }
                else if(!isIced)
                {
                    StartCoroutine(ApplyStatusIce(true));
                    StartCoroutine(ApplyStatusChill(false));
                }
            }
            else if(!isApplied && isChilled)
            {
                mobCurrentSpeed += mobScriptable.chillSpeedMalus;
                isChilled = isApplied;
            }
        }
    }

    public IEnumerator ApplyStatusIce(bool isApplied)
    {
        // Check if Mob is immune
        if(!GetMobStatusImmunity("Ice"))
        {
            if(isApplied)
            {
                Debug.Log("----ApplyStatusIce : " + isApplied);
                if(!isIced)
                {
                    mobCurrentSpeed -= 999; 
                    isIced = isApplied;
                    yield return new WaitForSeconds(mobScriptable.iceMalusTime);
                    StartCoroutine(ApplyStatusIce(false));
                }
            }
            else if(!isApplied && isIced)
            {
                mobCurrentSpeed += 999;
                isIced = isApplied;
            }
        }
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
