globals
framehandle bt_lastCreatedFrame= null
trigger gg_trg_Untitled_Trigger= null
integer udl_int_0
// processed: 	integer array udl_int0_0[5][6]
group udl_unitgrp_0
// processed: 	group array udl_unitgrp0_0[3]
// processed: 	group array udl_unitgrp1_0[3][4]
// processed: 	force array udl_playergrp_0[3][4]
// processed: 	ability array udl_ability_0[3][4]
// processed: 	integer array udl_abilitycode_0[3][4]
// processed: 	integer array udl_abilitycode0_0[3][4]



//JASSHelper struct globals:
integer array s__udl_int0_0
group array s__udl_unitgrp0_0
group array s__udl_unitgrp1_0
force array s__udl_playergrp_0
ability array s__udl_ability_0
integer array s__udl_abilitycode_0
integer array s__udl_abilitycode0_0

endglobals





function InitGlobals takes nothing returns nothing
endfunction
//****************************************************************************
//* 
//*   Better Triggers Functions
//* 
//****************************************************************************
function BlzQueueImmediateOrderByIdBT takes unit whichUnit,string order returns nothing
    call BlzQueueImmediateOrderById(whichUnit, OrderId(order))
endfunction

function BlzQueuePointOrderByIdBT takes string order,unit whichUnit,location loc returns nothing
    call BlzQueuePointOrderById(whichUnit, OrderId(order), GetLocationX(loc), GetLocationY(loc))
endfunction

function BlzQueueTargetOrderByIdBT takes unit whichUnit,string order,widget targetWidget returns nothing
    call BlzQueueTargetOrderById(whichUnit, OrderId(order), targetWidget)
endfunction//****************************************************************************
//* 
//*   Map Item Tables
//* 
//****************************************************************************


//****************************************************************************
//* 
//*   Unit Item Tables
//* 
//****************************************************************************
//****************************************************************************
//* 
//*   Sounds
//* 
//****************************************************************************
function InitSounds takes nothing returns nothing

endfunction

//****************************************************************************
//* 
//*   Destructible Objects
//* 
//****************************************************************************
function CreateAllDestructables takes nothing returns nothing
local real life
endfunction

//****************************************************************************
//* 
//*   Item Creation
//* 
//****************************************************************************
function CreateAllItems takes nothing returns nothing
endfunction

//****************************************************************************
//* 
//*   Unit Creation
//* 
//****************************************************************************
function CreateAllUnits takes nothing returns nothing
 local unit u
 local integer unitID
 local trigger t
 local real life
endfunction

//****************************************************************************
//* 
//*   Regions
//* 
//****************************************************************************
function CreateRegions takes nothing returns nothing
local weathereffect we

endfunction

//****************************************************************************
//* 
//*   Cameras
//* 
//****************************************************************************
function CreateCameras takes nothing returns nothing

endfunction

//****************************************************************************
//* 
//*   Custom Script Code
//* 
//****************************************************************************
//****************************************************************************
//* 
//*   Triggers
//* 
//****************************************************************************
//*  Trigger Untitled_Trigger
//****************************************************************************
function Trig_Untitled_Trigger_Actions takes nothing returns nothing
 local integer i_BT
 local integer j_BT
	set udl_int_0=0
	set i_BT=0
	loop
	exitwhen i_BT > 5
	set j_BT=0
	loop
	exitwhen j_BT > 6
	set s__udl_int0_0[(i_BT)*(6)+j_BT]= 42
	set j_BT=j_BT + 1
	endloop
	set i_BT=i_BT + 1
	endloop
	set udl_unitgrp_0=CreateGroup()
	set i_BT=0
	loop
	exitwhen i_BT > 3
	set s__udl_unitgrp0_0[i_BT]= CreateGroup()
	set i_BT=i_BT + 1
	endloop
	set i_BT=0
	loop
	exitwhen i_BT > 3
	set j_BT=0
	loop
	exitwhen j_BT > 4
	set s__udl_unitgrp1_0[(i_BT)*(4)+j_BT]= CreateGroup()
	set j_BT=j_BT + 1
	endloop
	set i_BT=i_BT + 1
	endloop
	set i_BT=0
	loop
	exitwhen i_BT > 3
	set j_BT=0
	loop
	exitwhen j_BT > 4
	set s__udl_playergrp_0[(i_BT)*(4)+j_BT]= CreateForce()
	set j_BT=j_BT + 1
	endloop
	set i_BT=i_BT + 1
	endloop
	set i_BT=0
	loop
	exitwhen i_BT > 3
	set j_BT=0
	loop
	exitwhen j_BT > 4
	set s__udl_ability_0[(i_BT)*(4)+j_BT]= null
	set j_BT=j_BT + 1
	endloop
	set i_BT=i_BT + 1
	endloop
	set i_BT=0
	loop
	exitwhen i_BT > 3
	set j_BT=0
	loop
	exitwhen j_BT > 4
	set s__udl_abilitycode_0[(i_BT)*(4)+j_BT]= 0
	set j_BT=j_BT + 1
	endloop
	set i_BT=i_BT + 1
	endloop
	set i_BT=0
	loop
	exitwhen i_BT > 3
	set j_BT=0
	loop
	exitwhen j_BT > 4
	set s__udl_abilitycode0_0[(i_BT)*(4)+j_BT]= 'AHbz'
	set j_BT=j_BT + 1
	endloop
	set i_BT=i_BT + 1
	endloop
endfunction


//****************************************************************************
function InitTrig_Untitled_Trigger takes nothing returns nothing
	set gg_trg_Untitled_Trigger=CreateTrigger()
	call TriggerAddAction(gg_trg_Untitled_Trigger, function Trig_Untitled_Trigger_Actions)
endfunction

//****************************************************************************
//* 
//*   Triggers
//* 
//****************************************************************************
function InitCustomTriggers takes nothing returns nothing
	call InitTrig_Untitled_Trigger()
endfunction


function RunInitializationTriggers takes nothing returns nothing
endfunction

//****************************************************************************
//* 
//*   Players
//* 
//****************************************************************************
function InitCustomPlayerSlots takes nothing returns nothing
	call SetPlayerStartLocation(Player(0), 0)
	call SetPlayerColor(Player(0), ConvertPlayerColor(0))
	call SetPlayerRacePreference(Player(0), RACE_PREF_HUMAN)
	call SetPlayerRaceSelectable(Player(0), false)
	call SetPlayerController(Player(0), MAP_CONTROL_USER)

endfunction

//****************************************************************************
//* 
//*   Custom Teams
//* 
//****************************************************************************
function InitCustomTeams takes nothing returns nothing

	//*  Force: TRIGSTR_002
	call SetPlayerTeam(Player(0), 0)

endfunction

function InitAllyPriorities takes nothing returns nothing
	call SetStartLocPrioCount(0, 0)
endfunction

//****************************************************************************
//* 
//*   Main Initialization
//* 
//****************************************************************************
function main takes nothing returns nothing
	call SetCameraBounds(- 3328 + GetCameraMargin(CAMERA_MARGIN_LEFT), - 3584 + GetCameraMargin(CAMERA_MARGIN_BOTTOM), 3328 - GetCameraMargin(CAMERA_MARGIN_RIGHT), 3072 - GetCameraMargin(CAMERA_MARGIN_TOP), - 3328 + GetCameraMargin(CAMERA_MARGIN_LEFT), 3072 - GetCameraMargin(CAMERA_MARGIN_TOP), 3328 - GetCameraMargin(CAMERA_MARGIN_RIGHT), - 3584 + GetCameraMargin(CAMERA_MARGIN_BOTTOM))
	call SetDayNightModels("Environment\\DNC\\DNCLordaeron\\DNCLordaeronTerrain\\DNCLordaeronTerrain.mdl", "Environment\\DNC\\DNCLordaeron\\DNCLordaeronUnit\\DNCLordaeronUnit.mdl")
	call NewSoundEnvironment("")
	call SetAmbientDaySound("LordaeronSummerDay")
	call SetAmbientNightSound("LordaeronSummerNight")
	call SetMapMusic("Music", true, 0)
	call InitSounds()
	call CreateRegions()
	call CreateCameras()
	call CreateAllDestructables()
	call CreateAllItems()
	call CreateAllUnits()
	call InitBlizzard()

call ExecuteFunc("jasshelper__initstructs209145093")

	call InitGlobals()
	call InitTrig_Untitled_Trigger() // INLINED!!
	call RunInitializationTriggers()
endfunction

//****************************************************************************
//* 
//*   Map Configuration
//* 
//****************************************************************************
function config takes nothing returns nothing
	call SetMapName("Just another Warcraft III map")
	call SetMapDescription("Nondescript")
	call SetPlayers(1)
	call SetTeams(1)
	call SetGamePlacement(MAP_PLACEMENT_TEAMS_TOGETHER)

	call DefineStartLocation(0, - 64, 1024)
	call DefineStartLocation(0, - 64, 1024)

	call InitCustomPlayerSlots()
	call SetPlayerSlotAvailable(Player(0), MAP_CONTROL_USER)
	call InitGenericPlayerSlots()
	call SetStartLocPrioCount(0, 0) // INLINED!!
endfunction




//Struct method generated initializers/callers:

//Functions for BigArrays:

function jasshelper__initstructs209145093 takes nothing returns nothing







endfunction

