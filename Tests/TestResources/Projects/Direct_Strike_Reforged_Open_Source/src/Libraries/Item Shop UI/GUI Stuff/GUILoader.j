library TasItemShopGUILoader initializer init_function requires TasItemShop
    private function fusion takes nothing returns nothing
        local integer i = 1
        loop
            exitwhen udg_TasItemShop_Mats[i] == 0
            call TasItemFusionAdd(udg_TasItemShop_Item, udg_TasItemShop_Mats[i])
            set udg_TasItemShop_Mats[i] = 0
            set i = i + 1
        endloop
    endfunction
    private function add takes nothing returns nothing
        call TasItemShopAdd(udg_TasItemShop_Item, udg_TasItemShop_Category)
    endfunction
    private function category takes nothing returns nothing
        set udg_TasItemShop_Category = TasItemShopAddCategory(udg_TasItemShop_Icon, udg_TasItemShop_Text)
    endfunction

    private function createShop takes nothing returns nothing
        local integer i = 1
        call TasItemShopCreateShop(udg_TasItemShop_Unit, udg_TasItemShop_WhiteList, udg_TasItemShop_Gold, udg_TasItemShop_Lumber, null)        
        loop
            exitwhen udg_TasItemShop_Mats[i] == 0
            call TasItemShopAddShop(udg_TasItemShop_Unit, udg_TasItemShop_Mats[i])
            set udg_TasItemShop_Mats[i] = 0
            set i = i + 1
        endloop
        
    endfunction
    private function haggle takes nothing returns nothing
        local integer skill = udg_TasItemShop_Skill
        if skill == 0 then
            set skill = udg_TasItemShop_Buff
        endif
        call TasItemShopAddHaggleSkill(skill, udg_TasItemShop_Gold, udg_TasItemShop_Lumber, udg_TasItemShop_GoldAdd, udg_TasItemShop_LumberAdd)
        
        set udg_TasItemShop_Buff = 0
        set udg_TasItemShop_Skill = 0
    endfunction
    private function costs takes nothing returns nothing
        call TasItemShopGoldFactor(udg_TasItemShop_Unit, udg_TasItemShop_Gold, udg_TasItemShop_Item)
        call TasItemShopLumberFactor(udg_TasItemShop_Unit, udg_TasItemShop_Lumber, udg_TasItemShop_Item)
    endfunction
    private function shortCuts takes nothing returns nothing
        local integer i = 1
        call TasItemShop_ClearQuickLink(udg_TasItemShop_Player)
        loop
            exitwhen udg_TasItemShop_Mats[i] == 0
            call TasItemShop_SetQuickLink(udg_TasItemShop_Player, udg_TasItemShop_Mats[i])
            set udg_TasItemShop_Mats[i] = 0
            set i = i + 1
        endloop
        
    endfunction
    private function init_function takes nothing returns nothing
        set udg_TasItemShopFusion = CreateTrigger()
        set udg_TasItemShopAdd = CreateTrigger()
        set udg_TasItemShopCategory = CreateTrigger()
        set udg_TasItemShopCreateShop = CreateTrigger()
        set udg_TasItemShopHaggle = CreateTrigger()
        set udg_TasItemShopCosts = CreateTrigger()
        set udg_TasItemShopShortCuts = CreateTrigger()

        call TriggerAddAction(udg_TasItemShopFusion, function fusion)
        call TriggerAddAction(udg_TasItemShopAdd, function add)
        call TriggerAddAction(udg_TasItemShopCategory, function category)
        call TriggerAddAction(udg_TasItemShopCreateShop, function createShop)
        call TriggerAddAction(udg_TasItemShopHaggle, function haggle)
        call TriggerAddAction(udg_TasItemShopCosts, function costs)
        call TriggerAddAction(udg_TasItemShopShortCuts, function shortCuts)
    endfunction
endlibrary
