using UnityEngine;
using UnityEngine.UIElements;

public class ShootingStars : MonoBehaviour
{
    public StarShoot star;
    float nextStar;
    float starTimer;

    public float minSpeed;
    public float maxSpeed;

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
            Debug.Log("Star Timer Complete");
            CreateStar();
            nextStar = Random.Range(2.0f, 6.0f);
            starTimer = 0.0f;
        }
        else { starTimer += Time.deltaTime; }
    }

    void CreateStar()
    {
        float starY = Random.Range(-35, 100);
        Debug.Log("Star Spawned");
        StarShoot newStar = Instantiate(star, new Vector3(-166, starY, -35), this.transform.rotation);
        newStar.speed = Random.Range(minSpeed, maxSpeed);
    }
}