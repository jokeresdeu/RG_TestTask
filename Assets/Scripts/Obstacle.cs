using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private GameObject _blockContainer;
    [SerializeField] private GameObject _hitEffect;
    
    private SingleBlock[] _blocks;
    private bool _playedBumped;
    private bool _playerCrossed;
    private ObstaclePartsPool _pool;

    public bool _inited;

    public event Action<bool> ObstacleCrossed = delegate { }; 

    private void OnTriggerExit(Collider other)
    {
        if (_playerCrossed)
            return;

        Player player = other.GetComponentInParent<Player>();
        if (player == null)
            return;

        _playerCrossed = true;
        ObstacleCrossed(!_playedBumped);
    }

    public void Init(ObstaclePartsPool pool)
    {
        if (_inited)
            return;
        _pool = pool;
        _blocks = _blockContainer.GetComponentsInChildren<SingleBlock>().OrderBy(b => b.transform.position.y).ThenBy(b => b.transform.position.x).ToArray();
        for(int i = 0; i < _blocks.Length; i++)
        {
            _blocks[i].CollidedWithPlayer += OnPlayerCollidedWithBlock;
            _blocks[i].SetIndex(i);
        }
        _inited = true;
    }

    private void OnPlayerCollidedWithBlock(int i)
    {
        _blocks[i].gameObject.SetActive(false);
        HitEffect hit = _pool.TakeFromPool();
        hit.Init(_pool);
        hit.transform.position = _blocks[i].transform.position;
        hit.Activate();
        _playedBumped = true;
    }

    public void ChangeForm(Player player, Dirrection onRight)
    {
        for(int i = 0; i < _blocks.Length; i++)
        {
            _blocks[i].gameObject.SetActive(true);
        }

        Dirrection onLeft = Helper.GetOppositeDirrection(onRight);

        HideBlocks(player.Joints[onRight], 1);
        HideBlocks(player.Joints[onLeft], -1);

        _playerCrossed = false;
    }

    public void HideBlocks(List<Joint> joints, int dirrection)
    {
        for(int i = 0; i < joints.Count; i++)
        {
            {
                for(int j = 1;  j < joints[i].Length + 1; j++ )
                {
                    float xPos = dirrection * j;
                    SingleBlock block = _blocks.First(b => Vector2.Distance(b.transform.position, new Vector2(xPos, joints[i].Position + 0.5f)) < 0.2f);
                    block.gameObject.SetActive(false);
                }
            }
        }
    }
}
