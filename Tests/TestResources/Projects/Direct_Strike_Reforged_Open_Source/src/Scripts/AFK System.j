globals
    integer array currentGold
    boolean array isAFK
    timer array afkTimer
    trigger trigAFKCheck

    integer AFK_GOLD_THRESHOLD = 1200

    framehandle frameParentAFK
    framehandle frameAFKSplat
    framehandle frameParentAFKText
    framehandle btnNotAfk
endglobals

library LibAFKSystem initializer init requires LibRequired
    
    private function CheckAFK takes nothing returns nothing
        local integer p = 1

        loop
            exitwhen p > 6
            set currentGold[p] = GetPlayerState(Player(p - 1), PLAYER_STATE_RESOURCE_GOLD)

            if(currentGold[p] > AFK_GOLD_THRESHOLD and isAFK[p] == false) then
                set isAFK[p] = true
                call StartTimerBJ(afkTimer[p], false, 30)
                
                if(GetLocalPlayer() == Player(p - 1)) then
                    call BlzFrameSetVisible(frameParentAFK, true)
                endif
            endif
            //if(currentGold[p] > 1600) then
            //    call CustomDefeatBJ(Player(p - 1), "You were AFK")
            //    call DisplayTextToForce(GetPlayersAll(), GetShortPlayerName(Player(p - 1)) + " was AFK")
            //endif

            set p = p + 1
        endloop
    endfunction

    private function BtnNotAFK_OnClick takes nothing returns nothing
        local integer p = GetConvertedPlayerId(GetTriggerPlayer())

        set isAFK[p] = false
        call PauseTimer(afkTimer[p])        

        if(GetLocalPlayer() == GetTriggerPlayer()) then
            call BlzFrameSetVisible(frameParentAFK, false)
        endif
    endfunction

    function KickPlayerFromGame takes nothing returns nothing
        local integer p = 1

        loop
            exitwhen p > 6
            
            if(GetExpiredTimer() == afkTimer[p]) then
                call CustomDefeatBJ(Player(p - 1), "You were AFK")
                set udg_EventPlayerLeave = Player(p - 1)
                set udg_PlayerLeavesEventReal = 1.00
                call DisplayTextToForce(GetPlayersAll(), pColor[p] + GetShortPlayerName(Player(p - 1)) + "|r" + " was AFK")
            endif

            set p = p + 1
        endloop

    endfunction

    private function init takes nothing returns nothing
        local trigger trig
        local integer p = 1

        if(bj_isSinglePlayer == true) then
            return
        endif
            

        // AFK check
        set trigAFKCheck = CreateTrigger()
        call TriggerRegisterTimerEventPeriodic(trigAFKCheck, 30)
        call TriggerAddAction(trigAFKCheck, function CheckAFK)

        
        set frameParentAFK = BlzCreateFrameByType("TEXT", "", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
        call BlzFrameSetAbsPoint(frameParentAFK, FRAMEPOINT_CENTER, 0.4, 0.3)
        call BlzFrameSetLevel(frameParentAFK, 10)
        call BlzFrameSetVisible(frameParentAFK, false)

        set frameAFKSplat = BlzCreateFrameByType("BACKDROP", "", frameParentAFK, "", 0)
        call BlzFrameSetSize(frameAFKSplat, 0.3, 0.3)
        call BlzFrameSetAbsPoint(frameAFKSplat, FRAMEPOINT_CENTER, 0.41, 0.31)
        call BlzFrameSetTexture(frameAFKSplat, "war3mapImported\\splat.dds", 0, true)

        set frameParentAFKText = BlzCreateFrameByType("TEXT", "", frameParentAFK, "", 0)
        call BlzFrameSetAbsPoint(frameParentAFKText, FRAMEPOINT_CENTER, 0.4, 0.33)
        call BlzFrameSetText(frameParentAFKText, "|cffff3714AFK WARNING|r")
        call BlzFrameSetScale(frameParentAFKText, 1.5)

        set btnNotAfk = BlzCreateFrame("ScriptDialogButton", frameParentAFK, 0, 0)
        call BlzFrameSetSize(btnNotAfk, 0.15, 0.05)
        call BlzFrameSetAbsPoint(btnNotAfk, FRAMEPOINT_CENTER, 0.4, 0.3)
        call BlzFrameSetText(btnNotAfk, "I'm not AFK")

        // Not AFK button click
        set trig = CreateTrigger()
        call BlzTriggerRegisterFrameEvent(trig, btnNotAfk, FRAMEEVENT_CONTROL_CLICK)
        call TriggerAddAction(trig, function BtnNotAFK_OnClick)

        // AFK timer init
        loop
            exitwhen p > 6
            set afkTimer[p] = CreateTimer()
            set trig = CreateTrigger()
            call TriggerRegisterTimerExpireEvent(trig, afkTimer[p])
            call TriggerAddAction(trig, function KickPlayerFromGame)
            
            set p = p + 1
        endloop

    endfunction

endlibrary
