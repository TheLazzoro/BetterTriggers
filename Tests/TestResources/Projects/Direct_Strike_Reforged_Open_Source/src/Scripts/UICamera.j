globals
	framehandle zoomIn
	framehandle zoomOut
	framehandle resetCamera
endglobals

function ZoomOut takes nothing returns nothing
	local integer p = GetConvertedPlayerId(GetTriggerPlayer())
	set udg_CameraZoom[p] = udg_CameraZoom[p] + 500.00
	if (udg_CameraZoom[p] >= 4000.00) then
		set udg_CameraZoom[p] = 4000.00
	endif
	call SetCameraFieldForPlayer(GetTriggerPlayer(), CAMERA_FIELD_TARGET_DISTANCE, udg_CameraZoom[p], 1)
	call BlzFrameSetEnable(BlzGetTriggerFrame(), false)
	call BlzFrameSetEnable(BlzGetTriggerFrame(), true)
endfunction

function ZoomIn takes nothing returns nothing
	local integer p = GetConvertedPlayerId(GetTriggerPlayer())
	set udg_CameraZoom[p] = udg_CameraZoom[p] - 500.00
	if (udg_CameraZoom[p] <= 500.00) then
		set udg_CameraZoom[p] = 500.00
	endif
	call SetCameraFieldForPlayer(GetTriggerPlayer(), CAMERA_FIELD_TARGET_DISTANCE, udg_CameraZoom[p], 1)
	call BlzFrameSetEnable(BlzGetTriggerFrame(), false)
	call BlzFrameSetEnable(BlzGetTriggerFrame(), true)
endfunction

function ResetCamera takes nothing returns nothing
	local integer p = GetConvertedPlayerId(GetTriggerPlayer())
	set udg_CameraZoom[p] = 1650
	call CameraSetupApplyForPlayerSmooth(true, udg_PlayerCameras[p], Player(p - 1), 1.00, 1.00, 1.00, 1.00)
	call BlzFrameSetEnable(BlzGetTriggerFrame(), false)
	call BlzFrameSetEnable(BlzGetTriggerFrame(), true)
endfunction

function InitUICamera takes nothing returns nothing
	local trigger trig
	local trigger trig2
	local trigger trig3

	set zoomIn = BlzCreateFrame("ScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
	call BlzFrameSetSize(zoomIn, 0.05, 0.035)
	call BlzFrameSetAbsPoint(zoomIn, FRAMEPOINT_CENTER, 0.525, 0.55)
	call BlzFrameSetText(zoomIn, " + ")
	call BlzFrameSetVisible(zoomIn, false)

	set zoomOut = BlzCreateFrame("ScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
	call BlzFrameSetSize(zoomOut, 0.05, 0.035)
	call BlzFrameSetAbsPoint(zoomOut, FRAMEPOINT_CENTER, 0.475, 0.55)
	call BlzFrameSetText(zoomOut, " - ")
	call BlzFrameSetVisible(zoomOut, false)

	set resetCamera = BlzCreateFrame("ScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
	call BlzFrameSetSize(resetCamera, 0.095, 0.035)
	call BlzFrameSetAbsPoint(resetCamera, FRAMEPOINT_CENTER, 0.6, 0.55)
	call BlzFrameSetText(resetCamera, "Reset Camera")
	call BlzFrameSetVisible(resetCamera, false)

	// CALLBACK //
	set trig = CreateTrigger()
	call BlzTriggerRegisterFrameEvent(trig, zoomIn, FRAMEEVENT_CONTROL_CLICK)
	call TriggerAddAction(trig, function ZoomIn)

	set trig2 = CreateTrigger()
	call BlzTriggerRegisterFrameEvent(trig2, zoomOut, FRAMEEVENT_CONTROL_CLICK)
	call TriggerAddAction(trig2, function ZoomOut)

	set trig3 = CreateTrigger()
	call BlzTriggerRegisterFrameEvent(trig3, resetCamera, FRAMEEVENT_CONTROL_CLICK)
	call TriggerAddAction(trig3, function ResetCamera)
endfunction