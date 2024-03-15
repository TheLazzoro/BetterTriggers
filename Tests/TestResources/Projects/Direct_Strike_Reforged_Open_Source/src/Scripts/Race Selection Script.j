globals
    framehandle frameClassicSelection
    framehandle frameSelectRace
    framehandle array frameRaceImage
    framehandle array frameRaceBanner
    framehandle array btnRaceClassic
    framehandle frameTimeRemainingRace
    framehandle array frameClassicTimeRemaining

    boolean array isRaceSelected
    framehandle array framePlayerHasSelectedClassic
    integer timeRemainingRace

    string array raceImg
    string array raceImgBanner
    string array raceName
endglobals

function RaceSelectClassic takes nothing returns nothing
    local integer p = GetConvertedPlayerId(GetTriggerPlayer())
    local string statusColor = "|cff03ff10"
    local boolean start
    local integer o

    // Sets the race the player selected
    local integer i = 1
    local integer endIndex = 5
    loop
        exitwhen i > endIndex
        if (BlzGetTriggerFrame() == btnRaceClassic[i]) then
            if (i != 5) then
                set udg_PlayerRace[p] = i
            else
                set udg_PlayerRace[p] = 0
            endif
            set isRaceSelected[p] = true
        endif

        set o = 1
        loop
            exitwhen o > 6
            if(IsPlayerEnemy(GetTriggerPlayer(), Player(o - 1)) == false) then
                if(GetLocalPlayer() == Player(o - 1)) then
                    call BlzFrameSetText(framePlayerHasSelectedClassic[p], pColor[p] + GetShortPlayerName(Player(p - 1)) + "|r" + ": " + statusColor + "Ready" + "|r (" + raceNameWithColor[udg_PlayerRace[p]] + ")")
                endif
            else
                if(GetLocalPlayer() == Player(o - 1)) then
                    call BlzFrameSetText(framePlayerHasSelectedClassic[p], pColor[p] + GetShortPlayerName(Player(p - 1)) + "|r" + ": " + statusColor + "Ready" + "|r")
                endif
            endif
            
            set o = o + 1
        endloop
        
        if (GetLocalPlayer() == GetTriggerPlayer()) then
            call BlzFrameSetEnable(btnRaceClassic[i], false)
        endif
        set i = i + 1
    endloop

    // Checks if all players have selected a race
    set start = true
    set i = 1
    set endIndex = 6
    loop
        exitwhen i > endIndex
        if (udg_ActivePlayers[i] == true and isRaceSelected[i] == false) then
            set start = false
        endif
        set i = i + 1
    endloop

    if (start == true) then
        set timeRemainingRace = 0
    endif
endfunction

function RaceSelectionTimer takes nothing returns nothing
    local integer i
    local integer endIndex
    local string statusColor
    local string timeStr
    local integer endIndex1
    local integer num
    local integer p

    if (timeRemainingRace > 0) then
        set timeRemainingRace = timeRemainingRace - 1
        call StartTimerBJ(udg_RaceSelectionTimer, false, 1)
    else
        // Resets the Race selection UI in case people vote for another round.
        set i = 1
        set endIndex = 5
        loop
            exitwhen i > endIndex
            call BlzFrameSetEnable(btnRaceClassic[i], true)
            set i = i + 1
        endloop

        set statusColor = "|cfffffb03"
        set p = 1
        set endIndex = 6
        loop
            exitwhen p > endIndex
            set isRaceSelected[p] = false
            if (udg_ActivePlayers[p] == true) then
                call BlzFrameSetText(framePlayerHasSelectedClassic[p], pColor[p] + GetShortPlayerName(Player(p - 1)) + "|r" + ": " + statusColor + "Selecting..." + "|r")
            else
                call BlzFrameSetText(framePlayerHasSelectedClassic[p], "<Empty>")
            endif
            set p = p + 1
        endloop

        // Actual game setup
        call BlzFrameSetVisible(frameClassicSelection, false)
        call PlaySoundBJ(gg_snd_QuestActivateWhat1)
        call TriggerExecuteBJ(gg_trg_Start_Game, true)
    endif

    // Update Timer UI
    set timeStr = I2S(timeRemainingRace)
    set i = 1
    set endIndex1 = StringLength(timeStr)
    loop
        exitwhen i > endIndex1
        set num = S2I(SubStringBJ(timeStr, i, i))
        if (timeRemainingRace < 10) then
            call BlzFrameSetVisible(frameClassicTimeRemaining[2], false)
            call BlzFrameSetTexture(frameClassicTimeRemaining[i], textureNumber[num], 0, true)
        else
            call BlzFrameSetVisible(frameClassicTimeRemaining[2], true)
            call BlzFrameSetTexture(frameClassicTimeRemaining[i], textureNumber[num], 0, true)
        endif
        set i = i + 1
    endloop
endfunction

function SetupClassicSelection takes nothing returns nothing
    local integer i
    local integer endIndex
    local real Xpos
    local trigger trig
    local string statusColor
    local integer incr
    local real YPos
    local trigger trig2
    
    set raceImg[1] = "war3mapImported\\Warcraft_III_Reforged_-_Humans_Icon.dds"
    set raceImg[2] = "war3mapImported\\Warcraft_III_Reforged_-_Orcs_Icon.dds"
    set raceImg[3] = "war3mapImported\\Warcraft_III_Reforged_-_Undead_Icon.dds"
    set raceImg[4] = "war3mapImported\\Warcraft_III_Reforged_-_Night_Elves_Icon.dds"
    set raceImg[5] = "war3mapImported\\Random.tga"

    set raceImgBanner[1] = "war3mapImported\\HumanBanner.dds"
    set raceImgBanner[2] = "war3mapImported\\OrcBanner.dds"
    set raceImgBanner[3] = "war3mapImported\\UndeadBanner.dds"
    set raceImgBanner[4] = "war3mapImported\\NightElfBanner.dds"
    set raceImgBanner[5] = "war3mapImported\\RandomBanner.dds" // import this

    set raceName[1] = "Human"
    set raceName[2] = "Orc"
    set raceName[3] = "Undead"
    set raceName[4] = "Night Elf"
    set raceName[5] = "Random"

    set textureNumber[0] = "war3mapImported\\0.dds"
    set textureNumber[1] = "war3mapImported\\1.dds"
    set textureNumber[2] = "war3mapImported\\2.dds"
    set textureNumber[3] = "war3mapImported\\3.dds"
    set textureNumber[4] = "war3mapImported\\4.dds"
    set textureNumber[5] = "war3mapImported\\5.dds"
    set textureNumber[6] = "war3mapImported\\6.dds"
    set textureNumber[7] = "war3mapImported\\7.dds"
    set textureNumber[8] = "war3mapImported\\8.dds"
    set textureNumber[9] = "war3mapImported\\9.dds"

    set frameClassicSelection = BlzCreateFrame("EscMenuBackdrop", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
    call BlzFrameSetSize(frameClassicSelection, 0.65, 0.3)
    call BlzFrameSetAbsPoint(frameClassicSelection, FRAMEPOINT_CENTER, 0.4, 0.38)
    call BlzFrameSetVisible(frameClassicSelection, false)

    set frameSelectRace = BlzCreateFrameByType("BACKDROP", "", frameClassicSelection, "", 0)
    call BlzFrameSetSize(frameSelectRace, 0.3, 0.07)
    call BlzFrameSetAbsPoint(frameSelectRace, FRAMEPOINT_CENTER, 0.4, 0.49)
    call BlzFrameSetTexture(frameSelectRace, "war3mapImported\\Select_Your_Race.dds", 0, true)

    set i = 1
    set endIndex = 5
    loop
        exitwhen i > endIndex

        set Xpos = (i - 1) * 0.12

        // Banner
        set frameRaceBanner[i] = BlzCreateFrameByType("BACKDROP", "", frameClassicSelection, "", 0)
        call BlzFrameSetAbsPoint(frameRaceBanner[i], FRAMEPOINT_CENTER, 0.16 + Xpos, 0.401)
        call BlzFrameSetSize(frameRaceBanner[i], 0.133, 0.133)
        call BlzFrameSetTexture(frameRaceBanner[i], raceImgBanner[i], 0, true)

        // Coat of Arms
        set frameRaceImage[i] = BlzCreateFrameByType("BACKDROP", "", frameClassicSelection, "", 0)
        call BlzFrameSetAbsPoint(frameRaceImage[i], FRAMEPOINT_CENTER, 0.16 + Xpos, 0.401)
        call BlzFrameSetSize(frameRaceImage[i], 0.133, 0.133)
        call BlzFrameSetTexture(frameRaceImage[i], raceImg[i], 0, true)

        set btnRaceClassic[i] = BlzCreateFrame("ScriptDialogButton", frameClassicSelection, 0, 0)
        call BlzFrameSetAbsPoint(btnRaceClassic[i], FRAMEPOINT_CENTER, 0.16 + Xpos, 0.33)
        call BlzFrameSetSize(btnRaceClassic[i], 0.1, 0.025)
        call BlzFrameSetText(btnRaceClassic[i], raceName[i])
        
        set i = i + 1
    endloop

    set frameTimeRemainingRace = BlzCreateFrameByType("BACKDROP", "", frameClassicSelection, "", 0)
    call BlzFrameSetSize(frameTimeRemainingRace, 0.06, 0.045)
    call BlzFrameSetAbsPoint(frameTimeRemainingRace, FRAMEPOINT_LEFT, 0.1, 0.275)
    call BlzFrameSetTexture(frameTimeRemainingRace, "war3mapImported\\Time_Remaining.dds", 0, true)

    set i = 1
    set endIndex = 2
    loop
        exitwhen i > endIndex

        set frameClassicTimeRemaining[i] = BlzCreateFrameByType("BACKDROP", "", frameClassicSelection, "", 0)
        call BlzFrameSetSize(frameClassicTimeRemaining[i], 0.03, 0.02)
        if (i == 1) then
            call BlzFrameSetAbsPoint(frameClassicTimeRemaining[i], FRAMEPOINT_LEFT, 0.165, 0.275)
            call BlzFrameSetTexture(frameClassicTimeRemaining[i], "war3mapImported\\3.tga", 0, true)
        else
            call BlzFrameSetAbsPoint(frameClassicTimeRemaining[i], FRAMEPOINT_LEFT, 0.183, 0.275)
            call BlzFrameSetTexture(frameClassicTimeRemaining[i], "war3mapImported\\0.tga", 0, true)
        endif
        set i = i + 1
    endloop

    //
    // BUTTON CALLBACKS
    //

    set i = 1
    set endIndex = 5
    loop
        exitwhen i > endIndex
        
        set trig = CreateTrigger()
        call BlzTriggerRegisterFrameEvent(trig, btnRaceClassic[i], FRAMEEVENT_CONTROL_CLICK)
        call TriggerAddAction(trig, function RaceSelectClassic)

        set i = i + 1
    endloop

    //
    // OTHER FUNCTIONS
    //

    set statusColor = "|cfffffb03"
    set incr = 1
    set YPos = 0.33 - incr * 0.017

    set i = 1
    set endIndex = 6
    loop
        exitwhen i > endIndex

        set isRaceSelected[i] = false
        //set framePlayerHasSelectedClassic[i] = BlzCreateFrameByType("TEXT", "gameModeText", frameClassicSelection, "EscMenuButtonTextTemplate", 0)
        set framePlayerHasSelectedClassic[i] = BlzCreateFrame("TasButtonTextTemplate", frameClassicSelection, 0, 0)
        if(ModuloInteger(i, 2) == 1) then
            call BlzFrameSetAbsPoint(framePlayerHasSelectedClassic[i], FRAMEPOINT_TOPLEFT, 0.3, YPos)
        else
            call BlzFrameSetAbsPoint(framePlayerHasSelectedClassic[i], FRAMEPOINT_TOPLEFT, 0.5, YPos)
            set incr = incr + 1
            set YPos = 0.33 - incr * 0.017
        endif
        if (udg_ActivePlayers[i] == true) then
            call BlzFrameSetText(framePlayerHasSelectedClassic[i], pColor[i] + GetShortPlayerName(Player(i - 1)) + "|r" + ": " + statusColor + "Selecting..." + "|r")
        else
            call BlzFrameSetText(framePlayerHasSelectedClassic[i], "<Empty>")
        endif
        set i = i + 1
    endloop

    set trig2 = CreateTrigger()
    call TriggerRegisterTimerExpireEvent(trig2, udg_RaceSelectionTimer)
    call TriggerAddAction(trig2, function RaceSelectionTimer)
endfunction

function InitiateRaceSelection takes nothing returns nothing
    local integer p
    local integer endIndex

    set timeRemainingRace = 30
    call PlaySoundBJ(gg_snd_QuestActivateWhat1)
    call BlzFrameSetVisible(scoreboard, false)
    call BlzFrameSetVisible(frameClassicSelection, true)
    call StartTimerBJ(udg_RaceSelectionTimer, false, 1)

    // Resets the selected race in case people vote for another round.
    set p = 1
    set endIndex = 6
    loop
        exitwhen p > endIndex
        set udg_PlayerRace[p] = 0
        set p = p + 1
    endloop
endfunction

function RaceSelectSetNameEmpty takes nothing returns nothing
    local integer p = GetConvertedPlayerId(GetTriggerPlayer())
    call BlzFrameSetText(framePlayerHasSelectedClassic[p], "<Empty>")
endfunction