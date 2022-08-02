﻿// ------------------------------------------------------------------------------
// <copyright file="TriggerItemType.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

namespace War3Net.Build.Script
{
    public enum TriggerItemType
    {
        RootCategory = 1 << 0,
        UNK1 = 1 << 1,
        Category = 1 << 2,
        Gui = 1 << 3,
        Comment = 1 << 4,
        Script = 1 << 5,
        Variable = 1 << 6,
        UNK7 = 1 << 7,
    }
}