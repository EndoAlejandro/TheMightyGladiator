using System;
using System.Collections.Generic;
using CustomUtils;
using GiantGrey.TileWorldCreator;
using Unity.AI.Navigation;
using UnityEngine;

namespace PlayerComponents
{
    public class TileWorldController : Singleton<TileWorldController>
    {
        [SerializeField] private string _crystalsLayer;
        [SerializeField] private string _wallsLayer;
        [SerializeField] private string _brokenWallsLayer1;
        [SerializeField] private string _brokenWallsLayer2;
        [SerializeField] private string _brokenWallsLayer3;

        private NavMeshSurface _meshSurface;
        private TileWorldCreatorManager _tileWorldManager;

        protected override void Awake()
        {
            base.Awake();
            _meshSurface = GetComponent<NavMeshSurface>();
            _tileWorldManager = GetComponent<TileWorldCreatorManager>();
        }

        private void Start()
        {
            _tileWorldManager.ResetConfiguration();
            _tileWorldManager.GenerateCompleteMap();
            _meshSurface.BuildNavMesh();
        }

        public void TryToAttackWalls(Vector3 position, float radius)
        {
            // Crystals XP
            var crystals = _tileWorldManager.GetCellPositionsInRadius(_crystalsLayer, position, radius);
            foreach (Vector2 crystal in crystals)
            {
                _tileWorldManager.RemoveCellsFromLayer(_crystalsLayer, new HashSet<Vector2>() { crystal });
                // TODO: Spawn xp
            }

            // TODO: Add fx in each layer damage.
            
            // Walls 3
            var walls3 = _tileWorldManager.GetCellPositionsInRadius(_brokenWallsLayer3, position, radius);
            _tileWorldManager.RemoveCellsFromLayer(_brokenWallsLayer3, walls3);

            // Walls 2
            var walls2 = _tileWorldManager.GetCellPositionsInRadius(_brokenWallsLayer2, position, radius);
            _tileWorldManager.RemoveCellsFromLayer(_brokenWallsLayer2, walls2);
            _tileWorldManager.AddCellsToLayer(_brokenWallsLayer3, walls2);
            
            // Walls 1
            var walls1 = _tileWorldManager.GetCellPositionsInRadius(_brokenWallsLayer1, position, radius);
            _tileWorldManager.RemoveCellsFromLayer(_brokenWallsLayer1, walls1);
            _tileWorldManager.AddCellsToLayer(_brokenWallsLayer2, walls1);
            
            // Walls 0
            var walls = _tileWorldManager.GetCellPositionsInRadius(_wallsLayer, position, radius);
            _tileWorldManager.RemoveCellsFromLayer(_wallsLayer, walls);
            _tileWorldManager.AddCellsToLayer(_brokenWallsLayer1, walls);
            
            _tileWorldManager.ExecuteBuildLayers(ExecutionMode.Normal);
            _meshSurface.BuildNavMesh();
        }
    }
}