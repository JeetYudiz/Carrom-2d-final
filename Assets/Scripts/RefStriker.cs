using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RefStriker : MonoBehaviour
{
    //public Vector3 leftextreme;
    //private Vector3 righttextreme;
    public SpriteRenderer parentsprite;
    public float xleftextreme;
    public float xrightextreme;
    private Vector3 refstrikerpos;
    private Vector3 orgpos;
    public StrikerController currentstriker;
    //private Vector3 currentstrikerorgpos;
    private int layerMask;
    int strikerLayer;
    public float strikeraroundlayerlength = 1f;
    Vector3 previousMousePosition;
    public Vector3 dragDirection;
    [SerializeField] GameObject StrikerCrossHair;
    // Start is called before the first frame update
    void Start()
    {
        orgpos = transform.position;
        xleftextreme = transform.position.x - Mathf.Abs(parentsprite.bounds.min.x);
        xrightextreme = transform.position.x + Mathf.Abs(parentsprite.bounds.max.x);
        
        //currentstrikerorgpos = currentstriker.transform.position;
        strikerLayer = LayerMask.NameToLayer("Striker"); // Replace "Striker" with the actual layer name
        layerMask = ~(1 << strikerLayer);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnMouseDown()
    {
        //turn on the outercircle
        StrikerCrossHair.gameObject.SetActive(true);
       currentstriker.DisableCollider();
    }
    private void OnMouseDrag()
    {

        refstrikerpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = GetStrikerPosition(refstrikerpos);
        currentstriker.transform.position = new Vector3(transform.position.x, currentstriker.transform.position.y,0f);
    }

    public Vector3 GetStrikerPosition(Vector3 refstrikerpos)
    {
        refstrikerpos.x = Mathf.Clamp(refstrikerpos.x, xleftextreme, xrightextreme);
        refstrikerpos.y = orgpos.y;
        Vector3 strikerpos = new Vector3(refstrikerpos.x, orgpos.y, 0f);
        return strikerpos;
    }


    public void RefStrikerMover(Vector3 despos)
    {
        float moveDuration = 0.1f;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(despos, moveDuration));
        sequence.Play();
    }
    public void RefStrikerSet(Transform despos)
    {
        transform.position = despos.position;
    }

    private void OnMouseUp()
    {
     
        StrikerCrossHair.gameObject.SetActive(false);
        StartCoroutine(HandleCollisions(Vector3.zero));
    }

    public IEnumerator HandleCollisions(Vector3 moveDirection)
    {
        Debug.Log("the layer length " + strikeraroundlayerlength);
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(currentstriker.transform.position, strikeraroundlayerlength);
        bool collided = false;
        Debug.Log("here in colliders " + hitColliders.Length);
        foreach (var collider in hitColliders)
        {
            //Debug.Log("here in colliders " + hitColliders.Length);
            if (collider.gameObject != currentstriker.gameObject)
            {

                Vector3 targetpos;
                Vector3 targetpos1;
                Vector3 targetpos2;
                if (moveDirection == Vector3.zero)
                {
                    if (currentstriker.transform.position.x > collider.transform.position.x)
                    {

                        targetpos = collider.gameObject.transform.position + new Vector3(BoardManager.Instance.CoinSize * 1.2f, 0, 0);
                    }
                    else
                    {

                        targetpos = collider.gameObject.transform.position - new Vector3(BoardManager.Instance.CoinSize * 1.2f, 0, 0);
                    }
                }
                else
                {
                    targetpos1 = collider.gameObject.transform.position + new Vector3(BoardManager.Instance.CoinSize * 1.2f, 0, 0);
                    Vector3 dir1 = (targetpos1 - transform.position).normalized;
                    float angle1 = Vector3.Angle(moveDirection, dir1);

                    targetpos2 = collider.gameObject.transform.position - new Vector3(BoardManager.Instance.CoinSize * 1.2f, 0, 0);
                    Vector3 dir2 = (targetpos2 - transform.position).normalized;
                    float angle2 = Vector3.Angle(moveDirection, dir2);

                    if (angle1 < angle2)
                    {
                        targetpos = targetpos1;
                    }
                    else
                    {
                        targetpos = targetpos2;
                    }
                }
                Debug.Log("before RefStrikerMover");
                RefStrikerMover(GetStrikerPosition(targetpos));
                currentstriker.StrikerMover(GetStrikerPosition(targetpos));
                collided = true;

                // Update the moveDirection to the current direction
                moveDirection = (targetpos - transform.position).normalized;
                if (targetpos.x > xrightextreme)
                {
                    Debug.Log("here in greater than xrightextreme");
                    // Change moveDirection to the opposite side
                    moveDirection = new Vector3(-moveDirection.x, moveDirection.y, moveDirection.z);
                }
                else if (targetpos.x < xleftextreme)
                {
                    Debug.Log("here in less than xleftextreme");
                    // Change moveDirection to the opposite side
                    moveDirection = new Vector3(-moveDirection.x, moveDirection.y, moveDirection.z);
                }


            }
        }

        if (collided)
        {
            // If a collision occurred, wait for a short delay and then continue handling collisions
            yield return new WaitForSeconds(0.1f);
            Debug.Log("collided");

            StartCoroutine(HandleCollisions(moveDirection));
        }
        else
        {
            // If no collision occurred, immediately enable the collider
            Debug.Log("here in enable collider");
            //turn on the redds

            currentstriker.EnableCollider();
        }
    }
}
