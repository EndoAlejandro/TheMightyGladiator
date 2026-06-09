using System;
using System.Collections;
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
        private Transform _wallsTransform;

        /// <summary>True once the map is fully generated and the NavMesh has been baked.</summary>
        public bool IsReady { get; private set; }

        /// <summary>Fired after the map has finished generating and the NavMesh is baked.
        /// Use this to spawn enemies / the player only once the world is walkable.</summary>
        public event Action OnReady;

        protected override void Awake()
        {
            base.Awake();
            _meshSurface = GetComponent<NavMeshSurface>();
            _tileWorldManager = GetComponent<TileWorldCreatorManager>();
        }

        private void Start()
        {
            StartCoroutine(GenerateAndBakeRoutine());
        }

        private IEnumerator GenerateAndBakeRoutine()
        {
            _tileWorldManager.ResetConfiguration();
            _tileWorldManager.GenerateCompleteMap();

            // GenerateCompleteMap() flags the build layers as executing and then kicks off
            // multi-frame tile-instantiation coroutines, returning immediately (OnMapReady
            // fires before the tiles actually exist). Wait until every layer is done before
            // baking, otherwise the NavMesh is built against an empty / partial scene.
            yield return WaitWhileBuilding();

            BuildNavMesh();

            IsReady = true;
            OnReady?.Invoke();
        }

        /// <summary>
        /// Yields until all TileWorldCreator build layers have finished their
        /// (multi-frame) instantiation coroutines. Use this from your own coroutines
        /// when you trigger a rebuild and need to wait for it to settle.
        /// </summary>
        public IEnumerator WaitWhileBuilding()
        {
            // Build layers set isExecuting = true synchronously, but "place on top" / late
            // layers are only flagged ~0.01s later by Configuration.LateExecution(). Give
            // that a chance to start before we begin polling.
            yield return new WaitForSeconds(0.05f);

            // Then require a few consecutive idle frames so we don't stop in the small gap
            // between the initial pass finishing and the late pass starting.
            const int requiredStableFrames = 3;
            int stableFrames = 0;
            while (stableFrames < requiredStableFrames)
            {
                stableFrames = IsBuilding() ? 0 : stableFrames + 1;
                yield return null;
            }
        }

        /// <summary>Returns true while any build layer is still instantiating tiles.</summary>
        public bool IsBuilding()
        {
            Configuration config = _tileWorldManager.configuration;
            if (config == null)
                return false;

            foreach (BuildLayerFolder folder in config.buildLayerFolders)
            {
                foreach (BuildLayer layer in folder.buildLayers)
                {
                    if (layer != null && layer.isExecuting)
                        return true;
                }
            }

            return false;
        }

        [ContextMenu("Build Navmesh")]
        private void BuildNavMesh()
        {
            _meshSurface ??= GetComponent<NavMeshSurface>();
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
