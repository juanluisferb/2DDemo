using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CICEman.Player;

public class EnemyRobot : Character {

    [SerializeField] AudioClip _shootClip;
    [SerializeField] Animator _spriteHitAnimatorPrefab;
    [SerializeField] Transform _hitTransform;
    AnimatorStateInfo info;



    protected override void Start()
    {
        base.Start();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerFireBall"))
        {
            //Este fragmento instancia un animator breve para dar feedback al jugador de que el enemigo está siendo dañado
            Animator spriteHit = Instantiate(_spriteHitAnimatorPrefab);
            spriteHit.transform.position = _hitTransform.position;
            AudioSource.PlayClipAtPoint(_shootClip, this.transform.position);

            info = spriteHit.GetCurrentAnimatorStateInfo(0);
            if (info.length > info.normalizedTime)
            {
                Destroy(spriteHit.gameObject, 0.2f);
            }
        }
    }
}
