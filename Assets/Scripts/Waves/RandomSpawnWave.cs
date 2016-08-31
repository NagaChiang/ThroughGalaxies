using UnityEngine;
using System.Collections;

public class RandomSpawnWave : Wave
{
    public int number;
    public float spawnInterval;
    public Vector3 spawnPoint;
    public float spawnWidth;
    public GameObject[] objects;

    public override IEnumerator spawn(float difficulty) // TODO difficulty
    {
        for (int i = 0; i < number; i++)
        {
            float xMin = spawnPoint.x - spawnWidth;
            float xMax = spawnPoint.x + spawnWidth;
            Vector3 posSpawn = new Vector3(Random.Range(xMin, xMax), spawnPoint.y, spawnPoint.z);
            GameObject obj = objects[Random.Range(0, objects.Length)];
            Instantiate(obj, posSpawn, obj.transform.rotation);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
