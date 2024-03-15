function UnitSell takes real costFraction returns nothing
	local integer p = GetConvertedPlayerId(GetOwningPlayer(GetTriggerUnit()))
	local integer unitType = GetUnitTypeId(GetTriggerUnit())
	local integer cost = R2I(GetUnitGoldCost(unitType) * costFraction)
	local location loc = GetUnitLoc(GetTriggerUnit())
	local texttag tt
	call AdjustPlayerStateBJ(cost, Player(p - 1), PLAYER_STATE_RESOURCE_GOLD)
	set tt = CreateTextTagLocBJ("+" + I2S(cost), loc, 10, 10, 100, 100, 0, 0)
	call SetTextTagVelocityBJ(tt, 20, 90)
	call SetTextTagPermanent(tt, false)
	call SetTextTagLifespan(tt, 2.00)

	call RemoveLocation(loc)
endfunction


function BuildHero takes nothing returns nothing
	// This function will spawn and kill the hero linked to the building.
	local integer p = GetConvertedPlayerId(GetOwningPlayer(GetTriggerUnit()))
	local integer unitTypeBuilding = GetUnitTypeId(GetTriggerUnit())
	local integer unitType = LoadInteger(udg_SpawnTable, 0, GetUnitTypeId(GetTriggerUnit()))
	local location point = GetRectCenter(gg_rct_UnitLoad)
	local location point2 = GetUnitLoc(GetTriggerUnit())
	local unit ub
	local unit u
	local group unitGroup
	local integer customValue

	call ShowUnit(GetTriggerUnit(), false)
	call ShowUnit(GetTriggerUnit(), true)
	set ub = GetTriggerUnit()
	set u = CreateUnitAtLoc(Player(p + 5), unitType, point, 270)
	call SaveUnitHandle(udg_HeroTable, 0, GetHandleId(ub), u)
	call SaveUnitHandle(udg_HeroTable, GetHandleId(u), 0, ub)
	call GroupAddUnit(udg_Heroes, u)
	call GroupAddUnit(udg_HeroBuildings, ub)
	set unitGroup = LoadGroupHandle(udg_UnitTypeTable, unitType, 0)
	call GroupAddUnit(unitGroup, u)
	call KillUnit(u)

	call RemoveLocation(point)
	call RemoveLocation(point2)
endfunction