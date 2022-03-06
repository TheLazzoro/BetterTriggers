globals
//globals from DamageDetection:
constant boolean LIBRARY_DamageDetection=true
//endglobals from DamageDetection
real array udg_TotalDamage

trigger l__library_init

//JASSHelper struct globals:

endglobals


//library DamageDetection:
    
   function DamageDetectCOND takes nothing returns boolean
      if ( GetConvertedPlayerId(GetOwningPlayer(GetEventDamageSource())) <= 12 ) then
         return true
      endif
      return false
   endfunction
   
   function DamageDetect takes nothing returns nothing
      local integer p= GetConvertedPlayerId(GetOwningPlayer(GetEventDamageSource())) - 6
      set udg_TotalDamage[p]=udg_TotalDamage[p] + GetEventDamage()
   endfunction
   
   function SetupDamageDetect takes nothing returns nothing
      local trigger trig= CreateTrigger()
      call TriggerRegisterAnyUnitEventBJ(trig, EVENT_PLAYER_UNIT_DAMAGED)
      call TriggerAddCondition(trig, Condition(function DamageDetectCOND))
      call TriggerAddAction(trig, function DamageDetect)
   endfunction
   

//library DamageDetection ends

function main takes nothing returns nothing


call ExecuteFunc("SetupDamageDetect")

endfunction



//Struct method generated initializers/callers:

