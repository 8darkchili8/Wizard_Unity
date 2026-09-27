using UnityEngine;
using System.Collections.Generic;

public class MobController : MonoBehaviour
{

    [Header("Stats")]
	[Space]
	[SerializeField] private int baseHitPoints = 3;							// Base hit points
	[SerializeField] private int hitPoints = 3;					            // Current hit points
	[SerializeField] private int enemyDamage = 5;							// Damage delt by the enemy
	[SerializeField] private float speed = 3f;								// Move speed
	[SerializeField] private float detectionRange = 3f;						// Detection range


    [Header("Params")]
	[Space]
    [SerializeField] private GameObject self;							    // Self
    [SerializeField] private GameObject view;							    // View
    [SerializeField] private Animator animator;							    // Animator
    [SerializeField] private bool isMovingTowardsTarget;				    // Is it moving towards the player
    [SerializeField] private bool isFlippingY;							    // Does its view needs to be flipped
    [SerializeField] private bool isLookingAtTarget;					    // Is it looking at the player
    [SerializeField] private Collider2D detectionCollider;					    // Mob detection collider

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
    private bool isMovingToMainTarget;                                           // Is mob moving towards its main target
    private bool isMovingToSecondary;                                      // Is mob moving towards its secondary target


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hitPoints = baseHitPoints;
        playerGameObject = GameObject.FindWithTag("Player");

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
        if(self.transform.position != playerGameObject.transform.position && isMovingTowardsTarget)
        {
            GoToTarget();
        }

        if(isLookingAtTarget)
        {
            // [TO UPDATE] only aims at player
            AimAtTarget(playerGameObject.transform.position);
        }
    }

    #region Managing enemy movement
    private void GoToTarget()
    {   
        // [TO UPDATE] only goes to player
        self.transform.position = Vector3.MoveTowards(self.transform.position, playerGameObject.transform.position, speed * Time.deltaTime);
        animator.SetTrigger("Moving");
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

    #region Manaing enemy life
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
    #endregion

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == playerGameObject.layer)
        {
            playerController.TakeHit(enemyDamage);
            DestroySelf();
        }
    }

    public void DestroySelf()
    {
        animator.SetTrigger("Dying");
        Destroy(self);
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
