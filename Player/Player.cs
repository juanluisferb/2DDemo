using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace CICEman.Player
{


    public class PlayerData
    {
        public Vector3 position;
        public Player.State currentState;
        public ItemType[] items;

    }


    public class Player : Character
    {

        public enum State
        {
            Default,
            Climb,
            Jump,
            Damaged,
            Shoot,
            Dash,
            SpecialDash
        }

        State _currentState;
        State _nextState;

        #region variables
        [Header("Physics")]
        [SerializeField] internal Transform _wallChecker;
        [SerializeField] internal Transform[] _groundCheckers;
        [SerializeField] internal LayerMask _floorLayerMask;
        [SerializeField] internal LayerMask _enemiesLayerMask;
        

        [Header("Animators")]
        [SerializeField] RuntimeAnimatorController[] _characterAnimators;

        [Header("Effects")]
        [SerializeField] ParticleSystem _particleSystemPrefab;
        [SerializeField] AudioClip _deathClip;

        [Header("References")]
        GameManager _gameManager;
        Animator _animator;
        Rigidbody2D _rigidbody;
        SpriteRenderer _spriteRenderer;
        PlayerItems _playerItems;

        Dictionary<State, PlayerState> _stateBehaviours;
        public bool _thereIsWall;
        public bool _isGrounded;
        bool _canDash = true;
        bool _playerIsDead = false;

        #endregion

        public void SetNextState(State desiredState)
        {
            _nextState = desiredState;
   
        }

        public State GetCurrentState() { return _currentState; }

        public bool GetIsGrounded() { return _isGrounded; }
        public bool CanDash
        {
            get { return _canDash; }
            set { _canDash = value;  }
        }

        public bool GetPlayerIsDead()
        {
            return _playerIsDead;
        }

        void Awake()
        {
            //Selección de personaje
            GetComponent<Animator>().runtimeAnimatorController = _characterAnimators[GameManager.singleton.selectedCharacter];

            PlayerState[] states = GetComponents<PlayerState>();
            _stateBehaviours = new Dictionary<State, PlayerState>();

            for (int i=0; i< states.Length; i++)
            {
                PlayerState s = states[i];
                s.enabled = false;
                _stateBehaviours.Add(s.State, s);
            }

            _playerItems = GetComponent<PlayerItems>();
            _gameManager = FindObjectOfType<GameManager>();
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void Start()
        {
            base.Start();
            _currentState = State.Default;
            _stateBehaviours[_currentState].enabled = true;

        }


        public void UpdateIsGrounded()
        {
            _isGrounded = false;
            for(int i=0; i<_groundCheckers.Length && !_isGrounded; i++)
            {
                Transform groundChecker = _groundCheckers[i];
                Collider2D ground = Physics2D.OverlapCircle(groundChecker.position, 0.1f, _floorLayerMask);
                _isGrounded = (ground != null);
            }

            
        }

        public bool CheckIsLanding()
        {
            for (int i = 0; i < _groundCheckers.Length && !_isGrounded; i++)
            {
                Transform groundChecker = _groundCheckers[i];
                RaycastHit2D hit = Physics2D.CircleCast(groundChecker.position, 0.1f, Vector2.down, 0.5f, _floorLayerMask);
                if(hit.collider != null)
                {
                    return true;
                }
                
            }

            return false;
        }

        public void CheckWall()
        { 
            Collider2D wall = Physics2D.OverlapCircle(_wallChecker.position, 0.5f, _floorLayerMask);
            _thereIsWall = (wall != null);

        }

        private void LateUpdate()
        {
            if (_currentState != _nextState)
            {
                UpdateBehaviour();
                _currentState = _nextState;
            }
                

            
        }

        public override void Die()
        {
            //Control para no llamar varias veces a este metodo al mismo tiempo
            if (!IsInvoking("Die") && !_playerIsDead)
            {  
                //Se lanza el estado de muerte en el animator y se ignoran las colisiones para que la transición sea fluida
                //Se anula la velocidad del player y la gravedad
                _animator.SetTrigger("isDead");
                _playerIsDead = true;
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemies"), false);
                AudioSource.PlayClipAtPoint(_deathClip, this.transform.position);
                Vector3 velocity = _rigidbody.velocity;
                _rigidbody.gravityScale = 0.0f;
                _rigidbody.velocity = Vector2.zero;
                /*velocity.y = 0.0f;
                velocity.x = 0.0f;
                _rigidbody.velocity = velocity;*/


                StartCoroutine(Death());
            }
            
            
        }

        IEnumerator Death()
        {
            

            //instancio particulas de animación
            ParticleSystem ps = Instantiate(_particleSystemPrefab);
            ps.transform.position = this.transform.position;
            Destroy(ps, ps.main.duration);

            yield return new WaitForSeconds(0.6f);
            _spriteRenderer.enabled = false;
            yield return new WaitForSeconds(1.0f);
            
            
            _gameManager.EndGame(false);
            
        }

        private void UpdateBehaviour()
        {
            MonoBehaviour currentBehaviour = _stateBehaviours[_currentState];
            MonoBehaviour nextBehaviour = _stateBehaviours[_nextState];

            if (!_playerIsDead) {

                if (currentBehaviour != nextBehaviour)
                {
                    currentBehaviour.enabled = false;
                    nextBehaviour.enabled = true;

                }
            }
            
            
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            //Función utilizada en las plataformas móviles para evitar vibraciones
            if (other.CompareTag("Platform"))
            {

                this.transform.SetParent(other.transform.parent);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Platform"))
            {
                this.transform.SetParent(null);
            }
        }


        #region Save-Load
        //Método de carga
        public PlayerData GetPlayerData()
        {
            PlayerData playerData = new PlayerData();
            playerData.position = this.transform.position;
            playerData.currentState = _currentState;
            playerData.items = _playerItems.GetItemsData();

            return playerData;
        }

        //Método de guardado
        public void SetPlayerData(PlayerData playerData)
        {
            this.transform.position = playerData.position;

            _nextState = playerData.currentState; 
            UpdateBehaviour();
            _playerItems.SetItemsData(playerData.items);
        }

        #endregion

        


    }

}


