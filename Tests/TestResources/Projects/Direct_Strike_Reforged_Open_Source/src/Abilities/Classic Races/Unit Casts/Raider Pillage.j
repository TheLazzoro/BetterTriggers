library RaiderPillage initializer init
    
    private function PillageCOND takes nothing returns boolean
        if(GetUnitTypeId(GetEventDamageSource()) == UNIT_RAIDER and IsUnitType(BlzGetEventDamageTarget(), UNIT_TYPE_STRUCTURE) == true) then
            return true
        endif

        return false
    endfunction

    function PillageACTION takes nothing returns nothing
        local location loc = GetUnitLoc(GetEventDamageSource())
        local integer gold = 10
        local integer p = GetConvertedPlayerId(GetOwningPlayer(GetEventDamageSource())) - 6
        call AdjustPlayerStateBJ( gold, Player(p - 1), PLAYER_STATE_RESOURCE_GOLD )

        call CreateTextTagLocBJ( "+" + I2S(gold), loc, 0, 10, 100, 100, 0.00, 0 )
        call SetTextTagVelocityBJ( GetLastCreatedTextTag(), 64, 90 )
        call SetTextTagPermanentBJ( GetLastCreatedTextTag(), false )
        call SetTextTagLifespanBJ( GetLastCreatedTextTag(), 1.00 )

        call RemoveLocation(loc)
    endfunction
    
    private function init takes nothing returns nothing
        local trigger trig = CreateTrigger()
        call TriggerRegisterAnyUnitEventBJ(trig, EVENT_PLAYER_UNIT_DAMAGED)
        call TriggerAddCondition(trig, Condition(function PillageCOND))
        call TriggerAddAction(trig, function PillageACTION)   
    
        set trig = null
    endfunction
endlibrary
