﻿using MajdataPlay.Buffers;

namespace MajdataPlay.Game.Buffers
{
    public interface INotePool<TInfo, TMember> : IObjectPool<IPoolableNote<TInfo, TMember>>
        where TInfo : NotePoolingInfo where TMember : NoteQueueInfo
    {
        public void OnUpdate(float currentSec);
        public void Destroy();
    }
}
