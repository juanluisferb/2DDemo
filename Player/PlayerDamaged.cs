using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CICEman.Player
{
    public class PlayerDamaged : PlayerState
    {

        [SerializeField] float _invulnerableTime;
        SpriteRenderer _spriteRenderer;
        bool _invulnerable;
        [SerializeField] AudioClip _damagedClip;

        protected override void Awake()
        {
            base.Awake();
            _spriteRenderer = GetComponent<SpriteRenderer>();


        }

        private void OnEnable()
        {
            _animator.SetBool("isDamaged", true);
            AudioSource.PlayClipAtPoint(_damagedClip, this.transform.position);
            Invoke("ResetToIdle", _invulnerableTime);
            _invulnerable = true;

            //Constraints
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemies"), true);

            //Sólo el jugador 1 parpadea
            if(GameManager.singleton.selectedCharacter == 0)
            {
                StartCoroutine(Blink());
            }
            
        }

        private void OnDisable()
        {
            CancelInvoke("ResetToIdle");
            _invulnerable = false;
            _animator.SetBool("isDamaged", false);
            //El jugador 1 se vuelve invulnerable en el tiempo de daño
            if (GameManager.singleton.selectedCharacter == 0)
            {
                StopAllCoroutines();
            }
               
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemies"), false);
        }

        void ResetToIdle()
        {
            _player.SetNextState(Player.State.Default);
        }

        IEnumerator Blink()
        {
         //el player es invulnerable durante el tiempo que dura la animación, parpadea
            while (_invulnerable)
            {
                yield return new WaitForSeconds(0.2f);
                _spriteRenderer.enabled = false;
                yield return new WaitForSeconds(0.1f);
                _spriteRenderer.enabled = true;
            }
        }
    }
}



