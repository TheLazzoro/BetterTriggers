function UpdateTimerText takes nothing returns nothing
	local integer i = 1
	local integer endIndex = 6
	local integer val
	loop
	exitwhen i > endIndex
		if (udg_ActivePlayers[i] == false) then
			call BlzFrameSetText(timerText[i], "--")
		else
			set val = R2I(TimerGetRemaining(udg_Timer)) + (udg_PlayerQueue[i] * R2I(udg_WaveTime))
			call BlzFrameSetText(timerText[i], I2S(val))
		endif
		set i = i + 1
	endloop
endfunction

function InitUIMapIndicators takes nothing returns nothing
	local integer p
	local integer i
	local integer endIndex
	local real Ypos = 0.00

	// Team 1
	set p = 0
	set i = 1
	set endIndex = 3
	loop
		exitwhen i > endIndex
		set timerText[i + p] = BlzCreateFrame("TasButtonTextTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
		call BlzFrameSetAbsPoint(timerText[i + p], FRAMEPOINT_LEFT, 0.136, 0.09 - Ypos)
		call BlzFrameSetText(timerText[i + p], "--")
		set Ypos = Ypos + 0.015
		set p = p + 1
		set i = i + 1
	endloop

	// Team 2
	set Ypos = 0.00
	set p = 1
	set i = 1
	loop
		exitwhen i > endIndex
		set timerText[i + p] = BlzCreateFrame("TasButtonTextTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
		call BlzFrameSetAbsPoint(timerText[i + p], FRAMEPOINT_LEFT, 0.013, 0.09 - Ypos)
		call BlzFrameSetText(timerText[i + p], "--")
		set Ypos = Ypos + 0.015
		set p = p + 1
		set i = i + 1
	endloop

endfunction