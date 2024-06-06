using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStrikerController : MonoBehaviour
{
    Rigidbody2D rb;
    bool isMoving;

    private void Start()
    {
        isMoving = false;
        rb = GetComponent<Rigidbody2D>();
    }
    //private void OnEnable()
    //{
    //    isMoving = false;
    //}
        private void Update()
    {
        //Debug.Log("is mocvinfg"+isMoving);
        // Check if the enemy striker has come to a near stop and is not moving
        if (rb.velocity.magnitude < 0.1f && !isMoving)
        {
            Debug.Log("here in magnitude less than 0.1f");
            isMoving = true;
            StartCoroutine(EnemyTurn());
        }
    }

    private void OnEnable()
    {
        isMoving = false;
        Debug.Log("here in on enable striker controller");
        // Reset the initial state of the enemy striker
        CollisionSoundManager.shouldBeStatic = true;
        GetComponent<SpriteRenderer>().enabled = false;
        transform.position = new Vector3(0, 3.45f, 0f);
    }

    IEnumerator EnemyTurn()
    {
        // Determine the target coin based on game logic
        // Find the closest coin to a pocket
        const int maxAttempts = 10;
        int attempts = 0;
        bool isObstructed;
       
        do
        {
            isObstructed = false;

            // Generate a random position within the board bounds
            float x = Random.Range(-3.24f, 3.24f);
            transform.position = new Vector3(x, 3.45f, 0f);

            // Check if the generated position is obstructed by other coins or the striker
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);

            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Black") || collider.CompareTag("White") || collider.CompareTag("Striker"))
                {
                    isObstructed = true;
                    break;
                }
            }
            Debug.Log("here");
            attempts++;
        }
        while (isObstructed && attempts < maxAttempts);

        if (isObstructed)
        {
            Debug.Log("Failed to find a valid position for the enemy striker.");
            transform.position = new Vector3(0f, 3.45f, 0f);
            isObstructed = false;
        }

        yield return new WaitWhile(() => isObstructed);
        Debug.Log("sprite renderer ");
        GetComponent<SpriteRenderer>().enabled = true;

        yield return new WaitForSeconds(2f);
        CollisionSoundManager.shouldBeStatic = false;
        yield return new WaitForSeconds(0.1f);

        // Find all the available black coins
        List<GameObject> coins = new List<GameObject>(GameObject.FindGameObjectsWithTag("Black"));
        GameObject queenCoin = GameObject.FindGameObjectWithTag("Queen");

        if (queenCoin != null)
        {
            coins.Add(queenCoin);
        }
        GameObject closestCoin = null;
        float closestDistance = Mathf.Infinity;

        if (coins.Count == 0)
        {
            Debug.Log("No coins left");
            yield break;
        }

        // Find the closest coin to a pocket
        foreach (GameObject coin in coins)
        {
            float distance = Vector3.Distance(coin.transform.position, GetClosestPocket(coin.transform.position));
            if (distance < closestDistance)
            {
                closestCoin = coin;
                closestDistance = distance;
            }
        }

        // Calculate the direction and speed of the striker based on the position of the target coin and the enemy's striker.
        Vector3 targetDirection = closestCoin.transform.position - transform.position;
        targetDirection.z = 0f;
        float targetSpeed = CalculateStrikerSpeed(targetDirection.magnitude);

        // Apply the calculated force to the striker and end the enemy's turn.
        rb.AddForce(targetDirection.normalized * targetSpeed, ForceMode2D.Impulse);
        GameManager.Instance.orgplayerturn = GameManager.Instance.playerTurn;
        GameManager.Instance.playerTurn = true;
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => rb.velocity.magnitude < 0.1f);
    
        yield return StartCoroutine(BoardManager.Instance.CheckQueenPocket());
        //gameObject.SetActive(false);
        Debug.Log("before end turn");
        //here need to add
        gameObject.SetActive(false);
        if (BoardManager.Instance.lastPocketedObject == "Striker")
        {
            GameManager.Instance.playerTurn = !GameManager.Instance.orgplayerturn;
            //here we can change
        }
        GameManager.Instance.EndTurn();
    }

    Vector3 GetClosestPocket(Vector3 position)
    {
        // Find the closest pocket to a given position
        Vector3 closestPocket = Vector3.zero;
        float closestDistance = Mathf.Infinity;
        GameObject[] pockets = GameObject.FindGameObjectsWithTag("Pocket");

        foreach (GameObject pocket in pockets)
        {
            float distance = Vector3.Distance(position, pocket.transform.position);
            if (distance < closestDistance)
            {
                closestPocket = pocket.transform.position;
                closestDistance = distance;
            }
        }

        return closestPocket;
    }

    float CalculateStrikerSpeed(float distance)
    {
        // Calculate the speed of the striker based on the distance to the target coin
        float maxDistance = 4.0f; // Maximum distance the striker can travel
        float minSpeed = 10f; // Minimum striker speed
        float maxSpeed = 30f; // Maximum striker speed

        float speed = Mathf.Lerp(minSpeed, maxSpeed, distance / maxDistance);
        return speed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Play the collision sound if the enemy striker collides with the board
        if (other.gameObject.CompareTag("Board"))
        {
            GetComponent<AudioSource>().Play();
        }
    }
}
