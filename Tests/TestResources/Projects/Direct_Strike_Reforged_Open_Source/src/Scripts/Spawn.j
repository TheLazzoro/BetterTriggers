function UnitDeactivateAbility takes integer playerNum, unit u returns nothing
	local integer unitType = LoadInteger(udg_SpawnTable, 0, GetUnitTypeId(GetEnumUnit()))

	if (udg_activeSlow[playerNum] == false) then
		call IssueImmediateOrderBJ(u, "slowoff")
	endif
	if (udg_activeHeal[playerNum] == false) then
		call IssueImmediateOrderBJ(u, "healoff")
	endif
	if (udg_activeBloodlust[playerNum] == false) then
		call IssueImmediateOrderBJ(u, "bloodlustoff")
	endif
	if (udg_activeEnsnare[playerNum] == false) then
		call IssueImmediateOrderBJ(u, "weboff")
	endif
	if (udg_activeWeb[playerNum] == false) then
		call IssueImmediateOrderBJ(u, "weboff")
	endif
	if (udg_activeCurse[playerNum] == false) then
		call IssueImmediateOrderBJ(u, "curseoff")
	endif
	if (udg_activeEssenceOfBlight[playerNum] == false) then
		call IssueImmediateOrderBJ(u, "replenishlifeoff")
	endif
	if (udg_activeRaiseDead[playerNum] == false) then
		call IssueImmediateOrderBJ(u, "raisedeadoff")
	endif
	if (udg_activeOrbOfAnnihilation[playerNum] == false) then
		call IssueImmediateOrderBJ(u, "unflamingattack")
	endif
	if (udg_activeAbolishMagic[playerNum] == false) then
		call IssueImmediateOrderBJ(u, "autodispeloff")
	endif
	if (udg_activeFaerieFire[playerNum] == false) then
		call IssueImmediateOrderBJ(u, "faeriefireoff")
	endif
	if (udg_activePhaseShift[playerNum] == false) then
		call IssueImmediateOrderBJ(u, "phaseshiftoff")
	endif
endfunction

function BuildingGroupSpawn1 takes nothing returns nothing
	local integer unitType = LoadInteger(udg_SpawnTable, 0, GetUnitTypeId(GetEnumUnit()))
	local location point = GetRectCenter(gg_rct_Team_1_Spawn)
	local location point2 = GetRectCenter(udg_BuildRegion[udg_SpawnPlayers[1]])
	local location point3 = GetUnitLoc(GetEnumUnit())
	local location point4 = PolarProjectionBJ(point, DistanceBetweenPoints(point2, point3), AngleBetweenPoints(point2, point3))
	local integer p = GetConvertedPlayerId(Player(udg_SpawnPlayers[1] - 1))
	local integer r = udg_PlayerRace[p]
	local unit u = null
	local group unitGroup
	local integer i
	local integer itemID
	local item it

	// Units
	if (IsUnitType(GetEnumUnit(), UNIT_TYPE_HERO) == false) then
		call UnitRemoveAbility(GetEnumUnit(), 'A000')
		call UnitAddAbility(GetEnumUnit(), 'A0BK')
		set u = CreateUnitAtLoc(Player(udg_SpawnPlayers[1] + 5), unitType, point4, 180)
		set unitGroup = LoadGroupHandle(udg_UnitTypeTable, unitType, 0)
		call GroupAddUnit(unitGroup, u) // AI unit group
		call UnitDeactivateAbility(udg_SpawnPlayers[1] + 6, u)
	else // Heroes
		if (IsUnitDeadBJ(LoadUnitHandle(udg_HeroTable, 0, GetHandleId(GetEnumUnit()))) == true or IsUnitInGroup(LoadUnitHandle(udg_HeroTable, 0, GetHandleId(GetEnumUnit())), udg_DeadHeroes) == true) then
			set u = LoadUnitHandle(udg_HeroTable, 0, GetHandleId(GetEnumUnit()))
			call ReviveHeroLoc(u, point4, true)
			call GroupRemoveUnit(udg_DeadHeroes, u)
			// activate autocast hero abilites (bug fix)
            
			if(udg_activeSearingArrows[udg_SpawnPlayers[1]] == true) then
				call IssueImmediateOrderBJ( u, "flamingarrows" )
			else
				call IssueImmediateOrderBJ( u, "unflamingarrows" )
			endif
			if(udg_activeFrostShield[udg_SpawnPlayers[1]] == true) then
				call IssueImmediateOrderBJ( u, "frostarmoron" )
			else
				call IssueImmediateOrderBJ( u, "frostarmoroff" )
			endif
			if(udg_activeCarrionBeetles[udg_SpawnPlayers[1]] == true) then
				call IssueImmediateOrderBJ( u, "carrionscarabson" )
			else
				call IssueImmediateOrderBJ( u, "carrionscarabsoff" )
			endif
			if(udg_activeFrostArrows[udg_SpawnPlayers[1]] == true) then
				call IssueImmediateOrderBJ( u, "coldarrows" )
			else
				call IssueImmediateOrderBJ( u, "uncoldarrows" )
			endif
			if(udg_activeBlackArrow[udg_SpawnPlayers[1]] == true) then
				call IssueImmediateOrderBJ( u, "blackarrowon" )
			else
				call IssueImmediateOrderBJ( u, "blackarrowoff" )
			endif
            

			// Items
			set i = 0
			loop
			exitwhen i > 5
				set it = UnitItemInSlot(GetEnumUnit(), i)
				set itemID = GetItemTypeId(it)
                
				set it = UnitItemInSlot(u, i)
				call RemoveItem(it)
				call UnitAddItemToSlotById(u, itemID, i)

				set i = i + 1
			endloop
		endif
	endif

	call UnitMoveLoc(u)
	call RemoveLocation(point)
	call RemoveLocation(point2)
	call RemoveLocation(point3)
	call RemoveLocation(point4)
endfunction

function BuildingGroupSpawn2 takes nothing returns nothing
	local integer unitType = LoadInteger(udg_SpawnTable, 0, GetUnitTypeId(GetEnumUnit()))
	local location point = GetRectCenter(gg_rct_Team_2_Spawn)
	local location point2 = GetRectCenter(udg_BuildRegion[udg_SpawnPlayers[2]])
	local location point3 = GetUnitLoc(GetEnumUnit())
	local location point4 = PolarProjectionBJ(point, DistanceBetweenPoints(point2, point3), AngleBetweenPoints(point2, point3))
	local integer p = GetConvertedPlayerId(Player(udg_SpawnPlayers[2] - 1))
	local integer r = udg_PlayerRace[p]
	local unit u = null
	local group unitGroup
	local integer i
	local integer itemID
	local item it

	if (IsUnitType(GetEnumUnit(), UNIT_TYPE_HERO) == false) then
		call UnitRemoveAbility(GetEnumUnit(), 'A000')
		call UnitAddAbility(GetEnumUnit(), 'A0BK')
		set u = CreateUnitAtLoc(Player(udg_SpawnPlayers[2] + 5), unitType, point4, 0)
		set unitGroup = LoadGroupHandle(udg_UnitTypeTable, unitType, 0)
		call GroupAddUnit(unitGroup, u)
		call UnitDeactivateAbility(udg_SpawnPlayers[2] + 6, u)
	else
		if (IsUnitDeadBJ(LoadUnitHandle(udg_HeroTable, 0, GetHandleId(GetEnumUnit()))) == true or IsUnitInGroup(LoadUnitHandle(udg_HeroTable, 0, GetHandleId(GetEnumUnit())), udg_DeadHeroes) == true) then
			set u = LoadUnitHandle(udg_HeroTable, 0, GetHandleId(GetEnumUnit()))
			call ReviveHeroLoc(u, point4, true)
			call GroupRemoveUnit(udg_DeadHeroes, u)
			// activate autocast hero abilites (bug fix)
            
			if(udg_activeSearingArrows[udg_SpawnPlayers[2]] == true) then
				call IssueImmediateOrderBJ( u, "flamingarrows" )
			else
				call IssueImmediateOrderBJ( u, "unflamingarrows" )
			endif
			if(udg_activeFrostShield[udg_SpawnPlayers[2]] == true) then
				call IssueImmediateOrderBJ( u, "frostarmoron" )
			else
				call IssueImmediateOrderBJ( u, "frostarmoroff" )
			endif
			if(udg_activeCarrionBeetles[udg_SpawnPlayers[2]] == true) then
				call IssueImmediateOrderBJ( u, "carrionscarabson" )
			else
				call IssueImmediateOrderBJ( u, "carrionscarabsoff" )
			endif
			if(udg_activeFrostArrows[udg_SpawnPlayers[2]] == true) then
				call IssueImmediateOrderBJ( u, "coldarrows" )
			else
				call IssueImmediateOrderBJ( u, "uncoldarrows" )
			endif
			if(udg_activeBlackArrow[udg_SpawnPlayers[2]] == true) then
				call IssueImmediateOrderBJ( u, "blackarrowon" )
			else
				call IssueImmediateOrderBJ( u, "blackarrowoff" )
			endif

			// Items
			set i = 0
			loop
			exitwhen i > 5
				set it = UnitItemInSlot(GetEnumUnit(), i)
				set itemID = GetItemTypeId(it)
                
				set it = UnitItemInSlot(u, i)
				call RemoveItem(it)
				call UnitAddItemToSlotById(u, itemID, i)

				set i = i + 1
			endloop
		endif
	endif

	call UnitMoveLoc(u)
	call RemoveLocation(point)
	call RemoveLocation(point2)
	call RemoveLocation(point3)
	call RemoveLocation(point4)
endfunction

function SpawnUnits takes nothing returns nothing
	call ForGroup(udg_UnitGroupBuildings[1], function BuildingGroupSpawn1)
	call ForGroup(udg_UnitGroupBuildings[2], function BuildingGroupSpawn2)
endfunction