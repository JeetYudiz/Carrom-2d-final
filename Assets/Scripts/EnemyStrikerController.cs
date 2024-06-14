using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        GetComponent<SpriteRenderer>().enabled = true;
        transform.position = new Vector3(0, 3.45f, 0f);
    }


    /// <summary>
    //IEnumerator EnemyTurn()
    //{
    //    // Find all the available black coins
    //    List<GameObject> coins = new List<GameObject>(FindGameObjectsWithTagType(Tags.Black));

    //    GameObject queenCoin = FindGameObjectWithTagType(Tags.Queen);
    //    if (queenCoin != null)
    //    {
    //        coins.Add(queenCoin);
    //    }

    //    if (coins.Count == 0)
    //    {
    //        Debug.Log("No coins left");
    //        yield break;
    //    }

    //    // Find the closest coins
    //    List<GameObject> closestCoins = new List<GameObject>();
    //    float minDistance = Mathf.Infinity;

    //    foreach (GameObject coin in coins)
    //    {
    //        float distance = Vector3.Distance(transform.position, coin.transform.position);
    //        if (distance < minDistance)
    //        {
    //            closestCoins.Clear();
    //            closestCoins.Add(coin);
    //            minDistance = distance;
    //        }
    //        else if (distance == minDistance)
    //        {
    //            closestCoins.Add(coin);
    //        }
    //    }

    //    // Among the closest coins, find the coin with the minimum angle difference to the pockets
    //    GameObject bestCoin = null;
    //    float minAngleDifference = Mathf.Infinity;

    //    foreach (GameObject coin in closestCoins)
    //    {
    //        Vector3 coinPosition = coin.transform.position;
    //        GameObject[] pockets = FindGameObjectsWithTagType(Tags.Pocket);

    //        foreach (GameObject pocket in pockets)
    //        {
    //            Vector3 pocketPosition = pocket.transform.position;

    //            // Calculate the angle between the striker and the coin
    //            Vector3 strikerToCoinDirection = (coinPosition - transform.position).normalized;
    //            float strikerToCoinAngle = Mathf.Atan2(strikerToCoinDirection.y, strikerToCoinDirection.x) * Mathf.Rad2Deg;

    //            // Calculate the angle between the coin and the pocket
    //            Vector3 coinToPocketDirection = (pocketPosition - coinPosition).normalized;
    //            float coinToPocketAngle = Mathf.Atan2(coinToPocketDirection.y, coinToPocketDirection.x) * Mathf.Rad2Deg;

    //            // Calculate the absolute difference between the angles
    //            float angleDifference = Mathf.Abs(strikerToCoinAngle - coinToPocketAngle);

    //            // Check if the path is not blocked by white coins
    //            if (!IsPathObstructedByWhiteCoin(transform.position, coinPosition) &&
    //                !IsPathObstructedByWhiteCoin(coinPosition, pocketPosition))
    //            {
    //                if (angleDifference < minAngleDifference)
    //                {
    //                    minAngleDifference = angleDifference;
    //                    bestCoin = coin;
    //                }
    //            }
    //        }
    //    }

    //    if (bestCoin != null)
    //    {
    //        // Calculate the direction and speed of the striker based on the position of the best coin
    //        Vector3 targetDirection = bestCoin.transform.position - transform.position;
    //        targetDirection.z = 0f;
    //        float targetSpeed = CalculateStrikerSpeed(targetDirection.magnitude);

    //        // Apply the calculated force to the striker and end the enemy's turn
    //        rb.AddForce(targetDirection.normalized * targetSpeed, ForceMode2D.Impulse);
    //    }
    //    else
    //    {
    //        // No valid coin found, take a defensive shot or skip turn
    //        // Implement your defensive shot logic here
    //        Debug.Log("No valid coin found. Taking a defensive shot or skipping turn.");
    //    }

    //    GameManager.Instance.orgplayerturn = GameManager.Instance.playerTurn;
    //    GameManager.Instance.playerTurn = true;
    //    yield return new WaitForSeconds(0.1f);
    //    yield return new WaitUntil(() => rb.velocity.magnitude < 0.1f);

    //    yield return StartCoroutine(BoardManager.Instance.CheckQueenPocket());
    //    gameObject.SetActive(false);
    //    if (BoardManager.Instance.lastPocketedObject == "Striker")
    //    {
    //        GameManager.Instance.playerTurn = !GameManager.Instance.orgplayerturn;
    //    }
    //    GameManager.Instance.EndTurn();
    //}

    /// <returns></returns>
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
                if (IsColliderObstructing(collider))
                {
                    isObstructed = true;
                    break;
                }
            }

            Debug.Log("here");
            attempts++;
        } while (isObstructed && attempts < maxAttempts);

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

        // Find all available black coins and the queen coin
        List<GameObject> coins = new List<GameObject>(FindGameObjectsWithTagType(Tags.Black));
        GameObject queenCoin = FindGameObjectWithTagType(Tags.Queen);

        if (queenCoin != null)
        {
            coins.Add(queenCoin);
        }

        Debug.Log("the length " + coins.Count);

        if (coins.Count == 0)
        {
            Debug.Log("No coins left");
            yield break;
        }

        // Find the closest two coins to the pockets
        List<GameObject> closestToPockets = FindClosestCoinsToPockets(coins, 2);

        // Find the closest two coins to the striker
        List<GameObject> closestToStriker = FindClosestCoinsToStriker(coins, 2);

        // Determine the coin with the minimum angle difference to the pockets
        GameObject bestCoin = FindCoinWithMinAngleDifference(closestToPockets, closestToStriker);

        //Debug.Log("the best cp");
        if (bestCoin != null)
        {
            Vector3 targetDirection = bestCoin.transform.position - transform.position;
            targetDirection.z = 0f;

            // Check if it's a backshot
            bool isBackshot = Vector3.Dot(targetDirection.normalized, transform.up) < 0f;

            float targetSpeed = CalculateStrikerSpeed(targetDirection.magnitude, isBackshot);
            Debug.Log("target speed "+targetSpeed);
            rb.AddForce(targetDirection.normalized * targetSpeed, ForceMode2D.Impulse);
        }

        else
        {
            // No valid coin found, take a defensive shot or skip turn
            // Implement your defensive shot logic here
            Debug.Log("No valid coin found. Taking a defensive shot or skipping turn.");
        }

        GameManager.Instance.orgplayerturn = GameManager.Instance.playerTurn;
        GameManager.Instance.playerTurn = true;
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => rb.velocity.magnitude < 0.1f);

        yield return StartCoroutine(BoardManager.Instance.CheckQueenPocket());
        gameObject.SetActive(false);

        if (BoardManager.Instance.lastPocketedObject == "Striker")
        {
            GameManager.Instance.playerTurn = !GameManager.Instance.orgplayerturn;
        }
        GameManager.Instance.EndTurn();
    }

    private List<GameObject> FindClosestCoinsToPockets(List<GameObject> coins, int count)
    {
        List<GameObject> closestCoins = new List<GameObject>();
        Dictionary<GameObject, float> coinDistances = new Dictionary<GameObject, float>();
        GameObject[] pockets = FindGameObjectsWithTagType(Tags.Pocket);

        foreach (GameObject coin in coins)
        {
            float minDistance = Mathf.Infinity;
            foreach (GameObject pocket in pockets)
            {
                float distance = Vector3.Distance(coin.transform.position, pocket.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }
            coinDistances[coin] = minDistance;
        }

        foreach (var coin in coinDistances.OrderBy(kv => kv.Value).Take(count))
        {
            closestCoins.Add(coin.Key);
        }

        return closestCoins;
    }
    private List<GameObject> FindClosestCoinsToStriker(List<GameObject> coins, int count)
    {
        List<GameObject> closestCoins = new List<GameObject>();
        Dictionary<GameObject, float> coinDistances = new Dictionary<GameObject, float>();

        foreach (GameObject coin in coins)
        {
            float distance = Vector3.Distance(coin.transform.position, transform.position);
            coinDistances[coin] = distance;
        }

        foreach (var coin in coinDistances.OrderBy(kv => kv.Value).Take(count))
        {
            closestCoins.Add(coin.Key);
        }

        return closestCoins;
    }








    private GameObject FindCoinWithMinAngleDifference(List<GameObject> closestToPockets, List<GameObject> closestToStriker)
    {
        GameObject bestCoin = null;
        float minAngleDifference = Mathf.Infinity;

        foreach (GameObject coin in closestToPockets.Concat(closestToStriker))
        {
            Vector3 coinPosition = coin.transform.position;
            GameObject[] pockets = FindGameObjectsWithTagType(Tags.Pocket);

            foreach (GameObject pocket in pockets)
            {
                Vector3 pocketPosition = pocket.transform.position;

                // Calculate the angle between the striker and the coin
                Vector3 strikerToCoinDirection = (coinPosition - transform.position).normalized;
                float strikerToCoinAngle = Mathf.Atan2(strikerToCoinDirection.y, strikerToCoinDirection.x) * Mathf.Rad2Deg;

                // Calculate the angle between the coin and the pocket
                Vector3 coinToPocketDirection = (pocketPosition - coinPosition).normalized;
                float coinToPocketAngle = Mathf.Atan2(coinToPocketDirection.y, coinToPocketDirection.x) * Mathf.Rad2Deg;

                // Calculate the absolute difference between the angles
                float angleDifference = Mathf.Abs(strikerToCoinAngle - coinToPocketAngle);

                // Calculate the distance between the coin and the pocket
                float distanceToPocket = Vector3.Distance(coinPosition, pocketPosition);

                // Check if the path is not blocked by white coins
                //if (!IsPathObstructedByWhiteCoin(transform.position, coinPosition) &&
                //    !IsPathObstructedByWhiteCoin(coinPosition, pocketPosition))
                {
                    // Prioritize coins with a distance to the pocket less than or equal to 2.8f
                    if (distanceToPocket <= 2.8f)
                    {
                        return coin; // Highest priority
                    }

                    // Otherwise, find the coin with the minimum angle difference
                    if (Mathf.Abs(angleDifference) < Mathf.Abs(minAngleDifference))
                    {
                        minAngleDifference = angleDifference;
                        bestCoin = coin;
                    }
                }
            }
        }

        return bestCoin;
    }


    //////////////////////////////////////////////////
    // Find all the available black coins
    //List<GameObject> coins = new List<GameObject>(FindGameObjectsWithTagType(Tags.Black));

    ////GameObject queenCoin = GameObject.FindGameObjectsWithTagType("Queen");
    //GameObject queenCoin = FindGameObjectWithTagType(Tags.Queen);

    //if (queenCoin != null)
    //{
    //    coins.Add(queenCoin);
    //}
    //Debug.Log("the length " + coins.Count);
    //GameObject closestCoin = null;
    //float closestDistance = Mathf.Infinity;

    //if (coins.Count == 0)
    //{
    //    Debug.Log("No coins left");
    //    yield break;
    //}



    ////changhe in the code below
    //foreach (GameObject coin in coins)
    //{
    //    Vector3 closestPocketPosition = GetClosestPocket(coin.transform.position);
    //    float distance = Vector3.Distance(coin.transform.position, closestPocketPosition);

    //    // Check if the path between the coin and the closest pocket is obstructed by a white coin
    //    if (!IsPathObstructedByWhiteCoin(coin.transform.position, closestPocketPosition))
    //    {
    //        if (distance < closestDistance)
    //        {
    //            closestCoin = coin;
    //            closestDistance = distance;
    //        }
    //    }
    //}


    //Vector3 targetDirection = closestCoin.transform.position - transform.position;
    //targetDirection.z = 0f;
    //float targetSpeed = CalculateStrikerSpeed(targetDirection.magnitude);

    //// Apply the calculated force to the striker and end the enemy's turn.
    //rb.AddForce(targetDirection.normalized * targetSpeed, ForceMode2D.Impulse);
    //GameManager.Instance.orgplayerturn = GameManager.Instance.playerTurn;
    //GameManager.Instance.playerTurn = true;
    //yield return new WaitForSeconds(0.1f);
    //yield return new WaitUntil(() => rb.velocity.magnitude < 0.1f);

    //yield return StartCoroutine(BoardManager.Instance.CheckQueenPocket());
    ////gameObject.SetActive(false);
    //Debug.Log("before end turn");
    ////here need to add
    //gameObject.SetActive(false);
    //if (BoardManager.Instance.lastPocketedObject == "Striker")
    //{
    //    GameManager.Instance.playerTurn = !GameManager.Instance.orgplayerturn;
    //    //here we can change
    //}
    //GameManager.Instance.EndTurn();
    //}


    private bool IsPathObstructedByWhiteCoin(Vector3 start, Vector3 end)
    {
        // Get all the white coin objects
        List<GameObject> whiteCoins = new List<GameObject>(FindGameObjectsWithTagType(Tags.White));

        // Check if any white coin intersects the line between the start and end points
        foreach (GameObject whiteCoin in whiteCoins)
        {
            Vector3 whiteCoinPosition = whiteCoin.transform.position;
            float distanceToLine = Vector3.Cross(end - start, whiteCoinPosition - start).magnitude / (end - start).magnitude;

            if (distanceToLine < whiteCoin.GetComponent<CircleCollider2D>().radius)
            {
                Vector3 projection = Vector3.Project(whiteCoinPosition - start, end - start) + start;
                if (Vector3.Distance(start, projection) + Vector3.Distance(projection, end) - Vector3.Distance(start, end) < 0.1f)
                {
                    return true; // White coin obstructing the path
                }
            }
        }

        return false; // No white coin obstructing the path
    }




    //private bool IsPathObstructedByWhiteCoin(Vector3 start, Vector3 end)
    //{
    //    // Get all the white coin objects
    //    List<GameObject> whiteCoins = new List<GameObject>(FindGameObjectsWithTagType(Tags.White));

    //    // Check if any white coin intersects the line between the start and end points
    //    foreach (GameObject whiteCoin in whiteCoins)
    //    {
    //        RaycastHit2D hit = Physics2D.Linecast(start, end, LayerMask.GetMask("Coin"));
    //        if (hit.collider != null && hit.collider.gameObject == whiteCoin)
    //        {
    //            return true; // White coin obstructing the path
    //        }
    //    }

    //    return false; // No white coin obstructing the path
    //}

    private bool IsColliderObstructing(Collider2D collider)
    {
        TagManager tagManager = collider.gameObject.GetComponent<TagManager>();
        if (tagManager != null)
        {
            switch (tagManager.tagType)
            {
                case Tags.Striker:
                case Tags.Black:
                case Tags.White:
                    return true;
            }
        }
        return false;
    }





    Vector3 GetClosestPocket(Vector3 position)
    {
        // Find the closest pocket to a given position
        Vector3 closestPocket = Vector3.zero;
        float closestDistance = Mathf.Infinity;
        GameObject[] pockets = FindGameObjectsWithTagType(Tags.Pocket);
        //whiteCoinCount = FindObjectsOfType<TagManager>().Count(t => t.tagType == Tags.White);
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

    float CalculateStrikerSpeed(float distance, bool isBackshot)
    {
        float maxDistance = 4.0f; // Maximum distance the striker can travel
        float minSpeed = 10f; // Minimum striker speed
        float maxSpeed = isBackshot ? 25f : 27f; // Maximum striker speed based on backshot

        float speed = Mathf.Lerp(minSpeed, maxSpeed, distance / maxDistance);
        return speed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Play the collision sound if the enemy striker collides with the board
        if (IsColliderOfType(other.collider, Tags.Board))
        {
            GetComponent<AudioSource>().Play();
        }
    }
    GameObject[] FindGameObjectsWithTagType(Tags tagType)
    {
        List<GameObject> taggedObjects = new List<GameObject>();
        TagManager[] allTagManagers = FindObjectsOfType<TagManager>();

        foreach (TagManager tagManager in allTagManagers)
        {
            if (tagManager.tagType == tagType)
            {
                taggedObjects.Add(tagManager.gameObject);
            }
        }

        return taggedObjects.ToArray();
    }
    private bool IsColliderOfType(Collider2D collider, Tags tagType)
    {
        TagManager tagManager = collider.gameObject.GetComponent<TagManager>();
        if (tagManager != null)
        {
            return tagManager.tagType == tagType;
        }
        return false;
    }
    GameObject FindGameObjectWithTagType(Tags tagType)
    {
        TagManager[] allTagManagers = FindObjectsOfType<TagManager>();

        foreach (TagManager tagManager in allTagManagers)
        {
            if (tagManager.tagType == tagType)
            {
                return tagManager.gameObject;
            }
        }

        return null;
    }

}