using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpellController : MonoBehaviour
{
    [Header("Stats")]
	[Space]
    [SerializeField] private SpellScriptable spellScriptable;					// Spell data

    [Header("Params")]
	[Space]
    [SerializeField] private GameObject self;							        // Self
    [SerializeField] private GameObject view;							        // View
    [SerializeField] private CircleCollider2D spellCollider;				    // Collider

    private List<GameObject> targetList = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Reduce spell size
        //view.transform.localScale = new Vector3(0,0,0);
        StartCoroutine(LoadSpell());
    }

    // Update is called once per frame
    void Update()
    {
        RotateSpellView();   
    }

    private void RotateSpellView()
    {
        //Spell view rotation
        view.transform.Rotate(0,0,((10-spellScriptable.castingSpeed)*10)*Time.deltaTime);
        
        //Spell view size
    }

    private IEnumerator LoadSpell()
	{
		yield return new WaitForSeconds(spellScriptable.castingSpeed);
        // Damage
        if(spellScriptable.spellDamage > 0)
        {
            ApplySpellDamage();
            ApplySpellStatus();
        }
        // Summon
        if(spellScriptable.isSpellSummoning)
        {
            Summon();
        }

        yield return new WaitForSeconds(0.25f);
        DestroySelf();
	}

    private void ApplySpellDamage()
    {
        foreach(GameObject target in targetList.ToList())
        {
            MobController mobController = target.GetComponent<MobController>();
            mobController.TakeHit(spellScriptable.spellDamage);
        }
    }

    private void ApplySpellStatus()
    {
        foreach(GameObject target in targetList.ToList())
        {
            MobController mobController = target.GetComponent<MobController>();
            mobController.ApplyStatusList(spellScriptable.statusList);
        }
    }

    private void Summon()
    {
        if(spellScriptable.spellSummonNumber == 1)
        {
            Instantiate(spellScriptable.spellSummon,gameObject.transform.position,Quaternion.identity);
        }
        else
        {
            for(int summonCount = 1;summonCount<=spellScriptable.spellSummonNumber;summonCount++)
            {
                // Give random spawn position based on spell radius
                var spawnPosition = (
                    new Vector3(Mathf.RoundToInt(Random.Range(0,spellCollider.radius)),Mathf.RoundToInt(Random.Range(0,spellCollider.radius)),0)
                    + gameObject.transform.position
                );
                Debug.Log(spawnPosition);
                Instantiate(spellScriptable.spellSummon,spawnPosition,Quaternion.identity);
            } 
        }
    }

    #region Setting spell targets
    // Adds units entering the collider in the list
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(!targetList.Contains(collider.gameObject))
        {
            foreach(string layerName in spellScriptable.attackableFactionsList)
            {
                if(collider.gameObject.layer == LayerMask.NameToLayer(layerName))
                {
                    targetList.Add(collider.gameObject);
                }
            }
        }
    }

    // Removes units exiting the collider from the list
    public void OnTriggerExit2D(Collider2D collider)
    {
        if(targetList.Contains(collider.gameObject) && spellScriptable.isTargetingOnlyInside)
        {
            targetList.Remove(collider.gameObject);
        }
    }
    #endregion

    public void DestroySelf()
    {
        Destroy(self);
    }
}
