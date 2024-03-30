globals
	framehandle scoreboard
	framehandle frameIconHuman
	framehandle frameIconOrc
	framehandle frameIconUndead
	framehandle frameIconNightElves
	framehandle victory
	framehandle defeat
	framehandle continue
	framehandle extraRound
	framehandle xrVotesText
	string totalColor = "|cffff7c7c"
	string array raceNameWithColor

	framehandle array pName[6]
	framehandle array pScore[6]
	framehandle array pKills[6]
	framehandle array pDamage[6]
	framehandle array pUnitsSpawned[6]
	framehandle array pGoldEarned[6]
	framehandle array pVote[6]
	framehandle array pVoteYes[6]
	framehandle array pBar[6]

	framehandle scoreTeam1
	framehandle killsTeam1
	framehandle damageTeam1
	framehandle unitsTeam1
	framehandle goldTeam1
	framehandle scoreTeam2
	framehandle killsTeam2
	framehandle damageTeam2
	framehandle unitsTeam2
	framehandle goldTeam2
endglobals


function ScoreboardRace takes nothing returns nothing
	local integer p = GetConvertedPlayerId(GetEnumPlayer())
	if (udg_PlayerRace[p] == 1) then
		if (GetLocalPlayer() == GetEnumPlayer()) then
			call BlzFrameSetVisible(frameIconHuman, true)
		endif
	endif
	if (udg_PlayerRace[p] == 2) then
		if (GetLocalPlayer() == GetEnumPlayer()) then
			call BlzFrameSetVisible(frameIconOrc, true)
		endif
	endif
	if (udg_PlayerRace[p] == 3) then
		if (GetLocalPlayer() == GetEnumPlayer()) then
			call BlzFrameSetVisible(frameIconUndead, true)
		endif
	endif
	if (udg_PlayerRace[p] == 4) then
		if (GetLocalPlayer() == GetEnumPlayer()) then
			call BlzFrameSetVisible(frameIconNightElves, true)
		endif
	endif
endfunction


function ShowScoreboard takes nothing returns nothing
	local integer i
	local integer endIndex
    
	call ForForce(GetPlayersAll(), function ScoreboardRace) // sets the correct icon
	call BlzFrameSetVisible(scoreboard, true)

	set i = 1
	set endIndex = 6
	loop
		exitwhen i > endIndex
		call BlzFrameSetText(pScore[i], I2S(udg_TotalScore[i]))
		call BlzFrameSetText(pKills[i], I2S(udg_TotalKills[i]))
		call BlzFrameSetText(pDamage[i], I2S(R2I(udg_TotalDamage[i])))
		//call BlzFrameSetText(pUnitsSpawned[i], I2S((udg_TotalUnitsSpawned[i])))
		call BlzFrameSetText(pGoldEarned[i], I2S(udg_TotalGoldEarned[i]))
		if (udg_Victory[i] == true) then
			if (GetLocalPlayer() == Player(i - 1)) then
				call BlzFrameSetVisible(victory, true)
				call BlzFrameSetVisible(defeat, false)
			endif
		endif
		set i = i + 1
	endloop

	// Total

	set totalScore[1] = udg_TotalScore[1] + udg_TotalScore[3] + udg_TotalScore[5]
	set totalKills[1] = udg_TotalKills[1] + udg_TotalKills[3] + udg_TotalKills[5]
	set totalDamage[1] = R2I(udg_TotalDamage[1]) + R2I(udg_TotalDamage[3]) + R2I(udg_TotalDamage[5])
	set totalUnits[1] = udg_TotalUnitsSpawned[1] + udg_TotalUnitsSpawned[3] + udg_TotalUnitsSpawned[5]
	set totalGold[1] = udg_TotalGoldEarned[1] + udg_TotalGoldEarned[3] + udg_TotalGoldEarned[5]
	set totalScore[2] = udg_TotalScore[2] + udg_TotalScore[4] + udg_TotalScore[6]
	set totalKills[2] = udg_TotalKills[2] + udg_TotalKills[4] + udg_TotalKills[6]
	set totalDamage[2] = R2I(udg_TotalDamage[2]) + R2I(udg_TotalDamage[4]) + R2I(udg_TotalDamage[6])
	set totalUnits[2] = udg_TotalUnitsSpawned[2] + udg_TotalUnitsSpawned[4] + udg_TotalUnitsSpawned[6]
	set totalGold[2] = udg_TotalGoldEarned[2] + udg_TotalGoldEarned[4] + udg_TotalGoldEarned[6]

	call BlzFrameSetText(scoreTeam1, totalColor + I2S(totalScore[1]) + "|r")
	call BlzFrameSetText(killsTeam1, totalColor + I2S(totalKills[1]) + "|r")
	call BlzFrameSetText(damageTeam1, totalColor + I2S(R2I(totalDamage[1])) + "|r")
	//call BlzFrameSetText(unitsTeam1, totalColor + I2S(totalUnits[1]) + "|r")
	call BlzFrameSetText(goldTeam1, totalColor + I2S(totalGold[1]) + "|r")

	call BlzFrameSetText(scoreTeam2, totalColor + I2S(totalScore[2]) + "|r")
	call BlzFrameSetText(killsTeam2, totalColor + I2S(totalKills[2]) + "|r")
	call BlzFrameSetText(damageTeam2, totalColor + I2S(R2I(totalDamage[2])) + "|r")
	//call BlzFrameSetText(unitsTeam2, totalColor + I2S(totalUnits[2]) + "|r")
	call BlzFrameSetText(goldTeam2, totalColor + I2S(totalGold[2]) + "|r")

	// Initialize Vote

	set noPlayersOnRoundEnd = GetActivePlayers()
	call BlzFrameSetText(xrVotesText, "Votes: " + "0" + "/" + I2S(noPlayersOnRoundEnd))
    
endfunction


function StopAndResetUnitSpawn takes nothing returns nothing
	local integer p
	local integer endIndex
	local integer i
	local integer endIndex1

	call PauseTimer(udg_SpawnDelayTimer[1])
	call PauseTimer(udg_SpawnDelayTimer[2])

	set p = 1
	set endIndex = 6
	loop
		exitwhen p > endIndex
        
		set i = 1
		set endIndex1 = 65
		loop
			exitwhen i > endIndex1
			set unitSpawns[p][i] = 0
			set i = i + 1
		endloop

		set udg_TotalUnits[p] = 0
		set p = p + 1
	endloop
endfunction

function OnClickXR takes nothing returns nothing
	local integer p = GetConvertedPlayerId(GetTriggerPlayer())
	local integer i
	local integer endIndex

	call BlzFrameSetVisible(pVote[p], false)
	call BlzFrameSetVisible(pVoteYes[p], true)
    
	if (udg_XRVoteClickCheck[p] == false) then
		set udg_XRVoteClickCheck[p] = true
		set xrVotes[p] = 1

		set totalVotes = 0

		set i = 1
		set endIndex = 6
		loop
			exitwhen i > endIndex
			set totalVotes = totalVotes + xrVotes[i]
			set i = i + 1
		endloop

		if (totalVotes >= noPlayersOnRoundEnd) then
			set totalVotes = 0
			call BlzFrameSetVisible(frameIconHuman, false)
			call BlzFrameSetVisible(frameIconOrc, false)
			call BlzFrameSetVisible(frameIconUndead, false)
			call BlzFrameSetVisible(frameIconNightElves, false)
			call BlzFrameSetVisible(victory, false)
			call BlzFrameSetVisible(defeat, true)
			call StopAndResetUnitSpawn()
			call InitiateRaceSelection()
			call TriggerExecuteBJ(gg_trg_Reset_Game, true)
			set i = 1
			set endIndex = 6
			loop
				exitwhen i > endIndex
				set xrVotes[i] = 0
				call BlzFrameSetVisible(pVoteYes[i], false)
				call BlzFrameSetVisible(pVote[i], true)

				set i = i + 1
			endloop
		endif
        
		call BlzFrameSetText(xrVotesText, "Votes: " + I2S(totalVotes) + "/" + I2S(noPlayersOnRoundEnd))
	endif
endfunction

function OnClickContinue takes nothing returns nothing
	local integer p = GetConvertedPlayerId(GetTriggerPlayer())
	if (GetLocalPlayer() == GetTriggerPlayer()) then
		call BlzFrameSetVisible(scoreboard, false)
	endif
    
	//if(udg_MMD_Wins[p] >= udg_MMD_Losses[p]) then 
	//    call CustomVictoryBJ(Player(p - 1), false, true)
	//else
	//    call CustomDefeatBJ(Player(p - 1), "Defeat")
	//endif

	call CustomVictoryBJ(Player(p - 1), false, true)

endfunction

function CreateScoreboard takes nothing returns nothing
	local framehandle team1
	local framehandle unitsKilled
	local framehandle damageDealt
	local framehandle score
	local framehandle goldEarned
	local framehandle frameVote
	local framehandle team2
	local framehandle unitsKilled2
	local framehandle damageDealt2
	local framehandle score2
	local framehandle goldEarned2
	local framehandle frameVote2
	local string pColorEnd
	local real yPos
	local integer p
	local integer i
	local integer endIndex
	local framehandle total1
	local framehandle total2
	local trigger trig
	local trigger trig2

	set scoreboard = BlzCreateFrame("EscMenuBackdrop", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
	call BlzFrameSetSize(scoreboard, 0.6, 0.35)
	call BlzFrameSetAbsPoint(scoreboard, FRAMEPOINT_CENTER, 0.4, 0.35)
	call BlzFrameSetVisible(scoreboard, false)

	set frameIconHuman = BlzCreateFrameByType("BACKDROP", "", scoreboard, "", 0)
	call BlzFrameSetSize(frameIconHuman, 0.15, 0.15)
	call BlzFrameSetAbsPoint(frameIconHuman, FRAMEPOINT_CENTER, 0.6, 0.45)
	call BlzFrameSetTexture(frameIconHuman, "war3mapImported\\HumanBanner.dds", 0, true)
	call BlzFrameSetVisible(frameIconHuman, false)

	set frameIconOrc = BlzCreateFrameByType("BACKDROP", "", scoreboard, "", 0)
	call BlzFrameSetSize(frameIconOrc, 0.15, 0.15)
	call BlzFrameSetAbsPoint(frameIconOrc, FRAMEPOINT_CENTER, 0.6, 0.45)
	call BlzFrameSetTexture(frameIconOrc, "war3mapImported\\OrcBanner.dds", 0, true)
	call BlzFrameSetVisible(frameIconOrc, false)

	set frameIconUndead = BlzCreateFrameByType("BACKDROP", "", scoreboard, "", 0)
	call BlzFrameSetSize(frameIconUndead, 0.15, 0.15)
	call BlzFrameSetAbsPoint(frameIconUndead, FRAMEPOINT_CENTER, 0.6, 0.45)
	call BlzFrameSetTexture(frameIconUndead, "war3mapImported\\UndeadBanner.dds", 0, true)
	call BlzFrameSetVisible(frameIconUndead, false)

	set frameIconNightElves = BlzCreateFrameByType("BACKDROP", "", scoreboard, "", 0)
	call BlzFrameSetSize(frameIconNightElves, 0.15, 0.15)
	call BlzFrameSetAbsPoint(frameIconNightElves, FRAMEPOINT_CENTER, 0.6, 0.45)
	call BlzFrameSetTexture(frameIconNightElves, "war3mapImported\\NightElfBanner.dds", 0, true)
	call BlzFrameSetVisible(frameIconNightElves, false)

	set victory = BlzCreateFrameByType("BACKDROP", "", scoreboard, "", 0)
	call BlzFrameSetSize(victory, 0.2, 0.065)
	call BlzFrameSetAbsPoint(victory, FRAMEPOINT_LEFT, 0.135, 0.24)
	call BlzFrameSetTexture(victory, "war3mapImported\\Victory.tga", 0, true)
	call BlzFrameSetVisible(victory, false)

	set defeat = BlzCreateFrameByType("BACKDROP", "", scoreboard, "", 0)
	call BlzFrameSetSize(defeat, 0.2, 0.065)
	call BlzFrameSetAbsPoint(defeat, FRAMEPOINT_LEFT, 0.135, 0.24)
	call BlzFrameSetTexture(defeat, "war3mapImported\\Defeat.tga", 0, true)
	call BlzFrameSetVisible(defeat, true)

	set continue = BlzCreateFrame("ScriptDialogButton", scoreboard, 0, 0)
	call BlzFrameSetSize(continue, 0.08, 0.04)
	call BlzFrameSetAbsPoint(continue, FRAMEPOINT_CENTER, 0.635, 0.23)
	call BlzFrameSetText(continue, "Quit")

	set extraRound = BlzCreateFrame("ScriptDialogButton", scoreboard, 0, 0)
	call BlzFrameSetSize(extraRound, 0.12, 0.04)
	call BlzFrameSetAbsPoint(extraRound, FRAMEPOINT_CENTER, 0.52, 0.23)
	call BlzFrameSetText(extraRound, "Extra Round")

	set xrVotesText = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(xrVotesText, FRAMEPOINT_CENTER, 0.52, 0.26)
	call BlzFrameSetText(xrVotesText, "Votes: 0/6")

	set team1 = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(team1, FRAMEPOINT_LEFT, 0.135, 0.49)
	call BlzFrameSetText(team1, "Team 1")

	set score = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(score, FRAMEPOINT_LEFT, 0.235, 0.49)
	call BlzFrameSetText(score, "Score")

	set unitsKilled = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(unitsKilled, FRAMEPOINT_LEFT, 0.285, 0.49)
	call BlzFrameSetText(unitsKilled, "Kills")

	set damageDealt = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(damageDealt, FRAMEPOINT_LEFT, 0.345, 0.49)
	call BlzFrameSetText(damageDealt, "Damage Done")

	set goldEarned = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(goldEarned, FRAMEPOINT_LEFT, 0.425, 0.49)
	call BlzFrameSetText(goldEarned, "Gold Earned")

	set frameVote = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(frameVote, FRAMEPOINT_LEFT, 0.495, 0.49)
	call BlzFrameSetText(frameVote, "Vote")

	set team2 = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(team2, FRAMEPOINT_LEFT, 0.135, 0.38)
	call BlzFrameSetText(team2, "Team 2")

	set score2 = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(score2, FRAMEPOINT_LEFT, 0.235, 0.38)
	call BlzFrameSetText(score2, "Score")

	set unitsKilled2 = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(unitsKilled2, FRAMEPOINT_LEFT, 0.285, 0.38)
	call BlzFrameSetText(unitsKilled2, "Kills")

	set damageDealt2 = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(damageDealt2, FRAMEPOINT_LEFT, 0.345, 0.38)
	call BlzFrameSetText(damageDealt2, "Damage Done")

	set goldEarned2 = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(goldEarned2, FRAMEPOINT_LEFT, 0.425, 0.38)
	call BlzFrameSetText(goldEarned2, "Gold Earned")

	set frameVote2 = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(frameVote2, FRAMEPOINT_LEFT, 0.495, 0.38)
	call BlzFrameSetText(frameVote2, "Vote")



	set pColorEnd = "|r"
	

	// Team 1
	set yPos = 0.05
	set p = 0

	set i = 1
	set endIndex = 3
	loop
		exitwhen i > endIndex
		if (udg_ActivePlayers[i + p] == true) then
			set pBar[i + p] = BlzCreateFrameByType("BACKDROP", "", scoreboard, "", 0)
			call BlzFrameSetSize(pBar[i + p], 0.385, 0.018)
			call BlzFrameSetAbsPoint(pBar[i + p], FRAMEPOINT_LEFT, 0.135, 0.52 - yPos)
			call BlzFrameSetTexture(pBar[i + p], "war3mapImported\\BarPlayer" + I2S(i + p) + ".tga", 0, true)
			call BlzFrameSetAlpha(pBar[i + p], 100)

			set pName[i + p] = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
			call BlzFrameSetAbsPoint(pName[i + p], FRAMEPOINT_LEFT, 0.135, 0.52 - yPos)
			//call BlzFrameSetText(pName[i + p], pColor[i + p] + GetShortPlayerName(Player(i - 1 + p)) + pColorEnd)
			call BlzFrameSetText(pName[i + p], GetShortPlayerName(Player(i - 1 + p)))

			set pScore[i + p] = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
			call BlzFrameSetAbsPoint(pScore[i + p], FRAMEPOINT_LEFT, 0.235, 0.52 - yPos)
			call BlzFrameSetText(pScore[i + p], "0")

			set pKills[i + p] = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
			call BlzFrameSetAbsPoint(pKills[i + p], FRAMEPOINT_LEFT, 0.285, 0.52 - yPos)
			call BlzFrameSetText(pKills[i + p], "0")

			set pDamage[i + p] = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
			call BlzFrameSetAbsPoint(pDamage[i + p], FRAMEPOINT_LEFT, 0.345, 0.52 - yPos)
			call BlzFrameSetText(pDamage[i + p], "0")

			set pGoldEarned[i + p] = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
			call BlzFrameSetAbsPoint(pGoldEarned[i + p], FRAMEPOINT_LEFT, 0.425, 0.52 - yPos)
			call BlzFrameSetText(pGoldEarned[i + p], "0")

			set pVote[i + p] = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
			call BlzFrameSetAbsPoint(pVote[i + p], FRAMEPOINT_LEFT, 0.495, 0.52 - yPos)
			call BlzFrameSetText(pVote[i + p], "-")

			set pVoteYes[i + p] = BlzCreateFrameByType("BACKDROP", "", scoreboard, "", 0)
			call BlzFrameSetAbsPoint(pVoteYes[i + p], FRAMEPOINT_LEFT, 0.495, 0.52 - yPos)
			call BlzFrameSetSize(pVoteYes[i + p], 0.02, 0.02)
			call BlzFrameSetTexture(pVoteYes[i + p], "war3mapImported\\green-check.dds", 0, true)
			call BlzFrameSetVisible(pVoteYes[i + p], false)

		endif
		set yPos = yPos + 0.02
		set p = p + 1
		set i = i + 1
	endloop

	// Total Team 1
	set total1 = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(total1, FRAMEPOINT_LEFT, 0.135, 0.41)
	call BlzFrameSetText(total1, "Total")

	set scoreTeam1 = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(scoreTeam1, FRAMEPOINT_LEFT, 0.235, 0.41)
	call BlzFrameSetText(scoreTeam1, totalColor + "0" + "|r")

	set killsTeam1 = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(killsTeam1, FRAMEPOINT_LEFT, 0.285, 0.41)
	call BlzFrameSetText(killsTeam1, totalColor + "0" + "|r")

	set damageTeam1 = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(damageTeam1, FRAMEPOINT_LEFT, 0.345, 0.41)
	call BlzFrameSetText(damageTeam1, totalColor + "0" + "|r")

	set goldTeam1 = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(goldTeam1, FRAMEPOINT_LEFT, 0.425, 0.41)
	call BlzFrameSetText(goldTeam1, totalColor + "0" + "|r")

	// Team 2
	set yPos = 0.05
	set p = 1
	set i = 1
	loop
		exitwhen i > endIndex
		if (udg_ActivePlayers[i + p] == true) then
			set pBar[i + p] = BlzCreateFrameByType("BACKDROP", "", scoreboard, "", 0)
			call BlzFrameSetSize(pBar[i + p], 0.385, 0.018)
			call BlzFrameSetAbsPoint(pBar[i + p], FRAMEPOINT_LEFT, 0.135, 0.41 - yPos)
			call BlzFrameSetTexture(pBar[i + p], "war3mapImported\\BarPlayer" + I2S(i + p) + ".tga", 0, true)
			call BlzFrameSetAlpha(pBar[i + p], 100)

			set pName[i + p] = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
			call BlzFrameSetAbsPoint(pName[i + p], FRAMEPOINT_LEFT, 0.135, 0.41 - yPos)
			//call BlzFrameSetText(pName[i + p], pColor[i + p] + GetShortPlayerName(Player(i - 1 + p)) + pColorEnd)
			call BlzFrameSetText(pName[i + p], GetShortPlayerName(Player(i - 1 + p)))

			set pScore[i + p] = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
			call BlzFrameSetAbsPoint(pScore[i + p], FRAMEPOINT_LEFT, 0.235, 0.41 - yPos)
			call BlzFrameSetText(pScore[i + p], "0")

			set pKills[i + p] = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
			call BlzFrameSetAbsPoint(pKills[i + p], FRAMEPOINT_LEFT, 0.285, 0.41 - yPos)
			call BlzFrameSetText(pKills[i + p], "0")

			set pDamage[i + p] = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
			call BlzFrameSetAbsPoint(pDamage[i + p], FRAMEPOINT_LEFT, 0.345, 0.41 - yPos)
			call BlzFrameSetText(pDamage[i + p], "0")

			set pGoldEarned[i + p] = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
			call BlzFrameSetAbsPoint(pGoldEarned[i + p], FRAMEPOINT_LEFT, 0.425, 0.41 - yPos)
			call BlzFrameSetText(pGoldEarned[i + p], "0")

			set pVote[i + p] = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
			call BlzFrameSetAbsPoint(pVote[i + p], FRAMEPOINT_LEFT, 0.495, 0.41 - yPos)
			call BlzFrameSetText(pVote[i + p], "-")

			set pVoteYes[i + p] = BlzCreateFrameByType("BACKDROP", "", scoreboard, "", 0)
			call BlzFrameSetAbsPoint(pVoteYes[i + p], FRAMEPOINT_LEFT, 0.495, 0.41 - yPos)
			call BlzFrameSetSize(pVoteYes[i + p], 0.02, 0.02)
			call BlzFrameSetTexture(pVoteYes[i + p], "war3mapImported\\green-check.dds", 0, true)
			call BlzFrameSetVisible(pVoteYes[i + p], false)
		endif
		set yPos = yPos + 0.02
		set p = p + 1
		set i = i + 1
	endloop

	// Total Team 2
	set total2 = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(total2, FRAMEPOINT_LEFT, 0.135, 0.3)
	call BlzFrameSetText(total2, "Total")

	set scoreTeam2 = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(scoreTeam2, FRAMEPOINT_LEFT, 0.235, 0.3)
	call BlzFrameSetText(scoreTeam2, totalColor + "0" + pColorEnd)

	set killsTeam2 = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(killsTeam2, FRAMEPOINT_LEFT, 0.285, 0.3)
	call BlzFrameSetText(killsTeam2, totalColor + "0" + pColorEnd)

	set damageTeam2 = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(damageTeam2, FRAMEPOINT_LEFT, 0.345, 0.3)
	call BlzFrameSetText(damageTeam2, totalColor + "0" + pColorEnd)

	set goldTeam2 = BlzCreateFrame("TasButtonTextTemplate", scoreboard, 0, 0)
	call BlzFrameSetAbsPoint(goldTeam2, FRAMEPOINT_LEFT, 0.425, 0.3)
	call BlzFrameSetText(goldTeam2, totalColor + "0" + pColorEnd)

	// BUTTON CALLBACKS //
	set trig = CreateTrigger()
	call BlzTriggerRegisterFrameEvent(trig, continue, FRAMEEVENT_CONTROL_CLICK)
	call TriggerAddAction(trig, function OnClickContinue)

	set trig2 = CreateTrigger()
	call BlzTriggerRegisterFrameEvent(trig2, extraRound, FRAMEEVENT_CONTROL_CLICK)
	call TriggerAddAction(trig2, function OnClickXR)

	// This holds the votes for the players
	set timeRemainingRound = 30
	set noPlayersOnRoundEnd = GetActivePlayers()
	set i = 1
	set endIndex = 6
	loop
		exitwhen i > endIndex
		set xrVotes[i] = 0
		set i = i + 1
	endloop
endfunction