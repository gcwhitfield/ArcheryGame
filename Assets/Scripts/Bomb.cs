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
    public float delay;
    public int followSpeed;
    public float offsetX;
    public float offsetY;
    public float offsetZ;
    private Camera _cam;
    

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Tank" || other.gameObject.isStatic)
        {
            StartCoroutine("Explode");
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _cam = Camera.main;
        StartCoroutine("CameraFollow");
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
            Debug.Log(c);
            if (c.gameObject.tag == "Tank")
            {
                // force
                Vector3 dist = c.gameObject.transform.position - gameObject.transform.position ;
                Vector3 force = (dist.normalized / explodeRadius) * explodeStrength;
                c.gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
                // damage
                c.gameObject.GetComponent<Tank>().Damage(Mathf.FloorToInt(damageAmt * force.magnitude));
            }
        }

        Debug.Log("Ending turn...");
        // end turn
        tank.GetComponent<Tank>().EndTurn();

        Destroy(gameObject);
        yield break;
    }



    /* Camera follow */
    IEnumerator CameraFollow()
    {
        Debug.Log("CameraFollow");
        if (_cam == null) _cam = Camera.main;
        while (true) // run until object destroyed
        {
            Vector3 desiredPosition = gameObject.transform.position + new Vector3(offsetX, offsetY, offsetZ);
            _cam.transform.LookAt(gameObject.transform);
            _cam.transform.position = Vector3.Slerp(_cam.transform.position, desiredPosition, delay);
            yield return null;
        }
    }

}
