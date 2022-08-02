﻿// ------------------------------------------------------------------------------
// <copyright file="SoundChannel.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

namespace War3Net.Build.Audio
{
    public enum SoundChannel
    {
        Undefined = -1,

        General = 0,
        UnitSelection = 1,
        UnitAcknowledgement = 2,
        UnitMovement = 3,
        UnitReady = 4,
        Combat = 5,
        Error = 6,
        Music = 7,
        UserInterface = 8,
        LoopingMovement = 9,
        LoopingAmbient = 10,
        Animations = 11,
        Construction = 12,
        Birth = 13,
        Fire = 14,

        // Reforged channels
        // TODO: verify indices are correct
        LegacyMidi = 15,
        CinematicGeneral = 16,
        CinematicAmbient = 17,
        CinematicMusic = 18,
        CinematicDialogue = 19,
        CinematicSfx1 = 20,
        CinematicSfx2 = 21,
        CinematicSfx3 = 22,
    }
}