﻿using System;

namespace HG.Engine
{
    internal class HgConversion
    {
        public static dynamic DynamicCast(dynamic source, Type dest) => Convert.ChangeType(source, dest);
    }
}
