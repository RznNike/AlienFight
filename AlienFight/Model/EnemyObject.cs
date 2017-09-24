﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlienFight.Model
{
    public class EnemyObject : GameObject
    {
        private bool _isMoving;
        private int _leftWalkingBound;
        private int _rightWalkingBound;

        public bool IsMoving { get => _isMoving; set => _isMoving = value; }
        public int LeftWalkingBound { get => _leftWalkingBound; set => _leftWalkingBound = value; }
        public int RightWalkingBound { get => _rightWalkingBound; set => _rightWalkingBound = value; }
    }
}