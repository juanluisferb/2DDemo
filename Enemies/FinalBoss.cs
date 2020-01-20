using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CICEman.Player;

public class FinalBoss : MonoBehaviour {

    #region Variables
    EnemyRobot _boss;
    Player _player;
    [SerializeField] Transform _shootPoint;
    GameObject _gate;
    float _timeToAttack;
    float _timeBetweenAttacks = 5.0f;
    float vulnerabilityTime;
    Vector3 correction = new Vector3(0, 2.2f, 0);
    bool _isVulnerable;
    FinalBossIsVulnerable _fVulnerable;
    Animator _bossAnimator;

    #endregion

    private void Awake()
    {
        _boss = GetComponent<EnemyRobot>();
        _player = FindObjectOfType<Player>();
        _gate = GameObject.Find("BossGate");
        _fVulnerable = GetComponent<FinalBossIsVulnerable>();
        _bossAnimator = GetComponent<Animator>();
    }
    private void Start()
    {
        InvokeRepeating("IsVulnerable", 1.0f, 5.0f);
        InvokeRepeating("LookForPlayer", 0, 4.0f);
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(_shootPoint.transform.position, _player.transform.position + correction);

        if(Time.time == _timeToAttack)
        {
            
            _timeToAttack = Time.time + _timeBetweenAttacks;
        }

    }

    void IsVulnerable()
    {
        _fVulnerable.enabled = true;

    }

    void LookForPlayer()
    {

    }
}
