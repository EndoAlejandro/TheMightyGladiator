using CustomUtils;
using Enemies.Combat;
using Enemies.DanceAI.States;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.DanceAI
{
    /// <summary>
    /// New, self-contained enemy brain built on the existing state machine
    /// (<see cref="FiniteStateBehaviour"/>). Movement is hand-rolled steering
    /// (see <see cref="SteeringBody"/>) so it stays smooth and can be driven by any
    /// state, including future attacks. The NavMesh is used only to know the walkable
    /// area. Uses <see cref="Enemies.Combat.CombatEnemy"/> for health, damage, death, pooling.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class DanceEnemyController : FiniteStateBehaviour
    {
        [field: Header("Spawn")]
        [field: SerializeField] public float SpawnTime { get; private set; } = 1f;


        [field: Header("Movement (steering)")]
        [field: SerializeField] public float MaxSpeed { get; private set; } = 3.5f;

        [field: Tooltip("How fast the steering velocity is allowed to change (units/s^2). Higher = snappier.")]
        [field: SerializeField] public float Acceleration { get; private set; } = 20f;

        [field: SerializeField] public float BrakeAcceleration { get; private set; } = 25f;

        [Tooltip("Distance from the target at which 'arrive' starts slowing down.")]
        [SerializeField] private float _arriveRadius = 1.5f;

        [SerializeField] private float _rotationSpeed = 12f;

        [field: Header("Avoidance")]
        [field: SerializeField] public float SeparationRadius { get; private set; } = 1.5f;

        [field: SerializeField] public float SeparationWeight { get; private set; } = 1.5f;

        [field: SerializeField] public float WallLookAhead { get; private set; } = 1.25f;

        [field: SerializeField] public float WallAvoidWeight { get; private set; } = 2f;

        [Header("Dance / Hover")]
        [Tooltip("Closest distance to the player from which this enemy attacks.")]
        [SerializeField] private float _minAttackDistance = 1.5f;

        [Tooltip("How far OUTSIDE the attack distance the enemy orbits while waiting its turn.")]
        [SerializeField] private float _danceRingOffset = 1.5f;

        [Tooltip("Random per-enemy variation added to the orbit radius so they spread across a band.")]
        [SerializeField] private float _danceRingJitter = 0.75f;

        [Tooltip("Degrees per second the enemy circles the player while dancing.")]
        [SerializeField] private float _orbitSpeed = 60f;

        [Tooltip("How hard the enemy corrects back toward its ring radius while orbiting.")]
        [SerializeField] private float _radialGain = 2f;

        [field:SerializeField] public float DanceEnterDistance{get; private set; } = 6f;
        [field:SerializeField] public float DanceExitDistance{get; private set; } = 9f;

        [field: Header("Attack")]
        [field: SerializeField]public float TelegraphTime { get; private set; } = 0.6f;

        [field: SerializeField] public float AttackTime { get; private set; }= 1f;

        [Tooltip("Time after an attack before this enemy can attack again.")]
        [SerializeField] private float _attackCooldown = 3f;

        [Header("Knockback")]
        [Tooltip("If false, a knockback can NOT interrupt this enemy while it is telegraphing.")]
        [SerializeField] private bool _canBeStunned = true;

        [field: SerializeField] public float KnockbackForce{ get; private set; } = 8f;
        [field: SerializeField] public float KnockbackTime { get; private set; }= 0.35f;

        [field: Header("Death")]
        [field: SerializeField] public float DeathTime{ get; private set; } = 1f;

        private CombatEnemy _enemy;
        private Rigidbody _rigidbody;
        private SteeringBody _body;

        private DanceSpawnState _spawn;
        private DanceChaseState _chase;
        private DanceHoverState _hover;
        private DanceTelegraphState _telegraph;
        private DanceAttackState _attack;
        private DanceKnockBackState _knockBack;
        private DanceDeathState _death;

        private float _attackReadyTime;
        private bool _holdsToken;

        public CombatEnemy Enemy => _enemy;

        public Collider Collider { get; private set; }

        public Vector3 Velocity => _body.Velocity;

        public float DanceRadius { get; private set; }

        public float OrbitDirection { get; private set; }

        public Vector3 PendingKnockDirection { get; private set; }

        public bool IsDancing { get; set; }

        public bool AttackGranted { get; private set; }

        public bool AttackReady => Time.time >= _attackReadyTime;

        public bool IsEligibleToAttack => IsDancing && AttackReady && !_holdsToken;

        public Vector3 PlayerPosition =>
            Player.Instance != null ? Player.Instance.transform.position : transform.position;

        public float DistanceToPlayer => Utils.FlatDirection(PlayerPosition, transform.position).magnitude;

        public bool InAttackRange => DistanceToPlayer <= _minAttackDistance + 0.15f;

        protected override void References()
        {
            _enemy = GetComponent<CombatEnemy>();
            _rigidbody = GetComponent<Rigidbody>();
            Collider = GetComponent<Collider>();

            // We move the body ourselves via MovePosition; physics must not fight us.
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            _body = new SteeringBody(this, _rigidbody);
        }

        protected override void StateMachine()
        {
            _spawn = new DanceSpawnState(this);
            _chase = new DanceChaseState(this);
            _hover = new DanceHoverState(this);
            _telegraph = new DanceTelegraphState(this);
            _attack = new DanceAttackState(this);
            _knockBack = new DanceKnockBackState(this);
            _death = new DanceDeathState(this);

            stateMachine.AddTransition(_spawn, _chase, () => _spawn.Ended);

            stateMachine.AddTransition(_chase, _hover, () => DistanceToPlayer <= DanceEnterDistance);
            stateMachine.AddTransition(_hover, _chase, () => DistanceToPlayer > DanceExitDistance);

            stateMachine.AddTransition(_hover, _telegraph, () => AttackGranted && AttackReady && InAttackRange);
            stateMachine.AddTransition(_telegraph, _attack, () => _telegraph.Ended);
            stateMachine.AddTransition(_attack, _hover, () => _attack.Ended);

            stateMachine.AddTransition(_knockBack, _chase, () => _knockBack.Ended);
            stateMachine.AddTransition(_death, _spawn, () => _death.Ended);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();              // state.FixedTick() sets the movement intent
            _body.Tick(Time.fixedDeltaTime); // apply steering + navmesh clamp + smoothing
        }

        private void OnEnable()
        {
            DanceRadius = _minAttackDistance + _danceRingOffset + Random.Range(-_danceRingJitter, _danceRingJitter);
            OrbitDirection = Random.value < 0.5f ? -1f : 1f;
            _attackReadyTime = Time.time;
            _holdsToken = false;
            AttackGranted = false;
            IsDancing = false;

            _body.Reset();
            if (Collider != null) Collider.enabled = true;

            _enemy.OnHit += EnemyOnHit;
            _enemy.OnDead += EnemyOnDead;

            EnemyDirector.Instance.Register(this);
            stateMachine.SetState(_spawn);
        }

        private void OnDisable()
        {
            if (_enemy != null)
            {
                _enemy.OnHit -= EnemyOnHit;
                _enemy.OnDead -= EnemyOnDead;
            }

            ReleaseToken();
            if (EnemyDirector.HasInstance) EnemyDirector.Instance.Unregister(this);
        }

        private void EnemyOnDead(CombatEnemy enemy) => stateMachine.SetState(_death);

        private void EnemyOnHit(Vector3 hitPoint, float knockBack)
        {
            if (!_enemy.IsAlive) return; // killing blow -> Death handles it
            if (CurrentStateType == _death) return;
            if (!_canBeStunned && CurrentStateType == _telegraph) return; // telegraph super-armor

            PendingKnockDirection = Utils.NormalizedFlatDirection(transform.position, hitPoint);
            stateMachine.SetState(_knockBack);
        }

        // ---------- movement intents (call from a state's FixedTick) ----------

        /// <summary>Steer straight toward a point at full speed.</summary>
        public void SteerSeek(Vector3 target) =>
            _body.Steer(Utils.NormalizedFlatDirection(target, transform.position) * MaxSpeed);

        /// <summary>Steer toward a point, easing down inside <see cref="arriveRadius"/>.</summary>
        public void SteerArrive(Vector3 target)
        {
            var toTarget = Utils.FlatDirection(target, transform.position);
            var distance = toTarget.magnitude;
            if (distance < 0.0001f)
            {
                _body.Steer(Vector3.zero);
                return;
            }

            var speed = distance < _arriveRadius ? MaxSpeed * (distance / _arriveRadius) : MaxSpeed;
            _body.Steer(toTarget / distance * speed);
        }

        /// <summary>Circle the player at <see cref="DanceRadius"/>, correcting the radius as it goes.</summary>
        public void SteerOrbit()
        {
            var fromPlayer = Utils.FlatDirection(transform.position, PlayerPosition);
            var radius = fromPlayer.magnitude;
            if (radius < 0.0001f)
            {
                _body.Steer(transform.forward * MaxSpeed);
                return;
            }

            var radialDir = fromPlayer / radius;
            var tangentDir = Vector3.Cross(Vector3.up, radialDir) * OrbitDirection;
            var tangentSpeed = Mathf.Min(_orbitSpeed * Mathf.Deg2Rad * radius, MaxSpeed);
            var radial = radialDir * ((DanceRadius - radius) * _radialGain);

            _body.Steer(Vector3.ClampMagnitude(tangentDir * tangentSpeed + radial, MaxSpeed));
        }

        /// <summary>Move with an exact velocity (knockback, scripted attack lunges). No smoothing.</summary>
        public void Drive(Vector3 velocity) => _body.Drive(velocity);

        // ---------- facing (call from a state's Tick) ----------

        public void FacePlayer() => RotateTowards(Utils.NormalizedFlatDirection(PlayerPosition, transform.position));

        public void FaceMovement()
        {
            var v = _body.Velocity;
            v.y = 0f;
            if (v.sqrMagnitude > 0.01f) RotateTowards(v.normalized);
        }

        private void RotateTowards(Vector3 flatDirection)
        {
            if (flatDirection.sqrMagnitude < 0.001f) return;
            var target = Quaternion.LookRotation(flatDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, _rotationSpeed * Time.deltaTime);
        }

        public void WarpTo(Vector3 position)
        {
            transform.position = position;
            _rigidbody.position = position;
            _body.Reset();
        }

        // ---------- director token plumbing ----------

        public void GrantToken()
        {
            _holdsToken = true;
            AttackGranted = true;
        }

        public void ReleaseToken()
        {
            if (!_holdsToken) return;
            _holdsToken = false;
            AttackGranted = false;
            if (EnemyDirector.HasInstance) EnemyDirector.Instance.OnTokenReleased();
        }

        public void BeginAttackCooldown() => _attackReadyTime = Time.time + _attackCooldown;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _minAttackDistance);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, DanceEnterDistance);
        }
    }
}