using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bomb : MonoBehaviour
{
    public AudioClip explodeSound;
    public GameObject explodeEffect;
    public float explodeRadius;
    public float explodeStrength;
    public int damageAmt;
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
        Collider [] objs = Physics.OverlapSphere(gameObject.transform.position, explodeRadius);
        foreach (Collider c in objs)
        {
            if (c.gameObject.tag == "Tank")
            {
                // force
                Vector3 dist = gameObject.transform.position - c.gameObject.transform.position;
                Vector3 force = (dist.normalized / explodeRadius) * explodeStrength;
                c.gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Force);
                // damage
                c.gameObject.GetComponent<Tank>().Damage(Mathf.FloorToInt(damageAmt * force.magnitude));
            }
        }

        Destroy(gameObject);
        yield break;
    }
}
