using CustomUtils;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.DanceAI
{
    /// <summary>
    /// Reusable locomotion for the dancing enemies. States feed it an intent each
    /// physics step and it produces smooth motion:
    ///
    ///   * <see cref="Steer"/>  : a desired velocity from a steering behaviour
    ///                            (seek / arrive / orbit). Separation from neighbours
    ///                            and wall-avoidance are layered on automatically, and
    ///                            the result is acceleration-limited for smoothness.
    ///   * <see cref="Drive"/>  : an exact velocity (knockback, scripted attack lunges)
    ///                            applied as-is, no smoothing.
    ///   * no intent            : brakes smoothly to a stop.
    ///
    /// The NavMesh is used ONLY to know the walkable area: motion is clamped so the
    /// body slides along navmesh edges instead of crossing them. It never drives the
    /// movement itself, so attack states are free to push the body around.
    /// </summary>
    public class SteeringBody
    {
        private readonly DanceEnemyController _owner;
        private readonly Rigidbody _rigidbody;

        private Vector3 _velocity;
        private Vector3 _steer;
        private Vector3 _drive;
        private bool _hasSteer;
        private bool _hasDrive;

        public Vector3 Velocity => _velocity;

        public SteeringBody(DanceEnemyController owner, Rigidbody rigidbody)
        {
            _owner = owner;
            _rigidbody = rigidbody;
        }

        public void Reset()
        {
            _velocity = Vector3.zero;
            _steer = Vector3.zero;
            _drive = Vector3.zero;
            _hasSteer = false;
            _hasDrive = false;
        }

        public void Steer(Vector3 desiredVelocity)
        {
            _steer = desiredVelocity;
            _hasSteer = true;
        }

        public void Drive(Vector3 velocity)
        {
            _drive = velocity;
            _hasDrive = true;
        }

        public void Tick(float dt)
        {
            var position = _rigidbody.position;

            if (_hasDrive)
            {
                _velocity = _drive; // exact, scripted motion
            }
            else if (_hasSteer)
            {
                var target = _steer + Separation(position) + WallAvoidance(position, _steer);
                target = Vector3.ClampMagnitude(target, _owner.MaxSpeed);
                _velocity = Vector3.MoveTowards(_velocity, target, _owner.Acceleration * dt);
            }
            else
            {
                _velocity = Vector3.MoveTowards(_velocity, Vector3.zero, _owner.BrakeAcceleration * dt);
            }

            _velocity.y = 0f;
            var next = ClampToNavMesh(position, position + _velocity * dt, dt);
            _rigidbody.MovePosition(next);

            _hasSteer = false;
            _hasDrive = false;
        }

        // Steer away from nearby enemies (uses the director's registry, no physics query).
        private Vector3 Separation(Vector3 position)
        {
            if (!EnemyDirector.HasInstance) return Vector3.zero;

            var neighbours = EnemyDirector.Instance.Dancers;
            var radius = _owner.SeparationRadius;
            var sum = Vector3.zero;
            var count = 0;

            for (int i = 0; i < neighbours.Count; i++)
            {
                var other = neighbours[i];
                if (other == null || other == _owner) continue;

                var away = Utils.FlatDirection(position, other.transform.position);
                var distance = away.magnitude;
                if (distance > 0.0001f && distance < radius)
                {
                    sum += away / distance * (1f - distance / radius); // closer => stronger
                    count++;
                }
            }

            if (count == 0) return Vector3.zero;
            return sum / count * (_owner.SeparationWeight * _owner.MaxSpeed);
        }

        // Predictive nudge away from walls so the path bends smoothly before reaching an edge.
        private Vector3 WallAvoidance(Vector3 position, Vector3 intendedVelocity)
        {
            var speed = intendedVelocity.magnitude;
            if (speed < 0.0001f) return Vector3.zero;

            var direction = intendedVelocity / speed;
            var ahead = position + direction * _owner.WallLookAhead;

            if (!NavMesh.Raycast(position, ahead, out var hit, NavMesh.AllAreas)) return Vector3.zero;

            var distance = Utils.FlatDirection(hit.position, position).magnitude;
            var strength = 1f - Mathf.Clamp01(distance / _owner.WallLookAhead);
            var normal = hit.normal;
            normal.y = 0f;
            return normal.normalized * (_owner.MaxSpeed * _owner.WallAvoidWeight * strength);
        }

        // Hard safety: never cross a navmesh edge. Slide along it. Movement is planar
        // (the arena is flat) so we keep the body's height fixed.
        private Vector3 ClampToNavMesh(Vector3 from, Vector3 to, float dt)
        {
            if (NavMesh.Raycast(from, to, out var hit, NavMesh.AllAreas))
            {
                var normal = hit.normal;
                normal.y = 0f;
                normal = normal.normalized;

                _velocity -= Vector3.Project(_velocity, normal); // slide along the wall
                _velocity.y = 0f;
                to = from + _velocity * dt;

                if (NavMesh.Raycast(from, to, out hit, NavMesh.AllAreas))
                    to = hit.position; // corner case: stop at the edge
            }

            to.y = from.y;
            return to;
        }
    }
}
