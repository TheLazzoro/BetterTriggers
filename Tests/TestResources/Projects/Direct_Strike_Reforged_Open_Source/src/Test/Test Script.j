function SetupTestUI()
    udg_SpawnPlayers[1] = 1
    udg_SpawnPlayers[2] = 2

    testUI = BlzCreateFrame("EscMenuBackdrop", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
    BlzFrameSetSize(testUI, 0.8, 0.35)
    BlzFrameSetAbsPoint(testUI, FRAMEPOINT_CENTER, 0.4, 0.35)
    BlzFrameSetVisible(testUI, true)

    btnTestUIShowHide = BlzCreateFrame("ScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
    BlzFrameSetSize(btnTestUIShowHide, 0.1, 0.05)
    BlzFrameSetAbsPoint(btnTestUIShowHide, FRAMEPOINT_CENTER, 0.75, 0.55)
    BlzFrameSetText(btnTestUIShowHide, "Test UI")

    testUIisON = true

    local trig = CreateTrigger()
    BlzTriggerRegisterFrameEvent(trig, btnTestUIShowHide, FRAMEEVENT_CONTROL_CLICK)
    TriggerAddAction(trig, ShowHideTestUI)

    -----------------------------------------------------------------

    testPlayerSpawn = 1

    btnTestPlayer1 = BlzCreateFrame("ScriptDialogButton", testUI, 0, 0)
    BlzFrameSetSize(btnTestPlayer1, 0.1, 0.03)
    BlzFrameSetAbsPoint(btnTestPlayer1, FRAMEPOINT_CENTER, 0.71, 0.47)
    BlzFrameSetText(btnTestPlayer1, "Player 1")
    BlzFrameSetEnable(btnTestPlayer1, false)

    local trig2 = CreateTrigger()
    BlzTriggerRegisterFrameEvent(trig2, btnTestPlayer1, FRAMEEVENT_CONTROL_CLICK)
    TriggerAddAction(trig2, TestSetPlayer1)

    btnTestPlayer2 = BlzCreateFrame("ScriptDialogButton", testUI, 0, 0)
    BlzFrameSetSize(btnTestPlayer2, 0.1, 0.03)
    BlzFrameSetAbsPoint(btnTestPlayer2, FRAMEPOINT_CENTER, 0.71, 0.44)
    BlzFrameSetText(btnTestPlayer2, "Player 2")

    local trig3 = CreateTrigger()
    BlzTriggerRegisterFrameEvent(trig3, btnTestPlayer2, FRAMEEVENT_CONTROL_CLICK)
    TriggerAddAction(trig3, TestSetPlayer2)

    -----------------------------------------------------------------

    btnUnitAdd = {}
    btnUnitSubtract = {}
    txtUnitSpawn = {}
    for i = 1, 65 do
        local u = CreateUnit(Player(27), tableSpawnUnitTypes[i], 0, 0, 270)

        btnUnitAdd[i] = BlzCreateFrame("ScriptDialogButton", testUI, 0, 0)
        BlzFrameSetSize(btnUnitAdd[i], 0.02, 0.02)
        BlzFrameSetText(btnUnitAdd[i], "+")

        local addTrig = CreateTrigger()
        BlzTriggerRegisterFrameEvent(addTrig, btnUnitAdd[i], FRAMEEVENT_CONTROL_CLICK)
        TriggerAddAction(addTrig, TestBuild)

        btnUnitSubtract[i] = BlzCreateFrame("ScriptDialogButton", testUI, 0, 0)
        BlzFrameSetSize(btnUnitSubtract[i], 0.02, 0.02)
        BlzFrameSetText(btnUnitSubtract[i], "-")

        local subTrig = CreateTrigger()
        BlzTriggerRegisterFrameEvent(subTrig, btnUnitSubtract[i], FRAMEEVENT_CONTROL_CLICK)
        TriggerAddAction(subTrig, TestSell)

        txtUnitSpawn[i] = BlzCreateFrameByType("TEXT", "test", testUI, "EscMenuButtonTextTemplate", 0)
        local name = ""
        for n = 1, 7 do
            name = name .. SubStringBJ(GetUnitName(u), n, n)
        end
        BlzFrameSetText(txtUnitSpawn[i], name)

        if (i <= 13) then
            BlzFrameSetAbsPoint(btnUnitAdd[i], FRAMEPOINT_CENTER, 0.01 + i * 0.05, 0.495)
            BlzFrameSetAbsPoint(txtUnitSpawn[i], FRAMEPOINT_CENTER, 0.01 + i * 0.05, 0.48)
            BlzFrameSetAbsPoint(btnUnitSubtract[i], FRAMEPOINT_CENTER, 0.01 + i * 0.05, 0.465)
        else
            if (i <= 26) then
                BlzFrameSetAbsPoint(btnUnitAdd[i], FRAMEPOINT_CENTER, 0.01 + i * 0.05 - 0.65, 0.435)
                BlzFrameSetAbsPoint(txtUnitSpawn[i], FRAMEPOINT_CENTER, 0.01 + i * 0.05 - 0.65, 0.42)
                BlzFrameSetAbsPoint(btnUnitSubtract[i], FRAMEPOINT_CENTER, 0.01 + i * 0.05 - 0.65, 0.405)
            else
                if (i <= 39) then
                    BlzFrameSetAbsPoint(btnUnitAdd[i], FRAMEPOINT_CENTER, 0.01 + i * 0.05 - 1.3, 0.375)
                    BlzFrameSetAbsPoint(txtUnitSpawn[i], FRAMEPOINT_CENTER, 0.01 + i * 0.05 - 1.3, 0.36)
                    BlzFrameSetAbsPoint(btnUnitSubtract[i], FRAMEPOINT_CENTER, 0.01 + i * 0.05 - 1.3, 0.345)
                else
                    if (i <= 52) then
                        BlzFrameSetAbsPoint(btnUnitAdd[i], FRAMEPOINT_CENTER, 0.01 + i * 0.05 - 1.95, 0.315)
                        BlzFrameSetAbsPoint(txtUnitSpawn[i], FRAMEPOINT_CENTER, 0.01 + i * 0.05 - 1.95, 0.3)
                        BlzFrameSetAbsPoint(btnUnitSubtract[i], FRAMEPOINT_CENTER, 0.01 + i * 0.05 - 1.95, 0.285)
                    else
                        BlzFrameSetAbsPoint(btnUnitAdd[i], FRAMEPOINT_CENTER, 0.01 + i * 0.05 - 2.6, 0.255)
                        BlzFrameSetAbsPoint(txtUnitSpawn[i], FRAMEPOINT_CENTER, 0.01 + i * 0.05 - 2.6, 0.24)
                        BlzFrameSetAbsPoint(btnUnitSubtract[i], FRAMEPOINT_CENTER, 0.01 + i * 0.05 - 2.6, 0.225)
                    end
                end
            end
        end

        RemoveUnit(u)
    end
end

function ShowHideTestUI()
    if (testUIisON == true) then
        BlzFrameSetVisible(testUI, false)
        testUIisON = false
    else
        BlzFrameSetVisible(testUI, true)
        testUIisON = true
    end
end

function TestSetPlayer1()
    testPlayerSpawn = 1
    BlzFrameSetEnable(btnTestPlayer1, false)
    BlzFrameSetEnable(btnTestPlayer2, true)
end

function TestSetPlayer2()
    testPlayerSpawn = 2
    BlzFrameSetEnable(btnTestPlayer1, true)
    BlzFrameSetEnable(btnTestPlayer2, false)
end

function TestBuild()
    local totalGold = GetPlayerState(Player(testPlayerSpawn - 1), PLAYER_STATE_RESOURCE_GOLD)

    for i = 1, 65 do
        if (BlzGetTriggerFrame() == btnUnitAdd[i]) then
            local unitType = tableUnitTypes[i]

            if (totalGold >= R2I(GetUnitGoldCost(unitType))) then
                local cost = R2I(GetUnitGoldCost(unitType))
                SetPlayerStateBJ(Player(testPlayerSpawn - 1), PLAYER_STATE_RESOURCE_GOLD, totalGold - cost)

                unitSpawns[testPlayerSpawn][i] = unitSpawns[testPlayerSpawn][i] + 1
                udg_TotalUnits[testPlayerSpawn] = udg_TotalUnits[testPlayerSpawn] + 1
            end
        end
    end
end

function TestSell()
end

function LeakDetectorGlobals()
    Title = "Handle Counter"
    HandlesTitle = "Handles"
    SecondsTitle = "Time (sec.)"
    Timeout = 1.
    DisplayTo = Player(0)
    Row1 = Player(0)
    Row1 = Player(1)
    Single_Only = true
    Trigger = CreateTrigger()
    Lead = nil
    Seconds = 0
    L = nil
end

function H2I(h)
    return h
end

function HandleCounter_Conditions()
    return not Single_Only or bj_isSinglePlayer
end

function Update()
    Seconds = Seconds + Timeout
    LeaderboardSetItemValue(Lead,0,Seconds)
    L = Location(5.,5.)
    LeaderboardSetItemValue(Lead,1,H2I(L)-0x100000)
    RemoveLocation(L)
end

function HandleCounter_Actions()
    DestroyTrigger(Trigger)
    Trigger = CreateTrigger()
    TriggerRegisterTimerEvent(Trigger,Timeout,true)
    TriggerAddAction(Trigger,Update)
    
    Lead = CreateLeaderboard()
    LeaderboardSetLabel(Lead, Title)
    PlayerSetLeaderboard(DisplayTo,Lead)
    LeaderboardDisplay(Lead,true)
    LeaderboardAddItem(Lead,SecondsTitle,Seconds,Row1)
    LeaderboardAddItem(Lead,HandlesTitle,0,Row2)
    LeaderboardSetSizeByItemCount(Lead,7)
end

function InitLeakDetector()
    LeakDetectorGlobals()
    TriggerRegisterTimerEvent(Trigger,.001,false)
    TriggerAddCondition(Trigger,Condition(HandleCounter_Conditions))
    TriggerAddAction( Trigger, HandleCounter_Actions )
end