using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyAi : MonoBehaviour
{
    public float strikerSpeed = 10f;
    public float maxStrikerForce = 30f;
    public LayerMask coinLayer;
    public LayerMask boardLayer;
    public float boardWidth = 10f;
    public float boardHeight = 10f;
    public float pocketRadius = 1f;

    private Rigidbody2D strikerRigidbody;
    private List<GameObject> blackCoinObjects;
    private GameObject redCoinObject;
    private List<GameObject> pockets;
    private GameManager gameManager;
    private bool isMoving;
    public float strikerRadius = 0.37f;
    public float puckRadius = 0.26f;
    public float strikeThreshold = 0.5f;
    public float speedAdder = 1f;
    public Vector2 speedMinMax = Vector2.one + Vector2.up * 100f;
    private void Start()
    {
        strikerRigidbody = GetComponent<Rigidbody2D>();

        // Find objects with the TagManager component
        TagManager[] tagManagers = FindObjectsOfType<TagManager>();

        // Filter objects based on their tagType
        blackCoinObjects = new List<GameObject>();
        pockets = new List<GameObject>();

        foreach (TagManager tagManager in tagManagers)
        {
            if (tagManager.tagType == Tags.Black)
            {
                blackCoinObjects.Add(tagManager.gameObject);
            }
            else if (tagManager.tagType == Tags.Queen)
            {
                redCoinObject = tagManager.gameObject;
            }
            else if (tagManager.tagType == Tags.Pocket)
            {
                pockets.Add(tagManager.gameObject);
            }
        }

        gameManager = FindObjectOfType<GameManager>();
        speedMinMax = Vector2.one + Vector2.up * 100f;
        Debug.Log("the speed min max "+ Vector2.up);
        //Debug.Log("the speed min max " + speedMinMax.y);
    }

    private void OnEnable()
    {
        Debug.Log("here in onenable");
        isMoving = false;
    }

    private void Update()
    {
        if (strikerRigidbody.velocity.magnitude < 0.1f && !isMoving)
        {
            isMoving = true;
            Debug.Log("here in less than 0.1f");
            StartCoroutine(EnemyTurn());
        }
    }

    private IEnumerator EnemyTurn()
    {
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

            attempts++;
        }
        while (isObstructed && attempts < maxAttempts);

        if (isObstructed)
        {
            Debug.Log("Failed to find a valid position for the enemy striker.");
            transform.position = new Vector3(0f, 3.45f, 0f);
            isObstructed = false;
        }

        Debug.Log("sprite renderer ");
        GetComponent<SpriteRenderer>().enabled = true;

        yield return new WaitForSeconds(2f);
        CollisionSoundManager.shouldBeStatic = false;
        yield return new WaitForSeconds(0.1f);

        // Find the target coin
        GameObject targetCoin = GetHighestPriorityCoin();
        Debug.Log("the target coin " + targetCoin);
        // Check for direct shot
        if (CanTakeDirectShot(targetCoin))
        {
            TakeDirectShot(targetCoin);
        }
        else
        {
            // Check for reflected shot
           
            {
                // Check for intermediate coin shot
                if (CanTakeIntermediateCoinShot(targetCoin))
                {
                    TakeIntermediateCoinShot(targetCoin);
                }
                else
                {
                    // Take a defensive shot or skip turn
                    TakeDefensiveShot();
                }
            }
        }

        yield return new WaitWhile(() => strikerRigidbody.velocity.magnitude > 0.1f);

        GameManager.Instance.orgplayerturn = GameManager.Instance.playerTurn;
        GameManager.Instance.playerTurn = true;

        yield return new WaitUntil(() => strikerRigidbody.velocity.magnitude < 0.1f);

        gameObject.SetActive(false);
        if (BoardManager.Instance.lastPocketedObject == "Striker")
        {
            GameManager.Instance.playerTurn = !GameManager.Instance.orgplayerturn;
        }

        GameManager.Instance.EndTurn();



        //GameObject targetCoin = GetHighestPriorityCoin();
        //Debug.Log("the target coin " + targetCoin);

        //// Calculate the best shot
        //Vector2 strikerPos = strikerRigidbody.position;
        //Vector2 targetPos = ConvertToVector2(targetCoin.transform.position);
        //Vector2 pocketPos = GetNearestPocket(targetPos);

        //Vector2 hitDir = (pocketPos - targetPos).normalized;
        //Vector2 hitPos = targetPos - hitDir * (puckRadius + strikerRadius);

        //float hitDot = Vector2.Dot(hitDir, (hitPos - strikerPos).normalized);
        //float tSpeed = Vector2.Distance(targetPos, pocketPos) + Vector2.Distance(hitPos, strikerPos);
        //tSpeed = (tSpeed / hitDot) + speedAdder;
        //tSpeed = Mathf.Clamp(tSpeed, speedMinMax.x, speedMinMax.y);
        //Debug.Log("the speed min max x"+speedMinMax.x);
        //Debug.Log("the speed min max y" + speedMinMax.y);
        //Debug.Log("the t speed "+tSpeed);
        //if (hitDot > strikeThreshold)
        //{
        //    // Take the shot
        //    strikerRigidbody.AddForce((hitPos - strikerPos).normalized * tSpeed*2f, ForceMode2D.Impulse);
        //}
        //else
        //{
        //    // Take a defensive shot or skip turn
        //    TakeDefensiveShot();
        //}

        yield return new WaitWhile(() => strikerRigidbody.velocity.magnitude > 0.1f);

        GameManager.Instance.orgplayerturn = GameManager.Instance.playerTurn;
        GameManager.Instance.playerTurn = true;

        yield return new WaitUntil(() => strikerRigidbody.velocity.magnitude < 0.1f);

        gameObject.SetActive(false);
        if (BoardManager.Instance.lastPocketedObject == "Striker")
        {
            GameManager.Instance.playerTurn = !GameManager.Instance.orgplayerturn;
        }

        GameManager.Instance.EndTurn();
    }

    private Vector3 Get_NormalPoint_OnLine(Vector3 p, Vector3 a, Vector3 b)
    {
        Vector3 ap = p - a;
        Vector3 ab = b - a;
        ab.Normalize();
        ab *= Vector3.Dot(ap, ab);
        return a + ab;
    }

    private float? Get_Ray_OnLineSeg(Vector3 rayOrg, Vector3 rayDir, Vector3 point1, Vector3 point2)
    {
        var v1 = rayOrg - point1;
        var v2 = point2 - point1;
        var v3 = new Vector3(-rayDir.y, rayDir.x, 0);

        var dot = Vector2.Dot(v2, v3);
        if (Mathf.Abs(dot) < 0.000001f)
            return null;

        var rM = Vector3.Cross(v2, v1) / dot;
        var lM = Vector2.Dot(v1, v3) / dot;

        if (rM.z >= 0.0f && (lM >= 0.0f && lM <= 1.0f))
            return rM.z;

        return null;
    }
    private bool SetStrikerPosition()
    {
        const int maxAttempts = 10;
        int attempts = 0;
      
        while (attempts < maxAttempts)
        {
            Debug.Log("inside " + attempts);
            float x = Random.Range(-3.24f, 3.24f);
            transform.position = new Vector3(x, 3.45f, 0f);
            Debug.Log("1");
            if (!IsPositionObstructed(transform.position))
            {
                Debug.Log("2");
                return true;
            }
            Debug.Log("3");
            attempts++;
        }

        return false;
    }

    private bool IsPositionObstructed(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
        return colliders.Any(collider => IsColliderObstructing(collider));
    }

    private bool IsColliderObstructing(Collider2D collider)
    {
        TagManager tagManager = collider.gameObject.GetComponent<TagManager>();
        return tagManager != null && (tagManager.tagType == Tags.Striker || tagManager.tagType == Tags.Black || tagManager.tagType == Tags.White);
    }

    private GameObject GetHighestPriorityCoin()
    {
        GameObject highestPriorityCoin = null;
        float minDistanceToPocket = float.MaxValue;

        IEnumerable<GameObject> coins = blackCoinObjects.Concat(new[] { redCoinObject }).Where(coin => coin != null);
        GameObject firstCoin = coins.FirstOrDefault();
        Debug.Log("the first coin "+ firstCoin);
        foreach (GameObject coin in coins)
        {
            Debug.Log("7");
            Vector2 coinPosition = coin.transform.position;
            Vector2 nearestPocketPosition = GetNearestPocket(coinPosition);
            float distanceToPocket = Vector2.Distance(coinPosition, nearestPocketPosition);

            if (distanceToPocket < minDistanceToPocket && CanTakeDirectShot(coin))
            {
                Debug.Log("8");
                minDistanceToPocket = distanceToPocket;
                highestPriorityCoin = coin;
            }
        }

        return highestPriorityCoin;
    }
    private Vector2 ConvertToVector2(Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.y);
    }

    // Use this method to convert positions where necessary
    private bool CanTakeDirectShot(GameObject targetCoin)
    {
        Vector2 strikerPosition = strikerRigidbody.position;
        Vector2 targetCoinPosition = ConvertToVector2(targetCoin.transform.position);
        return IsLineOfSightClear(strikerPosition, targetCoinPosition);
    }

    private void TakeDirectShot(GameObject targetCoin)
    {
        Vector2 targetCoinPosition = ConvertToVector2(targetCoin.transform.position);
        Vector2 direction = (GetNearestPocket(targetCoinPosition) - targetCoinPosition).normalized;
        float force = Mathf.Clamp(strikerSpeed * Vector2.Distance(targetCoinPosition, strikerRigidbody.position), 0f, maxStrikerForce);
        Debug.Log("the force applied "+ force);
        strikerRigidbody.AddForce(direction * force, ForceMode2D.Impulse);
    }

    private bool CanTakeReflectedShot(GameObject targetCoin)
    {
        Vector2 strikerPosition = strikerRigidbody.position;
        Vector2 targetCoinPosition = ConvertToVector2(targetCoin.transform.position);
        Vector2 strikerToTarget = targetCoinPosition - strikerPosition;
        Vector2 reflectionDirection = Vector2.Reflect(strikerToTarget, Vector2.Perpendicular(strikerToTarget));

        return IsLineOfSightClear(strikerPosition, reflectionDirection);
    }

    private void TakeReflectedShot(GameObject targetCoin)
    {
        Vector2 strikerPosition = strikerRigidbody.position;
        Vector2 targetCoinPosition = ConvertToVector2(targetCoin.transform.position);
        Vector2 strikerToTarget = targetCoinPosition - strikerPosition;
        Vector2 reflectionDirection = Vector2.Reflect(strikerToTarget, Vector2.Perpendicular(strikerToTarget));
        float force = Mathf.Clamp(strikerSpeed * strikerToTarget.magnitude, 0f, maxStrikerForce);
        Debug.Log("the force applied " + force);
        strikerRigidbody.AddForce(reflectionDirection.normalized * force, ForceMode2D.Impulse);
    }

    private bool CanTakeIntermediateCoinShot(GameObject targetCoin)
    {
        Vector2 strikerPosition = strikerRigidbody.position;
        Vector2 targetCoinPosition = ConvertToVector2(targetCoin.transform.position);

        foreach (GameObject intermediateCoin in blackCoinObjects.Where(coin => coin != targetCoin))
        {
            Vector2 intermediateCoinPosition = ConvertToVector2(intermediateCoin.transform.position);
            if (IsLineOfSightClear(strikerPosition, intermediateCoinPosition) &&
                IsLineOfSightClear(intermediateCoinPosition, targetCoinPosition))
            {
                return true;
            }
        }

        return false;
    }

    private void TakeIntermediateCoinShot(GameObject targetCoin)
    {
        Vector2 strikerPosition = strikerRigidbody.position;
        Vector2 targetCoinPosition = ConvertToVector2(targetCoin.transform.position);

        foreach (GameObject intermediateCoin in blackCoinObjects.Where(coin => coin != targetCoin))
        {
            Vector2 intermediateCoinPosition = ConvertToVector2(intermediateCoin.transform.position);
            if (IsLineOfSightClear(strikerPosition, intermediateCoinPosition) &&
                IsLineOfSightClear(intermediateCoinPosition, targetCoinPosition))
            {
                Vector2 direction = (intermediateCoinPosition - strikerPosition).normalized;
                float force = Mathf.Clamp(strikerSpeed * Vector2.Distance(intermediateCoinPosition, strikerPosition), 0f, maxStrikerForce);
                strikerRigidbody.AddForce(direction * force, ForceMode2D.Impulse);
                break;
            }
        }
    }

    private bool IsLineOfSightClear(Vector2 start, Vector2 end)
    {
        RaycastHit2D hit = Physics2D.Raycast(start, end - start, Vector2.Distance(start, end), coinLayer);
        return hit.collider == null;
    }

    private Vector2 GetNearestPocket(Vector2 coinPosition)
    {
        GameObject nearestPocket = pockets.OrderBy(pocket => Vector2.Distance(coinPosition, ConvertToVector2(pocket.transform.position))).FirstOrDefault();
        return nearestPocket != null ? ConvertToVector2(nearestPocket.transform.position) : Vector2.zero;
    }
    private void TakeDefensiveShot()
    {
        Vector2 centerPosition = new Vector2(0f, 0f);
        Vector2 direction = (centerPosition - strikerRigidbody.position).normalized;
        float force = Mathf.Clamp(strikerSpeed * Vector2.Distance(centerPosition, strikerRigidbody.position), 0f, maxStrikerForce);
        strikerRigidbody.AddForce(direction * force, ForceMode2D.Impulse);
        Debug.Log("the force applied " + force);
    }
}
