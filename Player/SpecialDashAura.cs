using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CICEman.Player;

public class SpecialDashAura : MonoBehaviour {

    Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponentInParent<Rigidbody2D>();
    }

    //inflinge daño máximo al contacto con los enemigos
    private void OnTriggerEnter2D(Collider2D other)
    {
        Character _chara = other.GetComponent<Character>();
        if(_chara != null)
        {
            _chara.TakeDamage(50.0f);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            _rigidbody.velocity = Vector2.zero;
        }
    }

}
