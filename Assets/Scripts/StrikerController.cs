using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StrikerController : MonoBehaviour
{
    [SerializeField]
    float strikerSpeed;

    [SerializeField]
    float maxScale = 1f;

    [SerializeField]
    Transform strikerForceField;
    [SerializeField]
    Transform DottedLine;


    [SerializeField]
    Slider strikerSlider;


    [SerializeField] SpriteRenderer circleSpriteRenderer;
    [SerializeField] GameObject ArrowHandle;
    [SerializeField] GameObject ArrowHead;
    bool isMoving;
    bool isCharging;
    float maxForceMagnitude = 30f;
    Rigidbody2D rb;
    //public float strikeraroundlayerlength ;
    Vector3 orgpos;
    public Image Slider;
    Vector3 circleSpriteOrgScale;
    Vector3 arrowhandleScale;

    //public static bool playerTurn;

    private void Start()
    {
        //playerTurn = true;
        isMoving = false;
        rb = GetComponent<Rigidbody2D>();
        orgpos =transform.position;
        circleSpriteOrgScale = circleSpriteRenderer.transform.localScale;
        arrowhandleScale = ArrowHandle.transform.localScale;

    }

    private void OnEnable()
    {
        strikerForceField.LookAt(transform.position);
        CollisionSoundManager.shouldBeStatic = true;
        GetComponent<SpriteRenderer>().enabled = false;
       
        //here setaqctive thje circle lines and line

        StartCoroutine(PlayerTurn());
    }
    public void DisplayOutput()
    {

        Debug.Log("here in diuspalky output");
    }

    /// <summary>
    /// //////////////////
    /// </summary>
    /// <returns></returns>
    //public IEnumerator PlayerTurn()
    //{
    //    const int maxAttempts = 10;
    //    int attempts = 0;
    //    bool isObstructed;
    //    float x = -0.25f;
    //    transform.position = new Vector3(x, -4.55f, 0f);
    //    do
    //    {
    //        isObstructed = false;

    //        // Generate a random position within the board bounds


    //        // Check if the generated position is obstructed by other coins or the striker
    //        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);

    //        foreach (Collider2D collider in colliders)
    //        {
    //            if (collider.CompareTag("Black") || collider.CompareTag("White") || collider.CompareTag("Striker"))
    //            {
    //                isObstructed = true;
    //                break;
    //            }
    //        }
    //        Debug.Log("here");
    //        attempts++;
    //    }
    //    while (isObstructed && attempts < maxAttempts);

    //    if (isObstructed)
    //    {
    //        Debug.Log("Failed to find a valid position for the enemy striker.");
    //        transform.position = new Vector3(-0.25f, -4.55f, 0f);
    //        isObstructed = false;
    //    }

    //    yield return new WaitWhile(() => isObstructed);
    //    Debug.Log("sprite renderer ");
    //    //here i need tp set tje refstriker also
    //    GameObject refStriker = GameObject.Find("RefStriker"); // Replace "RefStriker" with the actual name of the refStriker GameObject
    //    if (refStriker != null)
    //    {
    //        Debug.Log("here in ref striker");
    //        Vector3 refStrikerPosition = refStriker.transform.position;
    //        refStrikerPosition.x = transform.position.x;
    //        refStriker.transform.position = refStrikerPosition;
    //    }
    //    GetComponent<SpriteRenderer>().enabled = true;
    //}
    public IEnumerator PlayerTurn()
    {
        const int maxAttempts = 10;
        int attempts = 0;
        bool isObstructed;
        Vector3 initialPosition = new Vector3(-0.25f, -4.55f, 0f);
        transform.position = initialPosition;

        do
        {
            isObstructed = false;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Black") || collider.CompareTag("White") || collider.CompareTag("Striker"))
                {
                    
                    isObstructed = true;
                    break;
                }
            }

            if (isObstructed)
            {
                Debug.Log("here in isobstructed");
                // Generate a new random position within a specified range
                float randomX = UnityEngine.Random.Range(-3.24f, 3.24f);
                transform.position = new Vector3(randomX, -4.55f, 0f);
            }

            attempts++;
        }
        while (isObstructed && attempts < maxAttempts);

        if (isObstructed)
        {
            Debug.Log("Failed to find a valid position for the player striker. Placing it at the initial position.");
            transform.position = initialPosition;
        }

        yield return null;

        Debug.Log("Sprite renderer enabled");
        GetComponent<SpriteRenderer>().enabled = true;

        GameObject refStriker = GameObject.Find("RefStriker");
        if (refStriker != null)
        {
            Debug.Log("Updating RefStriker position");
            Vector3 refStrikerPosition = refStriker.transform.position;
            refStrikerPosition.x = transform.position.x;
            refStriker.transform.position = refStrikerPosition;
        }
    }
    /// <summary>
    /// //////////////////////
    /// </summary>
    private void Update()
    {
        //Debug.Log("striker " + transform.position);
      

    }

    private void OnMouseDown()
    {
        // If the striker is moving, disable charging and return
        if (rb.velocity.magnitude > 0.1f)
        {
            isCharging = false;
            return;
        }

       
        isCharging = true;
        strikerForceField.gameObject.SetActive(true);
        ArrowHandle.gameObject.SetActive(true);
        circleSpriteRenderer.enabled = true;
        circleSpriteRenderer.transform.localScale = new Vector3(5f, 5f, 5f);
        DottedLine.gameObject.SetActive(true);
        ArrowHead.gameObject.SetActive(true);

    }

    private void OnMouseUp()
    {
        GameManager.Instance.slider.SetActive(false);
        ArrowHandle.gameObject.SetActive(false);
        ArrowHead.gameObject.SetActive(false);
        strikerForceField.gameObject.SetActive(false);
        circleSpriteRenderer.enabled = false;
        DottedLine.gameObject.SetActive(false);
        isMoving = true;

        // If charging is not enabled, return
        if (!isCharging)
        {
            return;
        }

        isCharging = false;
        
        Vector3 direction = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction.z = 0f;
        float forceMagnitude = direction.magnitude * strikerSpeed;
        forceMagnitude = Mathf.Clamp(forceMagnitude, 0f, maxForceMagnitude);
        rb.AddForce(direction.normalized * forceMagnitude, ForceMode2D.Impulse);
        CollisionSoundManager.shouldBeStatic = false;
        //1st strike problem
        GameManager.Instance.orgplayerturn = GameManager.Instance.playerTurn;
        GameManager.Instance.playerTurn = false;
        // Wait until the striker comes to a near stop
        StartCoroutine(WaitForStrikerStop());
    }

    private IEnumerator WaitForStrikerStop()
    {
        yield return new WaitUntil(() => rb.velocity.magnitude < 0.1f);

        isMoving = false;
        isCharging = false;

        yield return StartCoroutine(BoardManager.Instance.CheckQueenPocket());
        gameObject.SetActive(false);

        //check here for sriker special condition 
        if (BoardManager.Instance.lastPocketedObject == "Striker")
        {
            GameManager.Instance.playerTurn = !GameManager.Instance.orgplayerturn;
            //here we can change
        }

        GameManager.Instance.EndTurn();

    }
    private void OnMouseDrag()
    {
        // If charging is not enabled, return
        if (!isCharging)
        {
            return;
        }

        Vector3 direction = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction.z = 0f;
        strikerForceField.transform.up= direction.normalized;
        DottedLine.transform.up = direction.normalized;

        float scaleValue = Vector3.Distance(transform.position, transform.position + direction / 4f);

        if (scaleValue > maxScale)
        {
            scaleValue = maxScale;
        }

        strikerForceField.localScale = new Vector3(scaleValue, scaleValue, scaleValue);

     
        //elongate along y axis
        //Vector3 circleSpriteRendererScale = circleSpriteRenderer.transform.localScale;
        if (circleSpriteRenderer.transform.localScale.x <= 20)
        {
            circleSpriteRenderer.transform.localScale = new Vector3(circleSpriteOrgScale.x + scaleValue * 15f, circleSpriteOrgScale.y + scaleValue * 15f, circleSpriteOrgScale.z + scaleValue * 15f);
        }
        if (arrowhandleScale.y <= 20)
        {
            ArrowHandle.transform.localScale = new Vector3(arrowhandleScale.x, arrowhandleScale.y + scaleValue * 24f, circleSpriteOrgScale.z ); ;
        }

    }

    public void SetSliderX()
    {
        // Set the X position of the striker based on the slider value
        if (rb.velocity.magnitude < 0.1f)
        {
            transform.position = new Vector3(strikerSlider.value, -4.57f, 0);
        }

    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        // Play the collision sound if the striker collides with the board
        if (other.gameObject.CompareTag("Board"))
        {
            GetComponent<AudioSource>().Play();
        }
    }
   
    public void DisableCollider()
    {
        GetComponent<CircleCollider2D>().enabled = false;
    }
    public void EnableCollider()
    {
        GetComponent<CircleCollider2D>().enabled = true;
    }

    ///////////////////////////////////////////////////
    //public void RefStrikerMover(Vector3 despos)
    //{
    //float moveDuration = 0.1f;
    // Vector3 newDesPos = new Vector3(despos.x,orgpos.y, despos.z);
    ////SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
    //Sequence sequence = DOTween.Sequence();
    //sequence.Append(BoardManager.Instance.refStriker.transform.DOMove(despos, moveDuration));
    //    sequence.Play();
    //}

    /// <summary>
    /// /////////////
    /// </summary>
    /// <param name="refstrikerpos"></param>
    /// <returns></returns>
  
    public void StrikerMover(Vector3 despos)
    {
        Debug.Log("the destiabntioopnm pos " + despos);
        float moveDuration = 0.1f;
        despos = new Vector3(despos.x, orgpos.y, 0f);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(despos, moveDuration));

        sequence.OnComplete(() =>
        {

            //EnableCollider();

        });
        sequence.Play();

    }
    /// <summary>
    /// /////////////
    /// </summary>
    /// <param name="refstrikerpos"></param>
    /// <returns></returns>
    //private Vector3 GetStrikerPosition(Vector3 refstrikerpos)
    //{
    //    // Clamp the ref striker's x-coordinate within the defined range
    //    refstrikerpos.x = Mathf.Clamp(refstrikerpos.x, xleftextreme, xrightextreme);
    //    refstrikerpos.y = orgpos.y;

    //    return refstrikerpos;
    //}

    //public void StrikerMover(Vector3 despos)
    //{
    //    //Debug.Log("the destination pos " + despos);
    //    float moveDuration = 0.1f;

    //    // Convert the ref striker's position to the striker's position
    //   \
    //    SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
    //    Sequence sequence = DOTween.Sequence();
    //    sequence.Append(transform.DOMove(strikerpos, moveDuration));

    //    sequence.OnComplete(() =>
    //    {
    //        // Additional logic or actions to perform after the movement is complete
    //    });

    //    sequence.Play();
    //}

    ////private Vector3 ConvertRefStrikerPosToStrikerPos(Vector3 refstrikerpos)
    //{
    //    // Map the ref striker's x-coordinate to the striker's movement range
    //    float strikerX = Mathf.Lerp(-3.24f, 3.24f, Mathf.InverseLerp(xleftextreme, xrightextreme, refstrikerpos.x));

    //    // Create the striker position vector
    //    Vector3 strikerpos = new Vector3(strikerX, -4.57f, 0f);

    //    return strikerpos;
    //}
    //void CalculateFixedCoordinates()
    //{
    //    // Get the RectTransform of the RawImage
    //    RectTransform rectTransform = Slider.rectTransform;

    //    // Get the corners of the RectTransform in screen space
    //    Vector3[] corners = new Vector3[4];
    //    rectTransform.GetWorldCorners(corners);

    //    // Convert screen space corners to world space
    //    //for (int i = 0; i < 4; i++)
    //    //{
    //    //    corners[i] = Camera.main.ScreenToWorldPoint(corners[i]);
    //    //    Debug.Log("the cormers " + corners[i]);
    //    //}

    //    // The minimum x-coordinate is the smallest x-value among the corners
    //    float minX = Mathf.Min(corners[0].x, corners[1].x, corners[2].x, corners[3].x);

    //    // The maximum x-coordinate is the largest x-value among the corners
    //    float maxX = Mathf.Max(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
    //    xleftextreme = minX;
    //    xrightextreme = maxX;
    //    Debug.Log("Minimum X in World Space: " + minX);
    //    Debug.Log("Maximum X in World Space: " + maxX);
    //}
    //public IEnumerator HandleCollisions(Vector3 moveDirection)
    //{
    //    Debug.Log("the layer length " + BoardManager.Instance.refStriker.strikeraroundlayerlength);
    //    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, BoardManager.Instance.refStriker.strikeraroundlayerlength);
    //    bool collided = false;
    //    Debug.Log("here in colliders " + hitColliders.Length);
    //    foreach (var collider in hitColliders)
    //    {
    //        if (collider.gameObject != gameObject)
    //        {
    //            Vector3 targetpos;
    //            Vector3 targetpos1;
    //            Vector3 targetpos2;
    //            if (moveDirection == Vector3.zero)
    //            {
    //                if (transform.position.x > collider.transform.position.x)
    //                {
    //                    targetpos = collider.gameObject.transform.position + new Vector3(BoardManager.Instance.CoinSize * 1.5f, 0, 0);
    //                }
    //                else
    //                {
    //                    targetpos = collider.gameObject.transform.position - new Vector3(BoardManager.Instance.CoinSize * 1.5f, 0, 0);
    //                }
    //            }
    //            else
    //            {
    //                targetpos1 = collider.gameObject.transform.position + new Vector3(BoardManager.Instance.CoinSize * 1.5f, 0, 0);
    //                Vector3 dir1 = (targetpos1 - transform.position).normalized;
    //                float angle1 = Vector3.Angle(moveDirection, dir1);

    //                targetpos2 = collider.gameObject.transform.position - new Vector3(BoardManager.Instance.CoinSize * 1.5f, 0, 0);
    //                Vector3 dir2 = (targetpos2 - transform.position).normalized;
    //                float angle2 = Vector3.Angle(moveDirection, dir2);

    //                if (angle1 < angle2)
    //                {
    //                    targetpos = targetpos1;
    //                }
    //                else
    //                {
    //                    targetpos = targetpos2;
    //                }
    //            }

    //            BoardManager.Instance.refStriker.RefStrikerMover(BoardManager.Instance.refStriker.GetStrikerPosition(targetpos));
    //            StrikerMover(BoardManager.Instance.refStriker.GetStrikerPosition(targetpos));
    //            collided = true;

    //            // Update the moveDirection to the current direction
    //            moveDirection = (targetpos - transform.position).normalized;
    //            if (targetpos.x > BoardManager.Instance.refStriker.xrightextreme)
    //            {
    //                Debug.Log("here in greater than xrightextreme");
    //                // Change moveDirection to the opposite side
    //                moveDirection = new Vector3(-moveDirection.x, moveDirection.y, moveDirection.z);
    //            }
    //            else if (targetpos.x < BoardManager.Instance.refStriker.xleftextreme)
    //            {
    //                Debug.Log("here in less than xleftextreme");
    //                // Change moveDirection to the opposite side
    //                moveDirection = new Vector3(-moveDirection.x, moveDirection.y, moveDirection.z);
    //            }
    //        }
    //    }

    //    if (collided)
    //    {
    //        // If a collision occurred, wait for a short delay and then continue handling collisions
    //        yield return new WaitForSeconds(0.1f);
    //        Debug.Log("collided");

    //        StartCoroutine(HandleCollisions(moveDirection));
    //    }
    //    else
    //    {
    //        // If no collision occurred, immediately enable the collider
    //        Debug.Log("here in enable collider");
    //        EnableCollider();
    //        GetComponent<SpriteRenderer>().enabled=true;
    //    }
    //}

}
