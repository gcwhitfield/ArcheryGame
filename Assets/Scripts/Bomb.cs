using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bomb : MonoBehaviour
{
    [Header("Explosion")]
    public AudioClip explodeSound;
    public GameObject explodeEffect;
    public GameObject tank; // the tank that this bomb was shot from
    public float explodeRadius;
    public float explodeStrength;
    public int damageAmt;

    [Header("Camera follow")]
    [Range(0f, 1f)]
    public float cameraFollowDelay;
    public int followSpeed;
    public float offsetX;
    public float offsetY;
    public float offsetZ;
    private SmartCamera _smartCam;
    private Vector3 camFollowOffset;
    

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Tank" || other.gameObject.tag == "Ground")
        {
            StartCoroutine("Explode"); // damage applied in explode
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _smartCam = Camera.main.GetComponent<SmartCamera>();
        camFollowOffset = 
            (transform.forward * offsetZ) + 
            (transform.right * offsetX) + 
            (transform.up * offsetY);
    }

    /* Play explode efect, apply force to nearby tanks, end turn */
    IEnumerator Explode()
    {
        // play sound effeect
        if (explodeSound != null)
            AudioManager.Instance.PlaySoundEffect(explodeSound);
        
        // create effect, apply force to nearby tanks
        if (explodeEffect != null)
            Instantiate(explodeEffect, gameObject.transform.position, gameObject.transform.rotation);
        
        Collider [] objs = Physics.OverlapSphere(gameObject.transform.position, explodeRadius);
        foreach (Collider c in objs)
        {
            if (c.gameObject.tag == "Tank")
            {
                // force
                Vector3 dist = c.gameObject.transform.position - gameObject.transform.position ;
                Vector3 force = (dist.normalized / explodeRadius) * explodeStrength;
                c.gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
                // damage
                c.gameObject.GetComponent<Tank>().Damage(Mathf.FloorToInt(damageAmt * force.magnitude));
                Debug.Log("Here is the damage amt: " + damageAmt * force.magnitude);
            }
        }

        Debug.Log("Ending turn...");
        // end turn
        tank.GetComponent<Tank>().EndTurn();

        Destroy(gameObject);
        yield break;
    }

    void FixedUpdate()
    {
        CameraFollow();
    }

    /* Camera follow */
    void CameraFollow()
    {
        if (_smartCam == null)
        {
             _smartCam = Camera.main.GetComponent<SmartCamera>();
        }
        
        Vector3 desiredPosition = gameObject.transform.position + camFollowOffset;
        _smartCam.LookAt(gameObject.transform.position);
        _smartCam.SetPosition(Vector3.Slerp(_smartCam.transform.position, desiredPosition, cameraFollowDelay));
    }

}
