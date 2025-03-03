﻿using Cysharp.Threading.Tasks;
using MajdataPlay.Extensions;
using MajdataPlay.Game;
using MajdataPlay.IO;
using MajdataPlay.Timer;
using MajdataPlay.Types;
using MajdataPlay.Utils;
using MajdataPlay.View.Types;
using MajSimai;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
#nullable enable
namespace MajdataPlay.View
{
    internal class ViewManager: MajComponent, INoteController
    {
        public static ViewSummary Summary
        {
            get
            {
                return new()
                {
                    State = _state,
                    ErrMsg = _errMsg,
                    Timeline = _thisFrameSec,
                };
            }
        }
        public float AudioLength { get; private set; } = 0f;
        public bool IsStart { get; private set; }
        public bool IsAutoplay => AutoplayMode != AutoplayMode.Disable;
        public AutoplayMode AutoplayMode { get; private set; } = AutoplayMode.Enable;
        public JudgeGrade AutoplayGrade { get; private set; } = JudgeGrade.Perfect;
        public JudgeStyleType JudgeStyle { get; private set; } = JudgeStyleType.DEFAULT;
        public Material BreakMaterial { get; } = MajEnv.BreakMaterial;
        public Material DefaultMaterial { get; } = MajEnv.DefaultMaterial;
        public Material HoldShineMaterial { get; } = MajEnv.HoldShineMaterial;
        public float ThisFrameSec => _thisFrameSec;
        public float ThisFixedUpdateSec => _thisFrameSec;
        public float AudioTimeNoOffset => _audioTimeNoOffset;


        float _timerStartAt = 0f;

        static ViewStatus _state = ViewStatus.Idle;
        static string _errMsg = string.Empty;
        static float _thisFrameSec = 0f;
        static float _audioTimeNoOffset = 0f;
        static float _offset = 0f;

        readonly SimaiParser SIMAI_PARSER = SimaiParser.Shared;
        readonly string CACHE_PATH = Path.Combine(MajEnv.CachePath, "View");
        string? _videoPath = null;

        //WsServer _httpServer;
        GameSetting _setting = MajInstances.Setting;
        NoteLoader _noteLoader;
        NoteManager _noteManager;
        NoteAudioManager _noteAudioManager;
        NotePoolManager _notePoolManager;
        BGManager _bgManager;

        SimaiChart? _chart;
        Sprite _bgCover = MajEnv.EmptySongCover;
        AudioSampleWrap? _audioSample = null;

        static MajTimer _timer = MajTimeline.CreateTimer();
        
        protected override void Awake()
        {
            base.Awake();
            if(!Directory.Exists(CACHE_PATH))
            {
                Directory.CreateDirectory(CACHE_PATH);
            }
            Majdata<ViewManager>.Instance = this;
            Majdata<INoteController>.Instance = this;
            PlayerSettings.resizableWindow = true;
            //Screen.SetResolution(1920, 1080, false);
        }
        void Start()
        {
            _bgManager = Majdata<BGManager>.Instance!;
            //_httpServer = Majdata<WsServer>.Instance!;
            _noteLoader = Majdata<NoteLoader>.Instance!;
            _noteManager = Majdata<NoteManager>.Instance!;
            _noteAudioManager = Majdata<NoteAudioManager>.Instance!;
            _notePoolManager = Majdata<NotePoolManager>.Instance!;
        }
        void Update()
        {
            switch (_state)
            {
                case ViewStatus.Playing:
                    var elasped = _timer.UnscaledElapsedSecondsAsFloat;
                    _thisFrameSec = elasped - _timerStartAt - _offset;
                    _audioTimeNoOffset = elasped - _timerStartAt;
                    _noteManager.OnUpdate();
                    _notePoolManager.OnUpdate();
                    _noteAudioManager.OnUpdate();
                    break;
            }
        }
        void LateUpdate()
        {
            switch (_state)
            {
                case ViewStatus.Playing:
                    _noteManager.OnLateUpdate();
                    _noteAudioManager.OnLateUpdate();
                    break;
            }
        }
        internal async UniTask<bool> PlayAsync()
        {
            switch(_state)
            {
                case ViewStatus.Ready:
                case ViewStatus.Paused:
                    break;
                default:
                    return false;
            }
            try
            {
                while (_state is ViewStatus.Busy)
                    await UniTask.Yield();
                _state = ViewStatus.Busy;
                await UniTask.SwitchToMainThread();
                _noteManager.InitializeUpdater();
                await UniTask.Yield();
                _timerStartAt = _timer.UnscaledElapsedSecondsAsFloat;
                _state = ViewStatus.Playing;
                _thisFrameSec = (float)_audioSample!.CurrentSec;
                _audioSample!.Play();
                await UniTask.SwitchToThreadPool();
                if (_state == ViewStatus.Paused)
                    return true;
                return true;
            }
            catch (Exception ex)
            {
                MajDebug.LogException(ex);
                _state = ViewStatus.Error;
                throw;
            }
        }
        internal async UniTask<bool> PauseAsync()
        {
            switch (_state)
            {
                case ViewStatus.Playing:
                    break;
                default:
                    return false;
            }
            try
            {
                while (_state is ViewStatus.Busy)
                    await UniTask.Yield();
                _state = ViewStatus.Busy;
                await UniTask.Yield();
                _audioSample!.Pause();
                _thisFrameSec = (float)_audioSample.CurrentSec;
                _state = ViewStatus.Paused;
                return true;
            }
            catch(Exception ex) 
            {
                    MajDebug.LogException(ex);
                    _state = ViewStatus.Error;
                throw;
            }
        }
        internal async UniTask<bool> StopAsync()
        {
            switch (_state)
            {
                case ViewStatus.Playing:
                case ViewStatus.Paused:
                    break;
                default:
                    return false;
            }
            try
            {
                while (_state is ViewStatus.Busy)
                    await UniTask.Yield();
                _state = ViewStatus.Busy;
                await UniTask.Yield();
                _audioSample!.Stop();
                _thisFrameSec = 0;
                return true;
            }
            catch(Exception ex)
            {
                MajDebug.LogException(ex);
                _state = ViewStatus.Error;
                throw;
            }
        }
        internal async UniTask<bool> ResetAsync()
        {
            switch(_state)
            {
                case ViewStatus.Idle:
                    return false;
            }
            try
            {
                while(_state is ViewStatus.Busy)
                    await UniTask.Yield();
                _state = ViewStatus.Busy;
                await UniTask.Yield();
                _thisFrameSec = 0;
                if (_audioSample is not null)
                    _audioSample.Dispose();
                _bgManager.CancelTimeRef();
                await SceneManager.LoadSceneAsync("View");
                return true;
            }
            finally
            {
                _state = ViewStatus.Idle;
            }
        }
        internal async Task LoadAssests(byte[] audioTrack, byte[] bg, byte[]? pv)
        {
            while (_state is ViewStatus.Busy)
                await UniTask.Yield();
            _state = ViewStatus.Busy;
            try
            {
                var audioTrackPath = Path.Combine(CACHE_PATH, "audioTrack.track");
                var videoPath = Path.Combine(CACHE_PATH, "bg.mp4");

                await File.WriteAllBytesAsync(audioTrackPath, audioTrack);

                var sample = await MajInstances.AudioManager.LoadMusicAsync(audioTrackPath);
                var cover = await SpriteLoader.LoadAsync(bg);

                if (pv is null || pv.Length == 0)
                {
                    _videoPath = string.Empty;
                }
                else
                {
                    await File.WriteAllBytesAsync(videoPath, pv);
                    _videoPath = videoPath;
                }
                _audioSample = sample;
                _bgCover = cover;
                _state = ViewStatus.Loaded;
            }
            catch (Exception ex)
            {
                MajDebug.LogException(ex);
                _errMsg = ex.ToString();
                _state = ViewStatus.Error;
                throw;
            }
        }
        internal async Task ParseAndLoadChartAsync(double startAt, string fumen)
        {
            while (_state is ViewStatus.Busy)
                await UniTask.Yield();
            _state = ViewStatus.Busy;
            try
            {
                _chart = await SIMAI_PARSER.ParseChartAsync(string.Empty, string.Empty, fumen);
                var range = new Range<double>(startAt, double.MaxValue);
                _chart.Clamp(range);
                await UniTask.SwitchToMainThread();

                var tapSpeed = Math.Abs(_setting.Game.TapSpeed);

                if (_setting.Game.TapSpeed < 0)
                    _noteLoader.NoteSpeed = -((float)(107.25 / (71.4184491 * Mathf.Pow(tapSpeed + 0.9975f, -0.985558604f))));
                else
                    _noteLoader.NoteSpeed = ((float)(107.25 / (71.4184491 * Mathf.Pow(tapSpeed + 0.9975f, -0.985558604f))));
                _noteLoader.TouchSpeed = _setting.Game.TouchSpeed;

                await _noteLoader.LoadNotesIntoPool(_chart);
                if(_videoPath is null)
                {
                    _bgManager.SetBackgroundPic(_bgCover);
                }
                else
                {
                    _bgManager.SetBackgroundMovie(Path.Combine(CACHE_PATH, "bg.mp4"));
                }
                _audioSample!.CurrentSec = startAt;
                await _noteAudioManager.GenerateAnswerSFX(_chart, false, 0);
                await UniTask.SwitchToThreadPool();
                _state = ViewStatus.Ready;
            }
            catch (Exception ex)
            {
                MajDebug.LogException(ex);
                _errMsg = ex.ToString();
                _state = ViewStatus.Error;
                throw;
            }
        }
        void OnDestroy()
        {
            Majdata<ViewManager>.Free();
            Majdata<INoteController>.Free();
        }
    }
}
