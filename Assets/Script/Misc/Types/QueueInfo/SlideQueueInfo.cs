﻿using MajdataPlay.Game.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable enable
namespace MajdataPlay.Types
{
    public class SlideQueueInfo : NoteQueueInfo
    {
        public float AppearTiming { get; init; }
        public SlideBase SlideObject { get; init; }
    }
}
