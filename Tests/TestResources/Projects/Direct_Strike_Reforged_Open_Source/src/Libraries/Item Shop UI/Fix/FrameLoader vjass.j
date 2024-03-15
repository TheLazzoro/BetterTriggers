library FrameLoader initializer init_function
// in 1.31 and upto 1.32.9 PTR (when I wrote this). Frames are not correctly saved and loaded, breaking the game.
// This library runs all functions added to it with a 0s delay after the game was loaded.
// function FrameLoaderAdd takes code func returns nothing
    // func runs when the game is loaded.
    globals
        private trigger eventTrigger = CreateTrigger()
        private trigger actionTrigger = CreateTrigger()
        private timer t = CreateTimer()
    endglobals
    function FrameLoaderAdd takes code func returns nothing
        call TriggerAddAction(actionTrigger, func)
    endfunction

    private function timerAction takes nothing returns nothing
        call TriggerExecute(actionTrigger)
    endfunction
    private function eventAction takes nothing returns nothing
        call TimerStart(t, 0, false, function timerAction)
    endfunction
    private function init_function takes nothing returns nothing
        call TriggerRegisterGameEvent(eventTrigger, EVENT_GAME_LOADED)
        call TriggerAddAction(eventTrigger, function eventAction)        
    endfunction
endlibrary
