library IsRightClick initializer Init requires optional FrameLoader
// function IsRightClick takes player p returns boolean
// tells you if the last click was current a rightclick, meant to be used inside a FrameEventMouseUp
    globals
        private boolean array isRight
        private trigger frameTrigger
    endglobals
    function IsRightClick takes player p returns boolean
        return isRight[GetPlayerId(p)]
    endfunction
    private function Action takes nothing returns nothing
        set isRight[GetPlayerId(GetTriggerPlayer())] = BlzGetTriggerPlayerMouseButton() == MOUSE_BUTTON_TYPE_RIGHT
    endfunction
    private function FrameAction takes nothing returns nothing
        // when the game is paused reset the lastClick Flag. This has to be done because EVENT_PLAYER_MOUSE_UP does not trigger during Pause
        local integer i = 0
        loop
            set isRight[i] = false
            set i = i + 1
            exitwhen i == bj_MAX_PLAYERS
        endloop
    endfunction
    private function FrameInit takes nothing returns nothing
        call BlzTriggerRegisterFrameEvent(frameTrigger, BlzGetFrameByName("PauseButton", 0), FRAMEEVENT_CONTROL_CLICK)
    endfunction
    private function Init takes nothing returns nothing
        local trigger t = CreateTrigger()
        local integer i = 0
        set frameTrigger = CreateTrigger()
        call TriggerAddAction(frameTrigger, function FrameAction)
        call TriggerAddAction(t, function Action)
        loop
            call TriggerRegisterPlayerEvent(t, Player(i), EVENT_PLAYER_MOUSE_UP)
            set i = i + 1
            exitwhen i == bj_MAX_PLAYERS
        endloop
        static if LIBRARY_FrameLoader then
            call FrameLoaderAdd(function FrameInit)
        endif
	call FrameLoaderAdd(function FrameInit)
    endfunction
endlibrary