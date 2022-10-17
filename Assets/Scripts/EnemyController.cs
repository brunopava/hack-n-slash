using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = transform.GetChild(0).GetComponent<Animator>();
    }

    public void TakeDamage()
    {
        // StartCoroutine(DamageAnimation());
        _animator.SetTrigger("damage");
    }


    private IEnumerator DamageAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("TakeDamage");
        
    }
}
