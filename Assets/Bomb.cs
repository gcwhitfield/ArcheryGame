using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bomb : MonoBehaviour
{
    public AudioClip explodeSound;
    public GameObject explodeEffect;
    public float explodeRadius;
    public float explodeStrength;
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
    
    }

    IEnumerator Explode()
    {
        // play sound effeect
        if (explodeSound != null)
            AudioManager.Instance.PlaySoundEffect(explodeSound);
        
        // create effect, apply force to nearby tanks
        if (explodeEffect != null)
            Instantiate(explodeEffect, gameObject.transform.position, gameObject.transform.rotation);
        RaycastHit hit;
        if (Physics.SphereCast(gameObject.transform.position, explodeRadius, Vector3.zero, out hit, 0.1f))
        {
            if (hit.collider.gameObject.tag == "Tank")
            {
                Vector3 dist = gameObject.transform.position - hit.collider.gameObject.transform.position;
                Vector3 force = (dist.normalized / explodeRadius) * explodeStrength;
                
                hit.rigidbody.AddForce(force, ForceMode.Force);
            }
        }

        Destroy(gameObject);
        yield break;
    }
}
