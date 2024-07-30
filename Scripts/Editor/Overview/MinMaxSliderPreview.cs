﻿using System.Numerics;
using UnityEngine.UIElements;
using Z3.UIBuilder.Core;

namespace Z3.UIBuilder.Editor
{
    public class MinMaxSliderPreview
    {
        [MinMaxSlider(0, 100)]
        public Vector2 minMaxSlider;

        public Vector2 minMaxSlider2;
    }
}