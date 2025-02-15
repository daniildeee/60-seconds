using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision){
        Transform hitTransform = collision.transform;
        if(hitTransform.CompareTag("Player"))
        {
            Debug.Log("Hit player");
            hitTransform.GetComponent<PlayerHealth>().TakeDamage(10);
        }
        Destroy(gameObject);

    }
}
