﻿using System;
using UnityEngine;

namespace Enemies
{
    [Serializable]
    public struct DirectionWeight
    {
        public Vector3 direction;
        public float weight;

        public DirectionWeight(Vector3 direction, float weight)
        {
            this.direction = direction;
            this.weight = weight;
        }
    }
}