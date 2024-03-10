using System;
using UnityEngine;
using DG.Tweening;
using Tools;

public class CameraMovePeopleRealistic : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbodyPlayer;
    [SerializeField] private AudioSource collisionSteelSound;
    [SerializeField] private AudioSource collisionOtherSound;

    [SerializeField] private Transform head;
    [SerializeField] private Transform body;
    [SerializeField] private float maxAngleBodyTilt;
    [SerializeField] private float stepBodyTilt;
    [SerializeField] private float stepPlayerRotation;
    [SerializeField] private float playerMovementSpeed;

    private Sequence _moveSequence;

    private Vector3 _touchStartPos;
    private Vector3 _touchCurrentPos;
    private Vector3 _forwardPlayer => rigidbodyPlayer.transform.forward;

    private Vector3 _localPositionHead;
    private Quaternion _startRotationBody;

    private bool _isMove;
    private bool _isTurn;

    void Start()
    {
        _localPositionHead = head.transform.localPosition;
        _startRotationBody = body.transform.localRotation;
    }

    void Update()
    {
        if (TouchUtility.TouchCount > 0)
        {
            var touch = TouchUtility.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _touchStartPos = Input.mousePosition;
                    _touchCurrentPos = Input.mousePosition;

                    break;
                case TouchPhase.Moved:
                    if (_touchStartPos == Vector3.zero)
                        _touchStartPos = Input.mousePosition;

                    _touchCurrentPos = Input.mousePosition;

                    if (Mathf.Abs(_touchCurrentPos.y - _touchStartPos.y) > 0.1f)
                    {
                        StartMove();
                    }
                    else
                    {
                        StopMove();
                    }

                    if (Mathf.Abs(_touchCurrentPos.x - _touchStartPos.x) > 0.1f)
                    {
                        _isTurn = true;
                    }
                    else
                    {
                        _isTurn = false;
                    }

                    break;
                case TouchPhase.Ended:
                    StopMove();
                    _isTurn = false;

                    _touchCurrentPos = Vector3.zero;
                    _touchStartPos = Vector3.zero;
                    break;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("SteelMaterial"))
        {
            collisionSteelSound.Play();
        }
        if (collision.gameObject.CompareTag("OtherMaterial"))
        {
            collisionOtherSound.Play();
        }
    }

    private void StartMove()
    {
        if (_isMove) return;

        _moveSequence?.Kill();

        _isMove = true;
        _moveSequence = StartMoveAnimation();
    }

    private void StopMove()
    {
        if (!_isMove) return;

        _isMove = false;
        rigidbodyPlayer.velocity = Vector3.zero;
        _moveSequence?.Kill();
    }

    private Sequence StartMoveAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(head.DOLocalMoveY(_localPositionHead.y-0.13f, 0.35f).SetEase(Ease.OutCubic));

        seq.Append(head.DOLocalMoveY(_localPositionHead.y, 0.35f));
        seq.AppendInterval(0.15f);
         seq.SetLoops(-1);

        return seq;
    }

    private void FixedUpdate()
    {
        if (!_isMove)
        {
            if ((_localPositionHead - head.localPosition).sqrMagnitude > 0.0001f)
            {
                head.localPosition = Vector3.Lerp(head.localPosition, _localPositionHead, 0.3f);
            }
        }
        else
        {
            float delta = (_touchCurrentPos.y - _touchStartPos.y) > 0 ? 1 : -1;
            rigidbodyPlayer.velocity = _forwardPlayer * (playerMovementSpeed * delta);
        }

        if (_isTurn)
        {
            float delta = _touchCurrentPos.x - _touchStartPos.x;
            BodyTilt(delta);
            PlayerRotation(delta);
        }
        else
        {
            body.localRotation = Quaternion.Slerp(body.localRotation, _startRotationBody, 0.3f);
        }
    }

    private void PlayerRotation(float delta)
    {
        float rotationAmount = delta * stepPlayerRotation;

        if (Mathf.Abs(rotationAmount) > 1)
        {
            int factor = delta > 0 ? 1 : -1;
            rotationAmount = 1 * factor;
        }

        transform.Rotate(Vector3.up, rotationAmount);
    }

    private void BodyTilt(float delta)
    {
        float angle = delta * stepBodyTilt;
        var newAngleBody = body.localEulerAngles;
        newAngleBody.z = -angle;

        if (Mathf.Abs(newAngleBody.z) > maxAngleBodyTilt)
        {
            int factor = newAngleBody.z > 0 ? 1 : -1;
            newAngleBody.z = maxAngleBodyTilt * factor;
        }

        body.localRotation = Quaternion.Slerp(body.localRotation, Quaternion.Euler(newAngleBody), 0.3f);
    }
}