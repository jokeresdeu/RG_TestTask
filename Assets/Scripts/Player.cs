using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    private const int MinJoints = 4;

    [SerializeField] private Transform _playerBody;
    
    [SerializeField] private float _normalSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _acceleration;

    [SerializeField] private float _minDeltaY;
    [SerializeField] private float _minDeltaX;
    [SerializeField] private PlayerPartPool _playerPartsPool;

    private bool _moveStarted;
    
    private List<Transform> _activeParts = new List<Transform>();
    
    private Quaternion _playerStartRot;

    private float _currentSpeed;
    private PlayerState _currentState;
    private Vector2 _prevMousePosition;
    private Collider[] _bodyParts;
    private Rigidbody _rigidbody;

    public Dictionary<Dirrection, List<Joint>> Joints { get; private set; }
    private Vector2 _delta;
    public event Action PlayerMoved = delegate { };
    private bool _canBeRotated = true;
    private float _lastRotationTime;
    private void Update()
    {
#if UNITY_EDITOR
        if (EventSystem.current.IsPointerOverGameObject())
            return;
#elif UNITY_ANDROID
        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            return;
#endif

        if (Input.GetButtonDown("Fire1"))
        {
            if (!_moveStarted)
            {
                PlayerMoved();
                _moveStarted = true;
            }
            _delta = Vector2.zero;
            _prevMousePosition = Input.mousePosition;
        }

        if (Input.GetButton("Fire1"))
        {
            if (_delta != Vector2.zero)
                return;

            _delta = _prevMousePosition - (Vector2)Input.mousePosition;
            _prevMousePosition = Input.mousePosition;
        }
    }

    public void FixedUpdate()
    {
        if (!_moveStarted)
            return;

        if (_delta.y < -_minDeltaY && _currentState == PlayerState.Normal)
        {
            _acceleration *= 2;
            _currentSpeed = _maxSpeed;
            _rigidbody.velocity = Vector3.forward * _maxSpeed;
            _currentState = PlayerState.Accelerated;
        }
        else if(_delta.y > _minDeltaY && _currentState == PlayerState.Accelerated)
        {
            _rigidbody.velocity = Vector3.forward * _normalSpeed;
            _currentSpeed = _normalSpeed;
            _currentState = PlayerState.Normal;
        }

        if (Mathf.Abs(_delta.x) > _minDeltaX && Time.time - _lastRotationTime > 0.1f)
        {
            _lastRotationTime = Time.time;
            float rotation = _delta.x > 0 ? 90 : -90;
            _playerBody.Rotate(0, rotation, 0);
            _canBeRotated = false;
        }

        _delta = Vector2.zero;

        if (_rigidbody.velocity.z < _currentSpeed )
            _rigidbody.AddForce(Vector3.forward * _acceleration, ForceMode.Acceleration);
    }

    public void Init()
    {
        _bodyParts = _playerBody.GetComponentsInChildren<Collider>();
        _currentState = PlayerState.Normal;
        _currentSpeed = _normalSpeed;
        _playerStartRot = _playerBody.localRotation;
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void StopPlayer()
    {
        _moveStarted = false;
        _rigidbody.velocity = Vector3.zero;
    }

    public void ResetPlayer()
    {
        _playerBody.localRotation = _playerStartRot;
        ClearParts();
        Joints = GeneratePlayerJoints();
        AddJoints();
    }

    public void OnBlockCrossed(bool successfully)
    {
        if (!successfully)
        {
            _rigidbody.velocity = Vector3.forward * _normalSpeed/2;
            _currentSpeed = _normalSpeed;
            _currentState = PlayerState.Normal;
        }
    }

    private void ClearParts()
    {
        _playerPartsPool.ReturnToPool(_activeParts);
    }

    private void AddJoints()
    {
        foreach (KeyValuePair<Dirrection, List<Joint>> item in Joints)
        {
            Vector2Int placeDirection = Helper.GetDirrectionForPlacing(item.Key);
            for (int i = 0; i < item.Value.Count; i++)
            {
                for (int j = 1; j <= item.Value[i].Length; j++)
                {
                    Transform part = _playerPartsPool.TakeFromPool();
                    part.position = _bodyParts[item.Value[i].Position].transform.position + new Vector3(placeDirection.x, 0, placeDirection.y) * j;
                    part.parent = _playerBody;
                    _activeParts.Add(part);
                }
            }
        }
    }

    private Dictionary<Dirrection, List<Joint>> GeneratePlayerJoints()
    {
        Dictionary<Dirrection, List<Joint>> newJoints = new Dictionary<Dirrection, List<Joint>>();
        int hardness = Mathf.Clamp(ProjectPrefs.CurrentLvl.GetValue()/10, 1, 3);
        int jointsCount = Random.Range(MinJoints, MinJoints * hardness);
        int additionalJoints = jointsCount % 4;
        int constJointPerSide = jointsCount / 4;
        for (int i = 0; i < 4; i++)
        {
            int additionalJointPerSide = Random.Range(0, additionalJoints);
            additionalJoints -= additionalJointPerSide;
            int jointPerSide = constJointPerSide + additionalJointPerSide;
            List<Joint> joints = new List<Joint>();
            List<int> availablePositions = GetAwailablePositions();
            for (int j = 0; j < jointPerSide; j++)
            {
                int position = availablePositions[Random.Range(0, availablePositions.Count - 1)];
                availablePositions.Remove(position);
                int length = Random.Range(1, 3);
                joints.Add(new Joint(position, length));
            }
            newJoints.Add((Dirrection)i, joints);
        }
        return newJoints;
    }

    private List<int> GetAwailablePositions()
    {
        List<int> positions = new List<int>();
        for(int i = 0; i < _bodyParts.Length; i++)
        {
            positions.Add(i);
        }
        return positions;
    }
}


