﻿using MajdataPlay.Buffers;
using MajdataPlay.Game.Controllers;
using MajdataPlay.Interfaces;
using MajdataPlay.IO;
using MajdataPlay.Types;
using System;
using UnityEngine;
#nullable enable
namespace MajdataPlay.Game.Notes
{
    public abstract class TapBase : NoteDrop, IDistanceProvider, INoteQueueMember<TapQueueInfo>, IRendererContainer
    {
        public RendererStatus RendererState
        {
            get => _rendererState;
            set 
            {
                if (State < NoteStatus.Initialized)
                    return;

                switch(value)
                {
                    case RendererStatus.Off:
                        thisRenderer.forceRenderingOff = true;
                        exRenderer.forceRenderingOff = true;
                        tapLineRenderer.forceRenderingOff = true;
                        break;
                    case RendererStatus.On:
                        thisRenderer.forceRenderingOff = false;
                        exRenderer.forceRenderingOff = !IsEX;
                        tapLineRenderer.forceRenderingOff = false;
                        break;
                }
            }
        }
        public TapQueueInfo QueueInfo { get; set; } = TapQueueInfo.Default;
        public float Distance { get; protected set; } = -100;
        public GameObject tapLine;

        protected BreakShineController? breakShineController = null;
        protected SpriteRenderer thisRenderer;
        protected SpriteRenderer exRenderer;
        protected SpriteRenderer tapLineRenderer;
        protected NotePoolManager notePoolManager;

        const int _spriteSortOrder = 1;
        const int _exSortOrder = 0;


        public virtual void Initialize(TapPoolingInfo poolingInfo)
        {
            if (State >= NoteStatus.Initialized && State < NoteStatus.Destroyed)
                return;
            StartPos = poolingInfo.StartPos;
            Timing = poolingInfo.Timing;
            _judgeTiming = Timing;
            SortOrder = poolingInfo.NoteSortOrder;
            Speed = poolingInfo.Speed;
            IsEach = poolingInfo.IsEach;
            IsBreak = poolingInfo.IsBreak;
            IsEX = poolingInfo.IsEX;
            QueueInfo = poolingInfo.QueueInfo;
            _isJudged = false;
            Distance = -100;
            if (State == NoteStatus.Start)
                Start();

            thisRenderer.sortingOrder = SortOrder - _spriteSortOrder;
            exRenderer.sortingOrder = SortOrder - _exSortOrder;

            State = NoteStatus.Initialized;
        }
        public virtual void End(bool forceEnd = false)
        {
            State = NoteStatus.Destroyed;
            UnsubscribeEvent();
            if (!_isJudged || forceEnd) 
                return;

            var result = new JudgeResult()
            {
                Result = _judgeResult,
                IsBreak = IsBreak,
                IsEX = IsEX,
                Diff = _judgeDiff
            };
            CanShine = false;
            if (breakShineController is not null)
                breakShineController.enabled = false;
            PlayJudgeSFX(result);
            _effectManager.PlayEffect(StartPos, result);
            _noteManager.NextNote(QueueInfo);
            _objectCounter.ReportResult(this, result);
        }
        protected override void Start()
        {
            if (IsInitialized)
                return;
            base.Start();
            notePoolManager = FindObjectOfType<NotePoolManager>();
            thisRenderer = GetComponent<SpriteRenderer>();
            exRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

            tapLine = Instantiate(tapLine, _noteManager.gameObject.transform.GetChild(7));
            tapLine.SetActive(false);
            tapLineRenderer = tapLine.GetComponent<SpriteRenderer>();

            transform.localScale = new Vector3(0, 0);
        }
        protected override void PlaySFX()
        {
            PlayJudgeSFX(new JudgeResult()
            {
                Result = _judgeResult,
                IsBreak = IsBreak,
                IsEX = IsEX,
                Diff = _judgeDiff
            });
        }
        protected override void PlayJudgeSFX(in JudgeResult judgeResult)
        {
            _audioEffMana.PlayTapSound(judgeResult);
        }
        protected void FixedUpdate()
        {
            if (State < NoteStatus.Running|| IsDestroyed)
                return;
            var timing = GetTimeSpanToJudgeTiming();
            var isTooLate = timing > 0.15f;
            if (!_isJudged && isTooLate)
            {
                _judgeResult = JudgeType.Miss;
                _isJudged = true;
                End();
            }
            else if (_isJudged)
                End();
        }
        // Update is called once per frame
        protected virtual void Update()
        {
            var timing = GetTimeSpanToArriveTiming();
            var distance = timing * Speed + 4.8f;
            var scaleRate = _gameSetting.Debug.NoteAppearRate;
            var destScale = distance * scaleRate + (1 - (scaleRate * 1.225f));

            switch (State)
            {
                case NoteStatus.Initialized:
                    if (destScale >= 0f)
                    {
                        transform.rotation = Quaternion.Euler(0, 0, -22.5f + -45f * (StartPos - 1));
                        tapLine.transform.rotation = Quaternion.Euler(0, 0, -22.5f + -45f * (StartPos - 1));

                        RendererState = RendererStatus.On;
                        CanShine = true;
                        State = NoteStatus.Scaling;
                        goto case NoteStatus.Scaling;
                    }
                    else
                        transform.localScale = new Vector3(0, 0);
                    return;
                case NoteStatus.Scaling:
                    {
                        if (destScale > 0.3f)
                            tapLine.SetActive(true);
                        if (distance < 1.225f)
                        {
                            Distance = distance;
                            transform.localScale = new Vector3(destScale, destScale);
                            transform.position = GetPositionFromDistance(1.225f);
                            var lineScale = Mathf.Abs(1.225f / 4.8f);
                            tapLine.transform.localScale = new Vector3(lineScale, lineScale, 1f);
                        }
                        else
                        {
                            State = NoteStatus.Running;
                            goto case NoteStatus.Running;
                        }
                    }
                    break;
                case NoteStatus.Running:
                    {
                        Distance = distance;
                        transform.position = GetPositionFromDistance(distance);
                        transform.localScale = new Vector3(1f, 1f);
                        var lineScale = Mathf.Abs(distance / 4.8f);
                        tapLine.transform.localScale = new Vector3(lineScale, lineScale, 1f);
                    }
                    break;
                default:
                    return;
            }
        }
        protected override void Check(object sender, InputEventArgs arg)
        {
            if (State < NoteStatus.Running)
                return;
            else if (arg.Type != _sensorPos)
                return;
            else if (_isJudged || !_noteManager.CanJudge(QueueInfo))
                return;

            if (arg.IsClick)
            {
                if (!_ioManager.IsIdle(arg))
                    return;
                else
                    _ioManager.SetBusy(arg);

                Judge(_gpManager.ThisFrameSec);
                //ioManager.SetIdle(arg);
                if (_isJudged)
                    End();
            }
        }
        protected void SubscribeEvent()
        {
            _ioManager.BindArea(Check, _sensorPos);
        }
        protected void UnsubscribeEvent()
        {
            _ioManager.UnbindArea(Check, _sensorPos);
        }
        RendererStatus _rendererState = RendererStatus.Off;
    }
}
