library TasItemShopUserInit initializer TasItemShopUserInit requires TasItemShop
    // This script  is meant to be used by vjass user to write init data for TasItemShop
    
        private function ShopCostFunction_ngme takes nothing returns nothing
        endfunction
         // This runs right before the actually UI is created.
        // this is a good place to add items, categories, fusions shops etc.
        function TasItemShopUserInit takes nothing returns nothing
            local integer shopObject
            // this can all be done in GUI aswell, enable the next Line or remove all Text of this function if you only want to use GUI
            //if true then return end
    
            // define Categories: Icon, Text
            // the Categories are displayed in the order added.
            // it is a good idea to save the returned Value in a local to make the category setup later much easier to understand.
            // you can only have 31 categories
            local integer catDmg = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNSteelMelee", "Damage")
            local integer catArmor = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNHumanArmorUpOne", "Armor")
            local integer catStr = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNGauntletsOfOgrePower", "STRENGTH")
            local integer catAgi = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNSlippersOfAgility", "AGILITY")
            local integer catInt = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNMantleOfIntelligence", "INTELLECT")
            local integer catLife = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNPeriapt", "Life")
            local integer catLifeReg = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNRegenerate", "Life Regeneration")
            local integer catMana = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNPendantOfMana", "Mana")
            local integer catManaReg = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNSobiMask", "Mana Regeneration")
            local integer catOrb = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNOrbOfDarkness", "Orb")
            local integer catAura = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNLionHorn", "Aura")
            local integer catActive = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNStaffOfSilence", "Active")
            // local integer catPower = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNControlMagic", "SpellPower")
            // local integer catCooldown = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNHumanMissileUpOne", "Cooldown")
            local integer catAtkSpeed = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNHumanMissileUpOne", "Attack Speed")
            local integer catMress = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNRunedBracers", "Magic-Resistence")
            //local integer catConsum = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNPotionGreenSmall", "Consumable")
            local integer catMoveSpeed = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNBootsOfSpeed", "Movement Speed")
            // local integer catCrit = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNCriticalStrike", "Crit")
            local integer catLifeSteal = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNVampiricAura", "Lifesteal")
            local integer catEvade = TasItemShopAddCategory("ReplaceableTextures\\CommandButtons\\BTNEvasion", "Evasion")
            
            
            call TasItemShopAdd('afac', catDmg + catAura)
            call TasItemShopAdd('spsh', catMress)
            call TasItemShopAdd('ajen', catAtkSpeed + catMoveSpeed + catAura)
            call TasItemShopAdd('bgst', catStr)
            call TasItemShopAdd('belv', catAgi)
            call TasItemShopAdd('cnob', catStr + catAgi + catInt)
            call TasItemShopAdd('ratc', catDmg)
            call TasItemShopAdd('clfm', catDmg + catActive)
            //call TasItemShopAdd('rst1', catStr)
            call TasItemShopAdd('gcel', catAtkSpeed)
            call TasItemShopAdd('hval', catStr + catAgi)
            call TasItemShopAdd('hcun', catAgi + catInt)
            call TasItemShopAdd('rhth', catLife)
            call TasItemShopAdd('kpin', catManaReg + catAura)
            call TasItemShopAdd('lgdh', catLifeReg + catMoveSpeed + catAura)
            //call TasItemShopAdd('rin1', catInt)
            call TasItemShopAdd('mcou', catStr + catInt)
            call TasItemShopAdd('odef', catDmg + catOrb)
            call TasItemShopAdd('pmna', catMana)
            //call TasItemShopAdd('rde1', catArmor)
            //call TasItemShopAdd('rde2', catArmor)
            call TasItemShopAdd('rde3', catArmor)
            call TasItemShopAdd('rlif', catLifeReg)
            call TasItemShopAdd('ciri', catInt)
            call TasItemShopAdd('brac', catMress)
            call TasItemShopAdd('sbch', catLifeSteal + catAura)
            //call TasItemShopAdd('rag1', catAgi)
            call TasItemShopAdd('rwiz', catManaReg)
            //call TasItemShopAdd('ssil', catActive)
            call TasItemShopAdd('evtl', catEvade)
            call TasItemShopAdd('lhst', catArmor + catAura)
            call TasItemShopAdd('ward', catDmg + catAura)
            call TasItemShopAdd('desc', catActive)
            call TasItemShopAdd('gemt', catActive)
            call TasItemShopAdd('ocor', catDmg + catOrb)
            call TasItemShopAdd('ofir', catDmg + catOrb)
            call TasItemShopAdd('oli2', catDmg + catOrb)
            call TasItemShopAdd('oslo', catDmg + catOrb)
            call TasItemShopAdd('oven', catDmg + catOrb)
    
    
            // setup custom shops
            // custom Shops are optional.
            // They can have a White or Blacklist of items they can(n't) sell and have a fixed cost modifier for Gold, Lumber aswell as a function for more dynamic things for Gold and Lumber.
            set shopObject = 'n000'
            // 'n000' can only sell this items (this items don't have to be in the pool of items)

            // enable WhiteListMode
            call TasItemShopSetMode(shopObject, true)
            
            // 'n001' can't sell this items (from the default pool of items)
            set shopObject = 'n001'

            // enable BlackListMode
            call TasItemShopSetMode(shopObject, false)
            
            // create an shopObject for 'ngme', has to pay 20% more than normal, beaware that this can be overwritten by GUI Example
            call TasItemShopCreateShop('ngme', false, 1.0, 1.0, function ShopCostFunction_ngme)
            //'I002' crown +100 was never added to the database but this shop can craft/sell it.
            set shopObject = 'n002'
            
    
            // Define skills/Buffs that change the costs in the shop
            // cursed Units have to pay +25%
            call TasItemShopAddHaggleSkill('Bcrs', 1.25, 1.25, 0, 0)
    
            // define Fusions
            // result created by 'xxx', 'xx' , 'x'+.
            // item can only be crafted by one way
            // can add any amount of material in the Lua version
            //call TasItemFusionAdd2('bgst', 'rst1', 'rst1')
            //call TasItemFusionAdd2('ciri', 'rin1', 'rin1')
            //call TasItemFusionAdd2('belv', 'rag1', 'rag1')
            //call TasItemFusionAdd2('hval', 'rag1', 'rst1')
            //call TasItemFusionAdd2('hcun', 'rag1', 'rin1')
            //call TasItemFusionAdd2('mcou', 'rst1', 'rin1')
            //call TasItemFusionAdd2('ckng', 'cnob', 'cnob')
            //call TasItemFusionAdd('rde4', 'rde3')
            //call TasItemFusionAdd('rde3', 'rde2')
            //call TasItemFusionAdd('rhth', 'prvt')
            //call TasItemFusionAdd('pmna', 'penr')
            //call TasItemFusionAdd2('arsh', 'rde3', 'rde2')
    
            //call TasItemFusionAdd('lhst', 'sfog')
    
            //// crown of Kings + 50
            //call TasItemFusionAdd4('I001', 'ckng', 'ckng', 'ckng', 'ckng')
            //call TasItemFusionAdd4('I001', 'ckng', 'ckng', 'bgst', 'bgst')
            //call TasItemFusionAdd6('I001', 'ciri', 'ciri', 'belv', 'belv', 'cnob', 'cnob')
            //// crown of Kings + 100, this is a joke you can not craft it because it was not added to buyAble Items
            //call TasItemFusionAdd2('I002', 'I001', 'I001')
    
    
            //call TasItemFusionAdd('modt', 'rst1')
            //call TasItemFusionAdd('ofro', 'rst1')
            //call TasItemFusionAdd('thdm', 'rst1')
            //call TasItemFusionAdd('hlst', 'rst1')
            //call TasItemFusionAdd('mnst', 'rst1')
            //call TasItemFusionAdd('ocor', 'rst1')
    
            // define item Categories
            // uses the locals from earlier.
            // An item can have multiple categories just add them together like this: catStr + catAgi + catInt
            
            
        endfunction
    endlibrary