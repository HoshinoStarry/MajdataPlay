using MajdataPlay.Buffers;
using MajdataPlay.Game.Buffers;
using MajdataPlay.Game.Types;
using MajdataPlay.Interfaces;
using MajdataPlay.Types;
using MajdataPlay.Utils;
using UnityEngine;
using UnityEngine.U2D;
#nullable enable
namespace MajdataPlay.Game.Notes
{
    internal class EachLineDrop : MajComponent,IPoolableNote<EachLinePoolingInfo,NoteQueueInfo>,IStateful<NoteStatus>, IRendererContainer
    {
        public RendererStatus RendererState
        {
            get => _rendererState;
            set
            {
                if (State < NoteStatus.Initialized)
                    return;
                switch (value)
                {
                    case RendererStatus.Off:
                        sr.forceRenderingOff = true;
                        break;
                    case RendererStatus.On:
                        sr.forceRenderingOff = false;
                        break;
                }
            }
        }
        public IDistanceProvider? DistanceProvider { get; set; }
        public IStatefulNote? NoteA { get; set; }
        public IStatefulNote? NoteB { get; set; }
        public NoteStatus State { get; set; } = NoteStatus.Start;
        public bool IsDestroyed => State == NoteStatus.End;
        public NoteQueueInfo QueueInfo => TapQueueInfo.Default;
        public bool IsInitialized => State >= NoteStatus.Initialized;

        public float timing;
        public int startPosition = 1;
        public int curvLength = 1;
        public float speed = 1;
        public Sprite[] curvSprites;
        private SpriteRenderer sr;
        GameSetting gameSetting = new();
        GamePlayManager _gpManager;
        NotePoolManager poolManager;
        public void Initialize(EachLinePoolingInfo poolingInfo)
        {
            if (State >= NoteStatus.Initialized && State < NoteStatus.End)
                return;
            startPosition = poolingInfo.StartPos;
            timing = poolingInfo.Timing;
            speed = poolingInfo.Speed;
            curvLength = poolingInfo.CurvLength;
            if (State == NoteStatus.Start)
                Start();
            sr.sprite = curvSprites[curvLength - 1];
            transform.localScale = new Vector3(1.225f / 4.8f, 1.225f / 4.8f, 1f);
            transform.rotation = Quaternion.Euler(0, 0, -45f * (startPosition - 1));
            State = NoteStatus.Initialized;
            RendererState = RendererStatus.Off;
            if (DistanceProvider is null)
                MajDebug.LogWarning("DistanceProvider not found");
        }
        public void End(bool forceEnd = false)
        {
            State = NoteStatus.End;
            RendererState = RendererStatus.Off;
            if (forceEnd)
                return;
            NoteA = null;
            NoteB = null;
            DistanceProvider = null;
            poolManager.Collect(this);
        }
        private void Start()
        {
            if (IsInitialized)
                return;
            _gpManager = Majdata<GamePlayManager>.Instance!;
            poolManager = FindObjectOfType<NotePoolManager>();
            gameSetting = MajInstances.Setting;
            sr = gameObject.GetComponent<SpriteRenderer>();
            sr.sprite = curvSprites[curvLength - 1];
            RendererState = RendererStatus.Off;
            Active = true;
        }
        void OnLateUpdate()
        {
            if (State < NoteStatus.Initialized || IsDestroyed)
                return;
            var timing = _gpManager.AudioTime - this.timing;
            var distance = DistanceProvider is not null ? DistanceProvider.Distance : timing * speed + 4.8f;
            var scaleRate = gameSetting.Debug.NoteAppearRate;
            var destScale = distance * scaleRate + (1 - (scaleRate * 1.225f));
            var lineScale = Mathf.Abs(distance / 4.8f);

            switch (State)
            {
                case NoteStatus.Initialized:
                    if (destScale >= 0f)
                    {
                        RendererState = RendererStatus.Off;

                        State = NoteStatus.Scaling;
                        goto case NoteStatus.Scaling;
                    }
                    return;
                case NoteStatus.Scaling:
                    if (destScale > 0.3f)
                        RendererState = RendererStatus.On;
                    if (distance < 1.225f)
                        transform.localScale = new Vector3(1.225f / 4.8f, 1.225f / 4.8f, 1f);
                    else
                    {
                        State = NoteStatus.Running;
                        goto case NoteStatus.Running;
                    }
                    break;
                case NoteStatus.Running:
                    transform.localScale = new Vector3(lineScale, lineScale, 1f);
                    if (NoteA is not null && NoteB is not null)
                    {
                        if (NoteA.State == NoteStatus.End || NoteB.State == NoteStatus.End)
                        {
                            End();
                            return;
                        }
                    }
                    else if (timing > 0)
                    {
                        End();
                        return;
                    }
                    break;
            }
        }
        void OnDestroy()
        {
            Active = false;
        }
        RendererStatus _rendererState = RendererStatus.Off;
    }
}