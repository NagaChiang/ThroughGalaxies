using UnityEngine;
using System.Collections;

[System.Serializable]
public struct RandomSpawnSet
{
    public int number;
    public float interval;
    public Vector3 point;
    public float width;
    public GameObject[] objects;
}

public class RandomSpawnWave : Wave
{
    [Header("RandomSpawnWave")]
    public RandomSpawnSet[] randomSpawnSets;

    public override void spawn(float difficulty)
    {
        // spawn each set
        foreach(RandomSpawnSet set in randomSpawnSets)
        {
            StartCoroutine(spawnSet(set, difficulty));
        }
    }

    private IEnumerator spawnSet(RandomSpawnSet set, float difficulty)
    {
        // Set Difficulty
        if (!isBoss && set.objects[0].GetComponent<Asteroid>() == null)
        {
            set.number = (int)(set.number * difficulty);
            set.interval = set.interval / difficulty;
        }
        
        // Spawn
        GameObject enemy = null;
        for (int i = 0; i < set.number; i++)
        {
            // x range
            float xMin = set.point.x - set.width;
            float xMax = set.point.x + set.width;

            // random position
            Vector3 posSpawn = new Vector3(Random.Range(xMin, xMax), set.point.y, set.point.z);

            // random object
            GameObject obj = set.objects[Random.Range(0, set.objects.Length)];

            // instantiate
            enemy = (GameObject)Instantiate(obj, posSpawn, obj.transform.rotation);
            enemy.GetComponent<Damageable>().SetDifficulty(difficulty);

            // interval
            yield return new WaitForSeconds(set.interval);
        }

        // Handle boss wave
        if(isBoss)
        {
            // Wait until the boss dies
            while (enemy)
                yield return null;
        }

        // Destroy self
        Destroy(gameObject);
    }
}
