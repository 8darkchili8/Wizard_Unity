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

    private void Summon()
    {
        Instantiate(spellScriptable.spellSummon,gameObject.transform.position,Quaternion.identity);
    }

    #region Setting spell targets
    // Adds units entering the collider in the list
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (!targetList.Contains(collider.gameObject) && collider.gameObject.layer == 7)
        {
            targetList.Add(collider.gameObject);
        }
    }

    // Removes units exiting the collider grom the list
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
