using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using Random = UnityEngine.Random;

public class LvlController : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private FinishLine _finishLinePrefab;
    [SerializeField] private Transform _groundPrefab;

    [SerializeField] private ObstaclePool _obstaclesPool;
    [SerializeField] private ObstaclePartsPool _obstaclePartsPool;
    [SerializeField] private CinemachineVirtualCamera _cam;

    [SerializeField] private float _renderDistance;
    [SerializeField] private float _lvlLengt;
    [SerializeField] private float _minSpawnDistance;
    [SerializeField] private float _maxSpawnDistance;

    private float _lastPlacedObstacleZPos;
    private Camera _mainCamera;
    private Dirrection _lastObstacleDirection;
    private List<Obstacle> _obstaclesOnScene = new List<Obstacle>();
    private FinishLine _finishLine;
    private Transform _ground;
    private Vector3 _camStartPos;
    private int _lvlPoints;
    private int _pointsMultiplier;

    public int PointsMultiplier
    {
        get
        {
            return _pointsMultiplier;
        }
        set
        {
            _pointsMultiplier = value;
            MultiplierChanged(value);
        }
    }


    public int LvlPoints 
    {
        get
        {
            return _lvlPoints;
        }
        set
        {
            _lvlPoints = value;
            LvlPointsChanged(value);
        }
    }

    public bool IsLvlStarted { get; private set; }

    public event Action LvlCompleted = delegate{ };
    public event Action<bool> PlayerCrossedObstacle = delegate { };
    public event Action LvlStarted = delegate { };

    public event Action<int> LvlPointsChanged = delegate { };
    public event Action<int> MultiplierChanged = delegate { };


    private void Awake()
    {
        _finishLine = Instantiate(_finishLinePrefab, transform);
        _player = Instantiate(_player, transform);
        _cam.Follow = _player.transform;
        _ground = Instantiate(_groundPrefab, transform);
       
        _mainCamera = Camera.main;
        _camStartPos = _mainCamera.transform.position;
        _player.Init();
        PrepareLvl();
    }

    public void NextLvl()
    {
        _cam.Follow = null;
        ClearLvl();
        PrepareLvl();
    }

    private void PrepareLvl()
    {
        _ground.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 2);
        _player.transform.position = new Vector3(0, 0, transform.position.z);
        _ground.transform.localScale = new Vector3(1, 1, _lvlLengt / 10 + 2);
        _finishLine.transform.position = new Vector3(0, 0, transform.position.z + _lvlLengt);
        _lastPlacedObstacleZPos = transform.position.z;
        _cam.enabled = false;
        _cam.transform.position = _camStartPos;
        _mainCamera.transform.position = _camStartPos;
        _player.ResetPlayer();
        PlaceObstacles();
        LvlPoints = 0;
        PointsMultiplier = 1;
        _player.PlayerMoved += OnPlayerMoved;
    }

    private void OnPlayerMoved()
    {
        _cam.Follow = _player.transform;
        _cam.enabled = true;
        _finishLine.FinishLineCrossed += OnFinishLineCrossed;
        _player.PlayerMoved -= OnPlayerMoved;
        IsLvlStarted = true;
        LvlStarted();
        StartCoroutine(ChangeObstaclesPlacement());
    }

    private void OnPlayerCrossedObstacle(bool successfully)
    {
        _player.OnBlockCrossed(successfully);
        PlayerCrossedObstacle(successfully);
        
        if(successfully)
        {
            PointsMultiplier++;
            int points = 300 * PointsMultiplier;
            LvlPoints += points;
        }
        else
        {
            PointsMultiplier = 1;
        }
    }

    private void OnFinishLineCrossed()
    {
        _finishLine.FinishLineCrossed -= OnFinishLineCrossed;
        StopCoroutine(ChangeObstaclesPlacement());
        LvlCompleted();
        IsLvlStarted = false;
        _player.StopPlayer();
        ProjectPrefs.CurrentLvl.AddValue(1);
    }

    private IEnumerator ChangeObstaclesPlacement()
    {
        yield return new WaitUntil(() => RemoveObstacles());
        yield return new WaitUntil(() => PlaceObstacles());
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(ChangeObstaclesPlacement());
    }

    private bool RemoveObstacles()
    {
        while (_obstaclesOnScene.Count > 0 && _obstaclesOnScene[0].transform.position.z < _mainCamera.transform.position.z)
        {
            Obstacle obstacle = _obstaclesOnScene[0];
            _obstaclesOnScene.RemoveAt(0);
            obstacle.ObstacleCrossed -= OnPlayerCrossedObstacle;
            _obstaclesPool.ReturnToPool(obstacle);
        }
        return true;
    }

    private bool PlaceObstacles()
    {
        while (_lastPlacedObstacleZPos < _finishLine.transform.position.z - 2 * _minSpawnDistance && _lastPlacedObstacleZPos < _player.transform.position.z + _renderDistance)
        {
            _lastPlacedObstacleZPos += Random.Range(_minSpawnDistance, _maxSpawnDistance);
            if (_lastPlacedObstacleZPos > _finishLine.transform.position.z - _minSpawnDistance)
                _lastPlacedObstacleZPos = _finishLine.transform.position.z - _minSpawnDistance;

            Obstacle obstacle = _obstaclesPool.TakeFromPool();
            obstacle.Init(_obstaclePartsPool);
            obstacle.transform.parent = transform;
            obstacle.transform.position = new Vector3(0, 0, _lastPlacedObstacleZPos);
            Dirrection newDir = _lastObstacleDirection;
            while (newDir == _lastObstacleDirection)
            {
                newDir = (Dirrection)Random.Range(0, 3);
            }
            _lastObstacleDirection = newDir;

            obstacle.ChangeForm(_player, newDir);
            obstacle.ObstacleCrossed += OnPlayerCrossedObstacle;
            _obstaclesOnScene.Add(obstacle);
        }
        return true;
    }

    private void ClearLvl()
    {
        _obstaclesPool.ReturnToPool(_obstaclesOnScene);
    }
}
