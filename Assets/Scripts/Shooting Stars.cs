using UnityEngine;

public class ShootingStars : MonoBehaviour
{
    public GameObject star;
    float nextStar;
    float starTimer;

    void Start()
    {
        nextStar = Random.Range(2.0f, 6.0f);
        starTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (starTimer > nextStar) 
        { 
            CreateStar(); 
            nextStar = Random.Range(2.0f, 6.0f);
            starTimer = 0.0f;
        }
    }

    void CreateStar()
    {
        float starY = Random.Range(-35, 100);
        Debug.Log("Star Spawned");
        Instantiate(star, new Vector3(-166, starY, -35), this.transform.rotation);
    }
}
