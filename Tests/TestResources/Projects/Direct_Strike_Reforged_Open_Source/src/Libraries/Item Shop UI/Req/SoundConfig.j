library RaceSounds initializer init_function
globals
    sound array SoundNoGold
    sound array SoundNoLumber
endglobals
private function init_function takes nothing returns nothing
    local integer index 
    set index = GetHandleId(RACE_HUMAN)
    set SoundNoGold[index] = CreateSound("Sound\\Interface\\Warning\\Human\\KnightNoGold1.wav", false, false, false, 10, 10, "")
    call SetSoundParamsFromLabel(SoundNoGold[index], "NoGoldHuman")
    call SetSoundDuration(SoundNoGold[index], 1618)
    set SoundNoLumber[index] = CreateSound("Sound\\Interface\\Warning\\Human\\KnightNoLumber1.wav", false, false, false, 10, 10, "")
    call SetSoundParamsFromLabel(SoundNoLumber[index], "NoLumberHuman")
    call SetSoundDuration(SoundNoLumber[index], 1903)

    set index = GetHandleId(ConvertRace(11))
    set SoundNoGold[index] = CreateSound("Sound\\Interface\\Warning\\Naga\\NagaNoGold1.wav", false, false, false, 10, 10, "")
    call SetSoundParamsFromLabel(SoundNoGold[index], "NoGoldNaga")
    call SetSoundDuration(SoundNoGold[index], 2690)
    set SoundNoLumber[index] = CreateSound("Sound\\Interface\\Warning\\Naga\\NagaNoLumber1.wav", false, false, false, 10, 10, "")
    call SetSoundParamsFromLabel(SoundNoLumber[index], "NoLumberNaga")
    call SetSoundDuration(SoundNoLumber[index], 2011)

    set index = GetHandleId(RACE_ORC)
    set SoundNoGold[index] = CreateSound("Sound\\Interface\\Warning\\Orc\\GruntNoGold1.wav", false, false, false, 10, 10, "")
    call SetSoundParamsFromLabel(SoundNoGold[index], "NoGoldOrc")
    call SetSoundDuration(SoundNoGold[index], 1450)
    set SoundNoLumber[index] = CreateSound("Sound\\Interface\\Warning\\Orc\\GruntNoLumber1.wav", false, false, false, 10, 10, "")
    call SetSoundParamsFromLabel(SoundNoLumber[index], "NoLumberOrc")
    call SetSoundDuration(SoundNoLumber[index], 1219)

    set index = GetHandleId(RACE_NIGHTELF)
    set SoundNoGold[index] = CreateSound("Sound\\Interface\\Warning\\NightElf\\SentinelNoGold1.wav", false, false, false, 10, 10, "")
    call SetSoundParamsFromLabel(SoundNoGold[index], "NoGoldNightElf")
    call SetSoundDuration(SoundNoGold[index], 1229)
    set SoundNoLumber[index] = CreateSound("Sound\\Interface\\Warning\\NightElf\\SentinelNoLumber1.wav", false, false, false, 10, 10, "")
    call SetSoundParamsFromLabel(SoundNoLumber[index], "NoLumberNightElf")
    call SetSoundDuration(SoundNoLumber[index], 1454)

    set index = GetHandleId(RACE_UNDEAD)
    set SoundNoGold[index] = CreateSound("Sound\\Interface\\Warning\\Undead\\NecromancerNoGold1.wav", false, false, false, 10, 10, "")
    call SetSoundParamsFromLabel(SoundNoGold[index], "NoGoldUndead")
    call SetSoundDuration(SoundNoGold[index], 2005)
    set SoundNoLumber[index] = CreateSound("Sound\\Interface\\Warning\\Undead\\NecromancerNoLumber1.wav", false, false, false, 10, 10, "")
    call SetSoundParamsFromLabel(SoundNoLumber[index], "NoLumberUndead")
    call SetSoundDuration(SoundNoLumber[index], 2005)
endfunction
endlibrary
