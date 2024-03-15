library Power2 initializer init_function
    globals
        private integer array data
    endglobals
    function GetPower2Value takes integer i returns integer
        return data[i]
    endfunction
    private function init_function takes nothing returns nothing
        local integer index = 1
        local integer value = 1
        loop
            set data[index] = value
            set index = index + 1
            set value = value*2
            exitwhen index == 32
            // body
        endloop
    endfunction
endlibrary
 