using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    float maxForceMagnitude = 40f;
    Rigidbody2D rb;
    //public float strikeraroundlayerlength ;
    Vector3 orgpos;
    public Image Slider;
    Vector3 circleSpriteOrgScale;
    Vector3 arrowhandleScale;
    public bool isObstructed = false;
    public float strikeraroundlayerlength = 0f;
    public float xleftextreme;
    public float xrightextreme;
    //public static bool playerTurn;

    private void Start()
    {
        //playerTurn = true;
        isMoving = false;
        rb = GetComponent<Rigidbody2D>();
        orgpos =transform.position;
        circleSpriteOrgScale = circleSpriteRenderer.transform.localScale;
        arrowhandleScale = ArrowHandle.transform.localScale;
        maxForceMagnitude = 40f;
        xleftextreme = -2.84f; // Adjust this value based on your game's dimensions
        xrightextreme = 2.84f;
        strikeraroundlayerlength = 0.3f;
    }

    private void OnEnable()
    {
        strikerForceField.LookAt(transform.position);
        CollisionSoundManager.shouldBeStatic = true;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        //here setaqctive thje circle lines and line

        //StartCoroutine(PlayerTurn());
        StartCoroutine(PositionStrikerWithoutCollision());

    }

    /// <summary>
    /// /////////////////
    /// </summary>
    /// <returns></returns>
    private IEnumerator PositionStrikerWithoutCollision()
    {
        Vector3 initialPosition = new Vector3(0f, -4f, 0f);
        transform.position = initialPosition;

        yield return StartCoroutine(HandleCollisions(Vector3.zero));

        // Enable sprite renderer and collider
        Debug.Log("here adter handling collision");
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;

        // Update RefStriker position
        
        GameObject refStriker = GameObject.Find("RefStriker");
        if (refStriker != null)
        {
            Debug.Log("set the ref striker pos "+refStriker);
            Vector3 refStrikerPosition = refStriker.transform.position;
            refStrikerPosition.x = transform.position.x;
            refStriker.transform.position = refStrikerPosition;
        }
        if(refStriker==null)
        {
            Debug.Log("<color=blue>The ref striker null </color>" + refStriker);
        }

    }

    private IEnumerator HandleCollisions(Vector3 moveDirection)
    {


        bool collided = false;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, strikeraroundlayerlength);
        Debug.Log("the movr dior "+ moveDirection);
        Debug.Log("striker aroind lauyer " + strikeraroundlayerlength);
        Debug.Log(" here in collider "+ hitColliders[0].gameObject.name);
        foreach (var collider in hitColliders)
        {
            if (collider.gameObject != gameObject)
            {
                Vector3 targetpos;
                if (moveDirection == Vector3.zero)
                {
                    if (transform.position.x > collider.transform.position.x)
                    {
                        targetpos = collider.transform.position + new Vector3(BoardManager.Instance.CoinSize * 1.4f, 0, 0);
                    }
                    else
                    {
                        targetpos = collider.transform.position - new Vector3(BoardManager.Instance.CoinSize * 1.4f, 0, 0);
                    }
                }
                else
                {
                    Vector3 targetpos1 = collider.transform.position + new Vector3(BoardManager.Instance.CoinSize * 1.4f, 0, 0);
                    Vector3 targetpos2 = collider.transform.position - new Vector3(BoardManager.Instance.CoinSize * 1.4f, 0, 0);

                    Vector3 dir1 = (targetpos1 - transform.position).normalized;
                    Vector3 dir2 = (targetpos2 - transform.position).normalized;

                    float angle1 = Vector3.Angle(moveDirection, dir1);
                    float angle2 = Vector3.Angle(moveDirection, dir2);

                    targetpos = (angle1 < angle2) ? targetpos1 : targetpos2;
                    Debug.Log("the target pos "+targetpos);
                }

                targetpos = GetStrikerPosition(targetpos);
                //RefStrikerMover(targetpos);
                StrikerMover(targetpos);
                collided = true;

                moveDirection = (targetpos - transform.position).normalized;
                if (targetpos.x > xrightextreme || targetpos.x < xleftextreme)
                {
                    moveDirection = new Vector3(-moveDirection.x, moveDirection.y, moveDirection.z);
                }
                Debug.Log("the direction "+moveDirection);
                break;
            }
        }

        if (collided)
        {
            yield return new WaitForSeconds(0.1f);
            Debug.Log("Collided, handling collisions again");
            hitColliders = null;
            Debug.Log("the move dir "+moveDirection);
            yield return StartCoroutine(HandleCollisions(moveDirection));
        }
        else
        {
            Debug.Log("No collision, enabling collider");
            //EnableCollider();
        }
    }

    private Vector3 GetStrikerPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, xleftextreme, xrightextreme);
        position.y = orgpos.y;
        return new Vector3(position.x, position.y, 0f);
    }

    //public void StrikerMover(Vector3 despos)
    //{
    //    float moveDuration = 0.1f;
    //    despos = new Vector3(despos.x, orgpos.y, 0f);
    //    transform.DOMove(despos, moveDuration);
    //}
    /// <summary>
    /// ///////////
    /// </summary>

    //private IEnumerator PositionStrikerWithoutCollision()
    //{
    //    //bool isObstructed;
    //    Vector3 initialPosition = new Vector3(0f, -4f, 0f);
    //    //check if obstruced
    //    if (!isObstructed)
    //    {
    //        transform.position = initialPosition;
    //    }
    //    do
    //    {
    //        isObstructed = false;
    //        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f);
    //        List<Collider2D> filteredColliders = new List<Collider2D>();
    //        //List<Collider2D> reColliders = new List<Collider2D>();
    //        foreach (Collider2D collider in colliders)
    //        {
    //            if (collider.gameObject != gameObject)
    //            {
    //                filteredColliders.Add(collider);
    //            }
    //        }

    //        Debug.Log("Filtered count: " + filteredColliders.Count);

    //        if (filteredColliders.Count > 0)
    //        {
    //            isObstructed = true;
    //            Vector3 moveDirection = Vector3.zero;

    //            foreach (Collider2D collider in filteredColliders)
    //            {
    //                Vector3 targetpos;
    //                if (transform.position.x > collider.transform.position.x)
    //                {
    //                    Debug.Log("1");
    //                    //collider transform position causomng a problem
    //                    Vector3 AddedVector = new Vector3(collider.transform.position.x * 0.01f, collider.transform.position.y, collider.transform.position.z);
    //                    targetpos = AddedVector;
    //                }
    //                else
    //                {
    //                    Debug.Log("2");
    //                    Vector3 AddedVector = new Vector3(collider.transform.position.x * 0.01f, collider.transform.position.y, collider.transform.position.z);
    //                    targetpos = AddedVector;
    //                }

    //                moveDirection = (targetpos - transform.position).normalized;
    //                //targetpos = collider.transform.position
    //                // Move the striker away from the colliding object
    //                //Debug.Log("the move direction " + moveDirection);
    //                //Debug.Log("the new move direction " + moveDirection*0.001f);
    //                //Debug.Log("the present position "+transform.position);
    //                //Debug.Log("the current transform position  " + transform.position);
    //                Vector3 newPosition = transform.position + moveDirection * 0.01f;
    //                Debug.Log("the present pos " + newPosition);
    //                //                   Debug.Log("the new position " + newPosition);
    //                //                   Debug.Log("the current collider position "+ collider.transform.position);
    //                //                   Debug.Log("tghe coin size "+ new Vector3(BoardManager.Instance.CoinSize * 0.00001f, 0, 0)
    //                //);
    //                Debug.Log("the current striker pos " + transform.position);
    //                transform.position = newPosition;
    //                Debug.Log("t");
    //                // Check for collisions at the new position
    //                Collider2D[] newColliders = Physics2D.OverlapCircleAll(newPosition, 0.2f);
    //                //foreach (Collider2D recollider in newColliders)
    //                //{
    //                //    if (collider.gameObject != gameObject)
    //                //    {
    //                //        reColliders.Add(collider);
    //                //    }
    //                //}
    //                Debug.Log("befire 1");
    //                //if (reColliders.Count == 1 && newColliders[0].gameObject == gameObject)
    //                if (newColliders[0].gameObject == gameObject)
    //                {
    //                    Debug.Log("here in count 1");
    //                    // No collisions at the new position, exit the loop
    //                    isObstructed = false;
    //                    break;
    //                }
    //            }

    //            if (isObstructed)
    //            {
    //                // If still obstructed, generate a new random position
    //                float randomX = Random.Range(-2.84f, 2.84f);
    //                //the below loop running again and agaoomn
    //                transform.position = new Vector3(randomX, -4f, 0f);
    //            }
    //        }

    //        yield return null;
    //    }
    //    while (isObstructed);

    //    Debug.Log("Sprite renderer enabled");
    //    GetComponent<SpriteRenderer>().enabled = true;
    //    GetComponent<CircleCollider2D>().enabled = true;
    //    GameObject refStriker = GameObject.Find("RefStriker");
    //    if (refStriker != null)
    //    {
    //        Debug.Log("Updating RefStriker position");
    //        Vector3 refStrikerPosition = refStriker.transform.position;
    //        refStrikerPosition.x = transform.position.x;
    //        refStriker.transform.position = refStrikerPosition;
    //    }
    //}

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
    
        EnableStrikerComponents();
        //Enabke striker eleebnt
  
    }

    public void EnableStrikerComponents()
    {
        strikerForceField.gameObject.SetActive(true);
        ArrowHandle.gameObject.SetActive(true);
        circleSpriteRenderer.enabled = true;
        circleSpriteRenderer.transform.localScale = new Vector3(5f, 5f, 5f);
        DottedLine.gameObject.SetActive(true);
        ArrowHead.gameObject.SetActive(true);
    }
    public void OnDisable()
    {
        ArrowHandle.gameObject.SetActive(false);
        Debug.Log("after ArrowHandle setactive false");
        ArrowHead.gameObject.SetActive(false);
        Debug.Log("after ArrowHead setactive false");
        strikerForceField.gameObject.SetActive(false);
        Debug.Log("after strikerForceField setactive false");
        circleSpriteRenderer.enabled = false;
        Debug.Log("after circleSpriteRenderer setactive false");
        DottedLine.gameObject.SetActive(false);
    }

    private void OnMouseUp()
    {
        isMoving = true;

        //disabke striker eleebnt
        // If charging is not enabled, return
        GameManager.Instance.turntimer.gameObject.SetActive(false);
        //GameManager.Instance.
        DisableStrikerComponents();
        //here disable timer
        if (!isCharging)
        {
            return;
        }

        isCharging = false;
        
        Vector3 direction = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction.z = 0f;
        float forceMagnitude = direction.magnitude * strikerSpeed;
        forceMagnitude = Mathf.Clamp(forceMagnitude, 0f, maxForceMagnitude);
        Debug.Log("the force magnitude " + forceMagnitude);
        if (forceMagnitude > 12f)
        {
            rb.AddForce(direction.normalized * forceMagnitude, ForceMode2D.Impulse);
        }
        else
        {
            forceMagnitude = 0f;
            rb.AddForce(direction.normalized * forceMagnitude, ForceMode2D.Impulse);
        }
        CollisionSoundManager.shouldBeStatic = false;
        //1st strike problem
        GameManager.Instance.orgplayerturn = GameManager.Instance.playerTurn;
        GameManager.Instance.playerTurn = false;
        // Wait until the striker comes to a near stop
        StartCoroutine(WaitForStrikerStop());
    }

    public void DisableStrikerComponents()
    {
        GameManager.Instance.slider.SetActive(false);
        Debug.Log("after slider setactive false");
        ArrowHandle.gameObject.SetActive(false);
        Debug.Log("after ArrowHandle setactive false");
        ArrowHead.gameObject.SetActive(false);
        Debug.Log("after ArrowHead setactive false");
        strikerForceField.gameObject.SetActive(false);
        Debug.Log("after strikerForceField setactive false");
        circleSpriteRenderer.enabled = false;
        Debug.Log("after circleSpriteRenderer setactive false");
        DottedLine.gameObject.SetActive(false);
        Debug.Log("after DottedLine setactive false");
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
   

}
