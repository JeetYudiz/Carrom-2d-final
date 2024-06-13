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
    }

    private void OnEnable()
    {
        strikerForceField.LookAt(transform.position);
        CollisionSoundManager.shouldBeStatic = true;
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = false;
        //here setaqctive thje circle lines and line

        //StartCoroutine(PlayerTurn());
        StartCoroutine(PositionStrikerWithoutCollision());

    }
    private IEnumerator PositionStrikerWithoutCollision()
    {
        bool isObstructed;
        Vector3 initialPosition = new Vector3(-0.25f, -4.55f, 0f);
        transform.position = initialPosition;

        do
        {
            isObstructed = false;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f);
            List<Collider2D> filteredColliders = new List<Collider2D>();

            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject != gameObject)
                {
                    filteredColliders.Add(collider);
                }
            }

            Debug.Log("Filtered count: " + filteredColliders.Count);

            if (filteredColliders.Count > 0)
            {
                isObstructed = true;
                Vector3 moveDirection = Vector3.zero;

                foreach (Collider2D collider in filteredColliders)
                {
                    Vector3 targetpos;
                    if (transform.position.x > collider.transform.position.x)
                    {
                        Debug.Log("1");
                        targetpos = collider.transform.position + new Vector3(BoardManager.Instance.CoinSize * 0.01f, 0, 0);
                    }
                    else
                    {
                        Debug.Log("2");
                        targetpos = collider.transform.position - new Vector3(BoardManager.Instance.CoinSize * 0.01f, 0, 0);
                    }

                    moveDirection = (targetpos - transform.position).normalized;

                    // Move the striker away from the colliding object
                    Vector3 newPosition = transform.position + moveDirection * 0.01f;
                    transform.position = newPosition;

                    // Check for collisions at the new position
                    Collider2D[] newColliders = Physics2D.OverlapCircleAll(newPosition, 0.2f);
                    if (newColliders.Length == 1 && newColliders[0].gameObject == gameObject)
                    {
                        // No collisions at the new position, exit the loop
                        isObstructed = false;
                        break;
                    }
                }

                if (isObstructed)
                {
                    // If still obstructed, generate a new random position
                    float randomX = Random.Range(-3.24f, 3.24f);
                    transform.position = new Vector3(randomX, -4.55f, 0f);
                }
            }

            yield return null;
        }
        while (isObstructed);

        Debug.Log("Sprite renderer enabled");
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;
        GameObject refStriker = GameObject.Find("RefStriker");
        if (refStriker != null)
        {
            Debug.Log("Updating RefStriker position");
            Vector3 refStrikerPosition = refStriker.transform.position;
            refStrikerPosition.x = transform.position.x;
            refStriker.transform.position = refStrikerPosition;
        }
    }
    //public IEnumerator PlayerTurn()
    //{
    //    const int maxAttempts = 10;
    //    int attempts = 0;
    //    bool isObstructed;
    //    Vector3 initialPosition = new Vector3(-0.25f, -4.55f, 0f);
    //    transform.position = initialPosition;

    //    do
    //    {
    //        isObstructed = false;
    //        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
    //        List<Collider2D> filteredColliders = new List<Collider2D>();

    //        foreach (Collider2D collider in colliders)
    //        {
    //            if (collider.gameObject != gameObject && (collider.CompareTag("Black") || collider.CompareTag("White") || collider.CompareTag("Striker")))
    //            {
    //                filteredColliders.Add(collider);
    //            }
    //        }

    //        if (filteredColliders.Count > 0)
    //        {
    //            isObstructed = true;
    //            // Generate a new random position within a specified range
    //            float randomX = Random.Range(-3.24f, 3.24f);
    //            transform.position = new Vector3(randomX, -4.55f, 0f);
    //        }

    //        attempts++;
    //    }
    //    while (isObstructed && attempts < maxAttempts);

    //    if (isObstructed)
    //    {
    //        Debug.Log("Failed to find a valid position for the player striker. Placing it at the initial position.");
    //        transform.position = initialPosition;
    //    }

    //    yield return null;

    //    Debug.Log("Sprite renderer enabled");
    //    GetComponent<SpriteRenderer>().enabled = true;

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
