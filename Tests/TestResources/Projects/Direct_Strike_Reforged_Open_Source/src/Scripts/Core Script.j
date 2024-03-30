globals
	integer array spawnPlayer[6]
	integer array unitSpawns[6][65] // legacy code from Desert Strike
	integer array unitSpawnCounter[6][65] // legacy code from Desert Strike
	integer totalVotes // votes for next round

	framehandle array timerText[6] // text displayed on the minimap

	framehandle array frameWatchTower[2]
	framehandle array frameTowerHealth[2]
	framehandle array frameTowerFill[2]

	framehandle array frameFortress[2]
	framehandle array frameFortressHealth[2]
	framehandle array frameFortressFill[2]

	integer array frameFortressCounter[2]
	real array frameFortDim[2]
	real array frameFortAncorX[2]
	real array frameFortAncorY[2]

	integer timeRemainingRound
	integer noPlayersOnRoundEnd
	integer array xrVotes[6]

	integer array totalScore[2]
	integer array totalKills[2]
	integer array totalDamage[2]
	integer array totalUnits[2]
	integer array totalGold[2]

	string array textureNumber

	string array pColor[6]
endglobals

function Setup takes nothing returns nothing
	local framehandle tinyBlackBox

    set pColor[1] = "|cffff0303"
	set pColor[2] = "|cff0042ff"
	set pColor[3] = "|cff1ce6b9"
	set pColor[4] = "|cff540081"
	set pColor[5] = "|cfffffc01"
	set pColor[6] = "|cfffe8a0e"
	
	set totalColor = "|cffff7c7c"
	set raceNameWithColor[0] = "Random"
	set raceNameWithColor[1] = "|cff1b6ef5Human|r"
	set raceNameWithColor[2] = "|cfff5381bOrc|r"
	set raceNameWithColor[3] = "|cff8431f7Undead|r"
	set raceNameWithColor[4] = "|cff008a17Night Elf|r"
	set raceNameWithColor[5] = "|cffa346ffDalaran|r"
	set raceNameWithColor[6] = "|cffff3232Blood Elf|r"
	set raceNameWithColor[7] = "|cff936d96Forsaken|r"
	set raceNameWithColor[8] = "|cff0bd9c1Naga|r"

	set spawnPlayer[1] = 0
	set spawnPlayer[2] = 0

	set totalVotes = 0

	call BlzFrameSetVisible(BlzGetFrameByName("ConsoleUIBackdrop", 0), false) // Hide bottom black box
	set tinyBlackBox = BlzCreateFrameByType("BACKDROP", "", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
	call BlzFrameSetSize(tinyBlackBox, 0.1, 0.15)
	call BlzFrameSetAbsPoint(tinyBlackBox, FRAMEPOINT_CENTER, 0.25, 0.05)
	call BlzFrameSetTexture(tinyBlackBox, "war3mapImported\\Black Box.tga", 0, true)
	call BlzFrameSetLevel(tinyBlackBox, - 1)

    call InitSpawns()
	call InitAIGroups()
	call CreateScoreboard()
	call InitUIMapIndicators()
	call InitUIFrameHealthIndicators()
	call InitUICamera()
	call SetupGrid()
	call SetupAbilityControl()
	call SetupClassicSelection()
endfunction


function MultiboardSetup takes nothing returns nothing
	local integer i = 1
	local integer endIndex = 6
	loop
        exitwhen i > endIndex
		if (GetLocalPlayer() == Player(i - 1)) then
			call MultiboardDisplayBJ(true, udg_Multiboard[i])
		endif
		set i = i + 1
	endloop
endfunction

function GameOverMusic takes nothing returns nothing
	local integer p = GetConvertedPlayerId(GetEnumPlayer())
	if (udg_Victory[p] == true) then
		if (GetLocalPlayer() == GetEnumPlayer()) then
			call PlayThematicMusicBJ("NightElfVictory")
		endif
	else
		if (GetLocalPlayer() == GetEnumPlayer()) then
			call PlayThematicMusicBJ("OrcDefeat")
		endif
	endif
endfunction

function ShowMainUI takes nothing returns nothing
    // Camera Buttons
    call BlzFrameSetVisible(zoomIn, true)
    call BlzFrameSetVisible(zoomOut, true)
    call BlzFrameSetVisible(resetCamera, true)
        
    // Menu Buttons
    call BlzFrameSetVisible(btnAbilityControl, true)
        
    // Grid Button
    call BlzFrameSetVisible(btnGrid, true)

    call BlzFrameSetVisible(frameWatchTower[1], true)
    call BlzFrameSetVisible(frameTowerHealth[1], true)
    call BlzFrameSetVisible(frameTowerFill[1], true)

    call BlzFrameSetVisible(frameWatchTower[2], true)
    call BlzFrameSetVisible(frameTowerHealth[2], true)
    call BlzFrameSetVisible(frameTowerFill[2], true)
endfunction

function HideMainUI takes nothing returns nothing
		// Camera Buttons
		call BlzFrameSetVisible(zoomIn, false)
		call BlzFrameSetVisible(zoomOut, false)
		call BlzFrameSetVisible(resetCamera, false)

		// Menus
		call BlzFrameSetVisible(btnAbilityControl, false)

		// Grid Button
		call BlzFrameSetVisible(btnGrid, false)

		// Tower and Fortress
		call BlzFrameSetVisible(frameWatchTower[1], false)
		call BlzFrameSetVisible(frameTowerHealth[1], false)
		call BlzFrameSetVisible(frameTowerFill[1], false)

		call BlzFrameSetVisible(frameWatchTower[2], false)
		call BlzFrameSetVisible(frameTowerHealth[2], false)
		call BlzFrameSetVisible(frameTowerFill[2], false)

		call BlzFrameSetVisible(frameFortress[1], false)
		call BlzFrameSetVisible(frameFortressHealth[1], false)
		call BlzFrameSetVisible(frameFortressFill[1], false)
		call BlzFrameSetSize(frameFortressFill[1], 0.06, 0.008)

		call BlzFrameSetVisible(frameFortress[2], false)
		call BlzFrameSetVisible(frameFortressHealth[2], false)
		call BlzFrameSetVisible(frameFortressFill[2], false)
		call BlzFrameSetSize(frameFortressFill[2], 0.06, 0.008)
endfunction

function PlayerLeaves takes nothing returns nothing
	local integer p = GetConvertedPlayerId(GetTriggerPlayer())
	call RaceSelectSetNameEmpty()
    
	set udg_ActivePlayers[p] = false
	set noPlayersOnRoundEnd = GetActivePlayers()
	set xrVotes[p] = 0
	call BlzFrameSetText(xrVotesText, "Votes: " + I2S(totalVotes) + "/" + I2S(noPlayersOnRoundEnd))
endfunction