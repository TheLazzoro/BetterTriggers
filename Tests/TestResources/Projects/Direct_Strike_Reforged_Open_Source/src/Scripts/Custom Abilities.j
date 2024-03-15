function Invisibility takes nothing returns nothing
	local unit target = GetSpellTargetUnit()
	call UnitAddAbilityBJ( 'A01D', target)
	call TriggerSleepAction(2)
	call UnitRemoveAbilityBJ( 'A01D', target)
endfunction