using System;
using PlayerComponents;
using UnityEngine;

namespace Enemies.BlobComponents
{
    public class Blob : Enemy
    {
        [SerializeField] private LayerMask playerLayerMask;
        [SerializeField] private float moveRate = 1.5f;

        [Header("Attack pattern")]

        [Range(1f, 360f)]
        [SerializeField] private float shotAngle = 180f;

        [SerializeField] private float detectionRange = 10f;
        public float MoveRate => moveRate;
        public LayerMask PlayerLayerMask => playerLayerMask;
        public float DetectionRange => detectionRange;


        public override void Parry(Player player)
        {
        }
    }
}