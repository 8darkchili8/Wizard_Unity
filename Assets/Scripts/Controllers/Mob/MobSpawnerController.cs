using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobSpawnerController : MonoBehaviour
{
    [Header("Stats")]
	[Space]
    [SerializeField] private int spawnrateMin = 5;						        // Enemy spawn rate min
    [SerializeField] private int spawnrateMax = 5;						        // Enemy spawn rate max

    [Header("Params")]
	[Space]
    [SerializeField] private GameObject self;							    // Self
    [SerializeField] private GameObject view;							    // View
    [SerializeField] private GameObject enemy;							    // Enemy
    [SerializeField] private bool isInCoolDown;							    // Is spawner cooling down

    private GameObject playerGameObject;                                    //Player GameObject

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(!isInCoolDown)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        Instantiate(enemy,self.transform.position,Quaternion.identity);
        isInCoolDown = true;
        StartCoroutine(CollingDown());
    }

    private IEnumerator CollingDown()
	{
        int spawnrate = Mathf.RoundToInt(Random.Range(spawnrateMin,spawnrateMax));
        yield return new WaitForSeconds(spawnrate);
        isInCoolDown = false;
    }
}
