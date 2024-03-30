function TowerBarRefresh takes nothing returns nothing
	local real health1 = GetUnitLifePercent(udg_WatchTower[1]) * 0.0006
	local real health2 = GetUnitLifePercent(udg_WatchTower[2]) * 0.0006
	call BlzFrameSetSize(frameTowerFill[1], health1, 0.008)
	call BlzFrameSetSize(frameTowerFill[2], health2, 0.008)
endfunction

function DamageDetectTowersCOND takes nothing returns boolean
	local boolean isTower = false

	if (BlzGetEventDamageTarget() == udg_WatchTower[1] or BlzGetEventDamageTarget() == udg_WatchTower[2]) then
		set isTower = true
	endif

	return isTower
endfunction

function DamageDetectTowers takes nothing returns nothing
	local real health = GetUnitLifePercent(BlzGetEventDamageTarget()) * 0.0006

	if (BlzGetEventDamageTarget() == udg_WatchTower[1]) then
		call BlzFrameSetSize(frameTowerFill[1], health, 0.008)
	elseif (BlzGetEventDamageTarget() == udg_WatchTower[2]) then
		call BlzFrameSetSize(frameTowerFill[2], health, 0.008)
	endif
endfunction

function DamageDetectFortressCOND takes nothing returns boolean
	local boolean isFortress = false

	if (BlzGetEventDamageTarget() == udg_Fortress[1] or BlzGetEventDamageTarget() == udg_Fortress[2]) then
		set isFortress = true
	endif

	return isFortress
endfunction

function DamageDetectFortress takes nothing returns nothing
	local real health = GetUnitLifePercent(BlzGetEventDamageTarget()) * 0.0006

	if (BlzGetEventDamageTarget() == udg_Fortress[1]) then
		call BlzFrameSetSize(frameFortressFill[1], health, 0.008)
	elseif (BlzGetEventDamageTarget() == udg_Fortress[2]) then
		call BlzFrameSetSize(frameFortressFill[2], health, 0.008)
	endif
endfunction

function TowerDiesTeam1 takes nothing returns nothing
	call BlzFrameSetVisible(frameWatchTower[1], false)
	call BlzFrameSetVisible(frameTowerHealth[1], false)
	call BlzFrameSetVisible(frameTowerFill[1], false)

	call BlzFrameSetVisible(frameFortressHealth[1], true)
	call BlzFrameSetVisible(frameFortressFill[1], true)

	call StartTimerBJ(udg_FrameFortressTimer[1], false, 0.02)
endfunction

function TowerDiesTeam2 takes nothing returns nothing
	call BlzFrameSetVisible(frameWatchTower[2], false)
	call BlzFrameSetVisible(frameTowerHealth[2], false)
	call BlzFrameSetVisible(frameTowerFill[2], false)

	call BlzFrameSetVisible(frameFortress[2], true)
	call BlzFrameSetVisible(frameFortressHealth[2], true)
	call BlzFrameSetVisible(frameFortressFill[2], true)

	call StartTimerBJ(udg_FrameFortressTimer[2], false, 0.02)
endfunction

function FrameFortressAnim1 takes nothing returns nothing
	call BlzFrameSetVisible(frameFortress[1], true)
	if (frameFortressCounter[1] < 255) then
		// increase counter so the trigger runs until alpha equal 255
		set frameFortressCounter[1] = frameFortressCounter[1] + 10

		// edit dimensions and placement
		set frameFortDim[1] = frameFortDim[1] - 0.005
		set frameFortAncorX[1] = frameFortAncorX[1] + 0.005
		set frameFortAncorY[1] = frameFortAncorY[1] - 0.005

		// set values
		call BlzFrameSetSize(frameFortress[1], frameFortDim[1], frameFortDim[1])
		call BlzFrameSetAbsPoint(frameFortress[1], FRAMEPOINT_CENTER, frameFortAncorX[1], frameFortAncorY[1])
		call BlzFrameSetAlpha(frameFortress[1], frameFortressCounter[1])

		call StartTimerBJ(udg_FrameFortressTimer[1], false, 0.02)
	endif
	if (frameFortressCounter[1] >= 255) then
		call BlzFrameSetAlpha(frameFortress[1], 255)
	else
		call StartTimerBJ(udg_FrameFortressTimer[1], false, 0.02)
	endif
endfunction

function FrameFortressAnim2 takes nothing returns nothing
	call BlzFrameSetVisible(frameFortress[2], true)
	if (frameFortressCounter[2] < 255) then
		// increase counter so the trigger runs until alpha equal 255
		set frameFortressCounter[2] = frameFortressCounter[2] + 10

		// edit dimensions and placement
		set frameFortDim[2] = frameFortDim[2] - 0.005
		set frameFortAncorX[2] = frameFortAncorX[2] + 0.005
		set frameFortAncorY[2] = frameFortAncorY[2] - 0.005

		// set values
		call BlzFrameSetSize(frameFortress[2], frameFortDim[2], frameFortDim[2])
		call BlzFrameSetAbsPoint(frameFortress[2], FRAMEPOINT_CENTER, frameFortAncorX[2], frameFortAncorY[2])
		call BlzFrameSetAlpha(frameFortress[2], frameFortressCounter[2])

		call StartTimerBJ(udg_FrameFortressTimer[2], false, 0.02)
	endif
	if (frameFortressCounter[2] >= 255) then
		call BlzFrameSetAlpha(frameFortress[2], 255)
	else
		call StartTimerBJ(udg_FrameFortressTimer[2], false, 0.02)
	endif
endfunction

function InitUIFrameHealthIndicators takes nothing returns nothing
	local trigger trig
	local trigger trig2
	local trigger trig3
	local trigger towerHPTrig
	local trigger dmgTrig
	local trigger dmgTrig2
	local trigger fortressTrig1
	local trigger fortressTrig2

	// Fortress and Watch Tower display
	// Team 1

	set frameWatchTower[1] = BlzCreateFrameByType("BACKDROP", "", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
	call BlzFrameSetSize(frameWatchTower[1], 0.055, 0.083)
	call BlzFrameSetAbsPoint(frameWatchTower[1], FRAMEPOINT_CENTER, 0.75, 0.2)
	call BlzFrameSetTexture(frameWatchTower[1], "war3mapImported\\WatchTower Red.tga", 0, true)
	call BlzFrameSetVisible(frameWatchTower[1], false)
	call BlzFrameSetLevel(frameWatchTower[1], -2)

	set frameTowerHealth[1] = BlzCreateFrameByType("BACKDROP", "", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
	call BlzFrameSetSize(frameTowerHealth[1], 0.06, 0.01)
	call BlzFrameSetAbsPoint(frameTowerHealth[1], FRAMEPOINT_LEFT, 0.72, 0.175)
	call BlzFrameSetTexture(frameTowerHealth[1], "war3mapImported\\bar.tga", 0, true)
	call BlzFrameSetVisible(frameTowerHealth[1], false)
	call BlzFrameSetLevel(frameTowerHealth[1], -1)

	set frameTowerFill[1] = BlzCreateFrameByType("BACKDROP", "", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
	call BlzFrameSetSize(frameTowerFill[1], 0.06, 0.008)
	call BlzFrameSetAbsPoint(frameTowerFill[1], FRAMEPOINT_LEFT, 0.72, 0.175)
	call BlzFrameSetTexture(frameTowerFill[1], "war3mapImported\\Bar Health.tga", 0, true)
	call BlzFrameSetVisible(frameTowerFill[1], false)
	call BlzFrameSetLevel(frameTowerFill[1], -2)

	//

	set frameFortress[1] = BlzCreateFrameByType("BACKDROP", "", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
	call BlzFrameSetSize(frameFortress[1], 0.083, 0.083)
	call BlzFrameSetAbsPoint(frameFortress[1], FRAMEPOINT_CENTER, 0.75, 0.2)
	call BlzFrameSetTexture(frameFortress[1], "war3mapImported\\Fortress RED.tga", 0, true)
	call BlzFrameSetVisible(frameFortress[1], false)
	call BlzFrameSetAlpha(frameFortress[1], 1)
	call BlzFrameSetLevel(frameFortress[1], -2)

	set frameFortressHealth[1] = BlzCreateFrameByType("BACKDROP", "", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
	call BlzFrameSetSize(frameFortressHealth[1], 0.06, 0.01)
	call BlzFrameSetAbsPoint(frameFortressHealth[1], FRAMEPOINT_LEFT, 0.72, 0.175)
	call BlzFrameSetTexture(frameFortressHealth[1], "war3mapImported\\bar.tga", 0, true)
	call BlzFrameSetVisible(frameFortressHealth[1], false)
	call BlzFrameSetLevel(frameFortressHealth[1], -1)

	set frameFortressFill[1] = BlzCreateFrameByType("BACKDROP", "", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
	call BlzFrameSetSize(frameFortressFill[1], 0.06, 0.008)
	call BlzFrameSetAbsPoint(frameFortressFill[1], FRAMEPOINT_LEFT, 0.72, 0.175)
	call BlzFrameSetTexture(frameFortressFill[1], "war3mapImported\\Bar Health.tga", 0, true)
	call BlzFrameSetVisible(frameFortressFill[1], false)
	call BlzFrameSetLevel(frameFortressFill[1], -2)

	// Team 2

	set frameWatchTower[2] = BlzCreateFrameByType("BACKDROP", "", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
	call BlzFrameSetSize(frameWatchTower[2], 0.055, 0.083)
	call BlzFrameSetAbsPoint(frameWatchTower[2], FRAMEPOINT_CENTER, 0.67, 0.2)
	call BlzFrameSetTexture(frameWatchTower[2], "war3mapImported\\WatchTower Blue.tga", 0, true)
	call BlzFrameSetVisible(frameWatchTower[2], false)
	call BlzFrameSetLevel(frameWatchTower[2], -2)

	set frameTowerHealth[2] = BlzCreateFrameByType("BACKDROP", "", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
	call BlzFrameSetSize(frameTowerHealth[2], 0.06, 0.01)
	call BlzFrameSetAbsPoint(frameTowerHealth[2], FRAMEPOINT_LEFT, 0.64, 0.175)
	call BlzFrameSetTexture(frameTowerHealth[2], "war3mapImported\\bar.tga", 0, true)
	call BlzFrameSetVisible(frameTowerHealth[2], false)
	call BlzFrameSetLevel(frameTowerHealth[2], -1)

	set frameTowerFill[2] = BlzCreateFrameByType("BACKDROP", "", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
	call BlzFrameSetSize(frameTowerFill[2], 0.06, 0.008)
	call BlzFrameSetAbsPoint(frameTowerFill[2], FRAMEPOINT_LEFT, 0.64, 0.175)
	call BlzFrameSetTexture(frameTowerFill[2], "war3mapImported\\Bar Health.tga", 0, true)
	call BlzFrameSetVisible(frameTowerFill[2], false)
	call BlzFrameSetLevel(frameTowerFill[2], -2)

	//

	set frameFortress[2] = BlzCreateFrameByType("BACKDROP", "", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
	call BlzFrameSetSize(frameFortress[2], 0.083, 0.083)
	call BlzFrameSetAbsPoint(frameFortress[2], FRAMEPOINT_CENTER, 0.67, 0.2)
	call BlzFrameSetTexture(frameFortress[2], "war3mapImported\\Fortress BLUE.tga", 0, true)
	call BlzFrameSetVisible(frameFortress[2], false)
	call BlzFrameSetAlpha(frameFortress[2], 1)
	call BlzFrameSetLevel(frameFortress[2], -2)

	set frameFortressHealth[2] = BlzCreateFrameByType("BACKDROP", "", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
	call BlzFrameSetSize(frameFortressHealth[2], 0.06, 0.01)
	call BlzFrameSetAbsPoint(frameFortressHealth[2], FRAMEPOINT_LEFT, 0.64, 0.175)
	call BlzFrameSetTexture(frameFortressHealth[2], "war3mapImported\\bar.tga", 0, true)
	call BlzFrameSetVisible(frameFortressHealth[2], false)
	call BlzFrameSetLevel(frameFortressHealth[2], -1)

	set frameFortressFill[2] = BlzCreateFrameByType("BACKDROP", "", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
	call BlzFrameSetSize(frameFortressFill[2], 0.06, 0.008)
	call BlzFrameSetAbsPoint(frameFortressFill[2], FRAMEPOINT_LEFT, 0.64, 0.175)
	call BlzFrameSetTexture(frameFortressFill[2], "war3mapImported\\Bar Health.tga", 0, true)
	call BlzFrameSetVisible(frameFortressFill[2], false)
	call BlzFrameSetLevel(frameFortressFill[2], -2)

	// Tower Bar Refresh
	set towerHPTrig = CreateTrigger()
	call TriggerRegisterTimerEventPeriodic(towerHPTrig, 2)
	call TriggerAddAction(towerHPTrig, function TowerBarRefresh)
    
	// Damage Detect Tower
	set dmgTrig = CreateTrigger()
	call TriggerRegisterAnyUnitEventBJ(dmgTrig, EVENT_PLAYER_UNIT_DAMAGED)
	call TriggerAddCondition(dmgTrig, Condition(function DamageDetectTowersCOND))
	call TriggerAddAction(dmgTrig, function DamageDetectTowers)

	// Damage Detect Fortress
	set dmgTrig2 = CreateTrigger()
	call TriggerRegisterAnyUnitEventBJ(dmgTrig2, EVENT_PLAYER_UNIT_DAMAGED)
	call TriggerAddCondition(dmgTrig2, Condition(function DamageDetectFortressCOND))
	call TriggerAddAction(dmgTrig2, function DamageDetectFortress)

	// Frame Fortress Timer
	set fortressTrig1 = CreateTrigger()
	call TriggerRegisterTimerExpireEvent(fortressTrig1, udg_FrameFortressTimer[1])
	call TriggerAddAction(fortressTrig1, function FrameFortressAnim1)

	set fortressTrig2 = CreateTrigger()
	call TriggerRegisterTimerExpireEvent(fortressTrig2, udg_FrameFortressTimer[2])
	call TriggerAddAction(fortressTrig2, function FrameFortressAnim2)

	set frameFortressCounter[1] = 0
	set frameFortressCounter[2] = 0

	set frameFortDim[1] = 0.210
	set frameFortDim[2] = 0.210

	set frameFortAncorX[1] = 0.623
	set frameFortAncorX[2] = 0.543

	set frameFortAncorY[1] = 0.327
	set frameFortAncorY[2] = 0.327

endfunction