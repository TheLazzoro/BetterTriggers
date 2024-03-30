library ItemHolder initializer init_function requires Table
    globals
        private Table data
    endglobals
    public function get takes item i returns unit
        return data.unit[GetHandleId(i)]
    endfunction
    private function gain takes nothing returns nothing
        set data.unit[GetHandleId(GetManipulatedItem())] = GetTriggerUnit()

    endfunction
    private function lose takes nothing returns nothing
        set data.unit[GetHandleId(GetManipulatedItem())] = null
    endfunction
    private function init_function takes nothing returns nothing
        local trigger t
        set data = Table.create()
        set t = CreateTrigger()
        call TriggerAddAction(t, function gain)
        call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_PICKUP_ITEM)

        set t = CreateTrigger()
        call TriggerAddAction(t, function lose)
        call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_DROP_ITEM)
    endfunction
endlibrary
