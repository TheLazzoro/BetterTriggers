globals 
    framehandle btnAbilityControl
    framehandle array abilityMenu
    framehandle frameHeroAbilityText
    framehandle array btnNeutralHeroes
    framehandle neutralAbilityMenu

    framehandle array btnAbilityEnabled[4][27] // (race, ability)
    string array btnAbilityImgEnabled[4][27]
    framehandle array btnAbilityDisabled[4][27] // (race, ability)
    string array btnAbilityImgDisabled[4][27]
    framehandle array frameAbilityName
    framehandle array frameAbilityState
    string array abilityName[4][27]

    framehandle array btnNeutralAbilityEnabled
    string array btnNeutralAbilityImgEnabled
    framehandle array btnNeutralAbilityDisabled
    string array btnNeutralAbilityImgDisabled
    string array neutralAbilityName
    framehandle frameNeutralAbilityState
    framehandle frameNeutralAbilityName

    boolean gl_state
    integer gl_player

    string tempOrder
endglobals

function MenuAbilities takes nothing returns nothing
    local integer p = GetConvertedPlayerId(GetTriggerPlayer())
    local integer r = udg_PlayerRace[p]

    if (GetLocalPlayer() == GetTriggerPlayer()) then
        call BlzFrameSetEnable(btnAbilityControl, false)
        call BlzFrameSetEnable(btnAbilityControl, true)
    endif

    if (isVisibleAbilityMenu[p] == false) then
        set isVisibleAbilityMenu[p] = true
        if (GetLocalPlayer() == GetTriggerPlayer()) then
            call BlzFrameSetVisible(abilityMenu[r], true)
        endif
    else
        set isVisibleAbilityMenu[p] = false
        set isVisibleNeutralAbilityMenu[p] = false
        if (GetLocalPlayer() == GetTriggerPlayer()) then
            call BlzFrameSetVisible(abilityMenu[r], false)
            call BlzFrameSetVisible(neutralAbilityMenu, false)
        endif
    endif
endfunction

function MenuNeutralAbilities takes nothing returns nothing
    local integer p = GetConvertedPlayerId(GetTriggerPlayer())

    if (GetLocalPlayer() == GetTriggerPlayer()) then
        call BlzFrameSetEnable(BlzGetTriggerFrame(), false)
        call BlzFrameSetEnable(BlzGetTriggerFrame(), true)
    endif

    if (isVisibleNeutralAbilityMenu[p] == false) then
        set isVisibleNeutralAbilityMenu[p] = true
        if (GetLocalPlayer() == GetTriggerPlayer()) then
            call BlzFrameSetVisible(neutralAbilityMenu, true)
        endif
    else
        set isVisibleNeutralAbilityMenu[p] = false
        if (GetLocalPlayer() == GetTriggerPlayer()) then
            call BlzFrameSetVisible(neutralAbilityMenu, false)
        endif
    endif
endfunction

function ToggleAutoCastEnum takes nothing returns nothing
    if(Player(gl_player - 1) == GetOwningPlayer(GetEnumUnit())) then
        call IssueImmediateOrder(GetEnumUnit(), tempOrder)
        call UnitMoveLoc(GetEnumUnit())
    endif
endfunction

function ToggleAutoCast takes group unitGroup, string abilityCode returns nothing
    set tempOrder = abilityCode
    call ForGroup(unitGroup, function ToggleAutoCastEnum)
endfunction

function toggleDefend takes nothing returns nothing
    set udg_activeDefend[gl_player] = gl_state
endfunction

function toggleSlow takes nothing returns nothing
    set udg_activeSlow[gl_player] = gl_state
        if(gl_state == false) then
            call ToggleAutoCast(udg_Sorceress, "slowoff")
        else
            call ToggleAutoCast(udg_Sorceress, "slowon")
        endif
endfunction

function toggleInvisibility takes nothing returns nothing
    set udg_activeInvisibility[gl_player] = gl_state
endfunction

function togglePolymorph takes nothing returns nothing
    set udg_activePolymorph[gl_player] = gl_state
endfunction

function toggleFlare takes nothing returns nothing
    set udg_activeFlare[gl_player] = gl_state
endfunction

function toggleHeal takes nothing returns nothing
    set udg_activeHeal[gl_player] = gl_state
        if(gl_state == false) then
            call ToggleAutoCast(udg_Priests, "healoff")
        else
            call ToggleAutoCast(udg_Priests, "healon")
        endif
endfunction

function toggleDispelMagic takes nothing returns nothing
    set udg_activeDispelMagic[gl_player] = gl_state
endfunction

function toggleInnerFire takes nothing returns nothing
    set udg_activeInnerFire[gl_player] = gl_state
endfunction

function toggleControlMagic takes nothing returns nothing
    set udg_activeControlMagic[gl_player] = gl_state
endfunction

function toggleAerialShackles takes nothing returns nothing
    set udg_activeAerialShackles[gl_player] = gl_state
endfunction

function toggleHolyLight takes nothing returns nothing
    set udg_activeHolyLight[gl_player] = gl_state
endfunction

function toggleDivineShield takes nothing returns nothing
    set udg_activeDivineShield[gl_player] = gl_state
endfunction

function toggleResurrection takes nothing returns nothing
    set udg_activeResurrection[gl_player] = gl_state
endfunction

function toggleBlizzard takes nothing returns nothing
    set udg_activeBlizzard[gl_player] = gl_state
endfunction

function toggleWaterElemental takes nothing returns nothing
    set udg_activeWaterElemental[gl_player] = gl_state
endfunction

function toggleTornado takes nothing returns nothing
    set udg_activeTornado[gl_player] = gl_state
endfunction

function toggleStormBolt takes nothing returns nothing
    set udg_activeStormBolt[gl_player] = gl_state
endfunction

function toggleThunderClap takes nothing returns nothing
    set udg_activeThunderClap[gl_player] = gl_state
endfunction

function toggleAvatar takes nothing returns nothing
    set udg_activeAvatar[gl_player] = gl_state
endfunction

function toggleFlamestrike takes nothing returns nothing
    set udg_activeFlamestrike[gl_player] = gl_state
endfunction

function toggleBanish takes nothing returns nothing
    set udg_activeBanish[gl_player] = gl_state
endfunction

function toggleSiphonMana takes nothing returns nothing
    set udg_activeSiphonMana[gl_player] = gl_state
endfunction

function togglePhoenix takes nothing returns nothing
    set udg_activePhoenix[gl_player] = gl_state
endfunction

//

function toggleBerserk takes nothing returns nothing
    set udg_activeBerserk[gl_player] = gl_state
endfunction

function togglePurge takes nothing returns nothing
    set udg_activePurge[gl_player] = gl_state
endfunction

function toggleLightningShield takes nothing returns nothing
    set udg_activeLightningShield[gl_player] = gl_state
endfunction

function toggleBloodlust takes nothing returns nothing
    set udg_activeBloodlust[gl_player] = gl_state
    if(gl_state == false) then
        call ToggleAutoCast(udg_Shamans, "bloodlustoff")
    else
        call ToggleAutoCast(udg_Shamans, "bloodluston")
    endif
endfunction

function toggleEnsnare takes nothing returns nothing
    set udg_activeEnsnare[gl_player] = gl_state
    if(gl_state == false) then
        call ToggleAutoCast(udg_Raiders, "weboff")
    else
        call ToggleAutoCast(udg_Raiders, "webon")
    endif
endfunction

function toggleSentryWard takes nothing returns nothing
    set udg_activeSentryWard[gl_player] = gl_state
endfunction

function toggleStasisTrap takes nothing returns nothing
    set udg_activeStasisTrap[gl_player] = gl_state
endfunction

function toggleHealingWard takes nothing returns nothing
    set udg_activeHealingWard[gl_player] = gl_state
endfunction

function toggleSpiritLink takes nothing returns nothing
    set udg_activeSpiritLink[gl_player] = gl_state
endfunction

function toggleDisenchant takes nothing returns nothing
    set udg_activeDisenchant[gl_player] = gl_state
endfunction

function toggleAncestralSpirit takes nothing returns nothing
    set udg_activeAncestralSpirit[gl_player] = gl_state
endfunction

function toggleUnstableConcoction takes nothing returns nothing
    set udg_activeUnstableConcoction[gl_player] = gl_state
endfunction

function toggleDevour takes nothing returns nothing
    set udg_activeDevour[gl_player] = gl_state
endfunction

function toggleWindWalk takes nothing returns nothing
    set udg_activeWindWalk[gl_player] = gl_state
endfunction

function toggleMirrorImage takes nothing returns nothing
    set udg_activeMirrorImage[gl_player] = gl_state
endfunction

function toggleBladestorm takes nothing returns nothing
    set udg_activeBladestorm[gl_player] = gl_state
endfunction

function toggleChainLightning takes nothing returns nothing
    set udg_activeChainLightning[gl_player] = gl_state
endfunction

function toggleFirebolt takes nothing returns nothing
    set udg_activeFirebolt[gl_player] = gl_state
endfunction

function toggleSpiritWolves takes nothing returns nothing
    set udg_activeSpiritWolves[gl_player] = gl_state
endfunction

function toggleEarthquake takes nothing returns nothing
    set udg_activeEarthquake[gl_player] = gl_state
endfunction

function toggleShockwave takes nothing returns nothing
    set udg_activeShockwave[gl_player] = gl_state
endfunction

function toggleWarStomp takes nothing returns nothing
    set udg_activeWarStomp[gl_player] = gl_state
endfunction

function toggleHealingWave takes nothing returns nothing
    set udg_activeHealingWave[gl_player] = gl_state
endfunction

function toggleHex takes nothing returns nothing
    set udg_activeHex[gl_player] = gl_state
endfunction

function toggleSerpentWard takes nothing returns nothing
    set udg_activeSerpentWard[gl_player] = gl_state
endfunction

function toggleBigBadVoodoo takes nothing returns nothing
    set udg_activeBigBadVoodoo[gl_player] = gl_state
endfunction

//
        
function toggleWeb takes nothing returns nothing
    set udg_activeWeb[gl_player] = gl_state
    if(gl_state == false) then
        call ToggleAutoCast(udg_CryptFiends, "weboff")
    else
        call ToggleAutoCast(udg_CryptFiends, "webon")
    endif
endfunction

function toggleBurrow takes nothing returns nothing
    set udg_activeBurrow[gl_player] = gl_state
endfunction

function toggleCurse takes nothing returns nothing
    set udg_activeCurse[gl_player] = gl_state
        if(gl_state == false) then
            call ToggleAutoCast(udg_Banshees, "curseoff")
        else
            call ToggleAutoCast(udg_Banshees, "curseon")
        endif
endfunction

function toggleAntiMagicShell takes nothing returns nothing
    set udg_activeAntiMagicShell[gl_player] = gl_state
endfunction

function togglePossession takes nothing returns nothing
    set udg_activePossession[gl_player] = gl_state
endfunction

function toggleSpiritTouch takes nothing returns nothing
    set udg_activeSpiritTouch[gl_player] = gl_state
endfunction

function toggleEssenceOfBlight takes nothing returns nothing
    set udg_activeEssenceOfBlight[gl_player] = gl_state
    if(gl_state == false) then
        call ToggleAutoCast(udg_ObsidianStatues, "replenishlifeoff")
    else
        call ToggleAutoCast(udg_ObsidianStatues, "replenishlifeon")
    endif
endfunction

function toggleStoneForm takes nothing returns nothing
    set udg_activeStoneForm[gl_player] = gl_state
endfunction

function toggleRaiseDead takes nothing returns nothing
    set udg_activeRaiseDead[gl_player] = gl_state
        if(gl_state == false) then
            call ToggleAutoCast(udg_Necromancers, "raisedeadoff")
        else
            call ToggleAutoCast(udg_Necromancers, "raisedeadon")
        endif
endfunction

function toggleUnholyFrenzy takes nothing returns nothing
    set udg_activeUnholyFrenzy[gl_player] = gl_state
endfunction

function toggleCripple takes nothing returns nothing
    set udg_activeCripple[gl_player] = gl_state
endfunction

function toggleDevourMagic takes nothing returns nothing
    set udg_activeDevourMagic[gl_player] = gl_state
endfunction

function toggleOrbOfAnnihilation takes nothing returns nothing
    set udg_activeOrbOfAnnihilation[gl_player] = gl_state
        if(gl_state == false) then
            call ToggleAutoCast(udg_Destroyers, "unflamingattack")
        else
            call ToggleAutoCast(udg_Destroyers, "flamingattack")
        endif
endfunction

function toggleAbsorbMana takes nothing returns nothing
    set udg_activeAbsorbMana[gl_player] = gl_state
endfunction

function toggleDeathCoil takes nothing returns nothing
    set udg_activeDeathCoil[gl_player] = gl_state
endfunction

function toggleDeathPact takes nothing returns nothing
    set udg_activeDeathPact[gl_player] = gl_state
endfunction

function toggleAnimateDead takes nothing returns nothing
    set udg_activeAnimateDead[gl_player] = gl_state
endfunction

function toggleFrostNova takes nothing returns nothing
    set udg_activeFrostNova[gl_player] = gl_state
endfunction

function toggleFrostShield takes nothing returns nothing
    set udg_activeFrostShield[gl_player] = gl_state
        if(gl_state == false) then
            call ToggleAutoCast(udg_Liches, "frostarmoroff")
        else
            call ToggleAutoCast(udg_Liches, "frostarmoron")
        endif
endfunction

function toggleDarkRitual takes nothing returns nothing
    set udg_activeDarkRitual[gl_player] = gl_state
endfunction

function toggleDeathAndDecay takes nothing returns nothing
    set udg_activeDeathAndDecay[gl_player] = gl_state
endfunction

function toggleCarrionSwarm takes nothing returns nothing
    set udg_activeCarrionSwarm[gl_player] = gl_state
endfunction

function toggleSleep takes nothing returns nothing
    set udg_activeSleep[gl_player] = gl_state
endfunction

function toggleInferno takes nothing returns nothing
    set udg_activeInferno[gl_player] = gl_state
endfunction

function toggleImpale takes nothing returns nothing
    set udg_activeImpale[gl_player] = gl_state
endfunction

function toggleCarrionBeetles takes nothing returns nothing
    set udg_activeCarrionBeetles[gl_player] = gl_state
    if(gl_state == false) then
        call ToggleAutoCast(udg_CryptLords, "Carrionscarabsoff")
    else
        call ToggleAutoCast(udg_CryptLords, "Carrionscarabson")
    endif
endfunction

function toggleLocustSwarm takes nothing returns nothing
    set udg_activeLocustSwarm[gl_player] = gl_state
endfunction


//

function toggleSentinel takes nothing returns nothing
    set udg_activeSentinel[gl_player] = gl_state
endfunction

function toggleAbolishMagic takes nothing returns nothing
set udg_activeAbolishMagic[gl_player] = gl_state
    if(gl_state == false) then
        call ToggleAutoCast(udg_Dryads, "autodispeloff")
    else
        call ToggleAutoCast(udg_Dryads, "autodispelon")
    endif
endfunction

function toggleRoar takes nothing returns nothing
    set udg_activeRoar[gl_player] = gl_state
endfunction

function toggleRejuvenation takes nothing returns nothing
    set udg_activeRejuvenation[gl_player] = gl_state
endfunction

function toggleBearForm takes nothing returns nothing
    set udg_activeBearForm[gl_player] = gl_state
endfunction

function toggleFaerieFire takes nothing returns nothing
    set udg_activeFaerieFire[gl_player] = gl_state
    if(gl_state == false) then
        call ToggleAutoCast(udg_DruidOfTheTalons, "faeriefireoff")
    else
        call ToggleAutoCast(udg_DruidOfTheTalons, "faeriefireon")
    endif
endfunction

function toggleCrowForm takes nothing returns nothing
    set udg_activeCrowForm[gl_player] = gl_state
endfunction

function toggleCyclone takes nothing returns nothing
    set udg_activeCyclone[gl_player] = gl_state
endfunction

function togglePhaseShift takes nothing returns nothing
    set udg_activePhaseShift[gl_player] = gl_state
    if(gl_state == false) then
        call ToggleAutoCast(udg_FaerieDragons, "phaseshiftoff")
    else
        call ToggleAutoCast(udg_FaerieDragons, "phaseshifton")
    endif
endfunction

function toggleManaFlare takes nothing returns nothing
    set udg_activeManaFlare[gl_player] = gl_state
endfunction

function toggleTaunt takes nothing returns nothing
    set udg_activeTaunt[gl_player] = gl_state
endfunction

function toggleEntanglingRoots takes nothing returns nothing
    set udg_activeEntanglingRoots[gl_player] = gl_state
endfunction

function toggleForceOfNature takes nothing returns nothing
    set udg_activeForceOfNature[gl_player] = gl_state
endfunction

function toggleTranquility takes nothing returns nothing
    set udg_activeTranquility[gl_player] = gl_state
endfunction

function toggleSilence takes nothing returns nothing
    set udg_activeSilence[gl_player] = gl_state
endfunction

function toggleSearingArrows takes nothing returns nothing
    set udg_activeSearingArrows[gl_player] = gl_state
    if(gl_state == false) then
        call ToggleAutoCast(udg_PriestessOfTheMoons, "unflamingarrows")
    else
        call ToggleAutoCast(udg_PriestessOfTheMoons, "flamingarrows")
    endif
endfunction

function toggleStarfall takes nothing returns nothing
    set udg_activeStarfall[gl_player] = gl_state
endfunction

function toggleManaBurn takes nothing returns nothing
    set udg_activeManaBurn[gl_player] = gl_state
endfunction

function toggleImmolation takes nothing returns nothing
    set udg_activeImmolation[gl_player] = gl_state
endfunction

function toggleMetamorphosis takes nothing returns nothing
    set udg_activeMetamorphosis[gl_player] = gl_state
endfunction

function toggleFanOfKnives takes nothing returns nothing
    set udg_activeFanOfKnives[gl_player] = gl_state
endfunction

function toggleBlink takes nothing returns nothing
    set udg_activeBlink[gl_player] = gl_state
endfunction

function toggleShadowStrike takes nothing returns nothing
    set udg_activeShadowStrike[gl_player] = gl_state
endfunction

function toggleVengeance takes nothing returns nothing
    set udg_activeVengeance[gl_player] = gl_state
endfunction

//

function toggleHealingSpray takes nothing returns nothing
    set udg_activeHealingSpray[gl_player] = gl_state
endfunction

function toggleChemicalRage takes nothing returns nothing
    set udg_activeChemicalRage[gl_player] = gl_state
endfunction

function toggleAcidBomb takes nothing returns nothing
    set udg_activeAcidBomb[gl_player] = gl_state
endfunction

function toggleTransmute takes nothing returns nothing
    set udg_activeTransmute[gl_player] = gl_state
endfunction

function toggleForkedLightning takes nothing returns nothing
    set udg_activeForkedLightning[gl_player] = gl_state
endfunction

function toggleFrostArrows takes nothing returns nothing
    set udg_activeFrostArrows[gl_player] = gl_state
    if(gl_state == false) then
        call ToggleAutoCast(udg_SeaWitches, "uncoldarrows")
    else
        call ToggleAutoCast(udg_SeaWitches, "coldarrows")
    endif
endfunction

function toggleManaShield takes nothing returns nothing
    set udg_activeManaShield[gl_player] = gl_state
endfunction

function toggleTornadoNeutral takes nothing returns nothing
    set udg_activeTornadoNeutral[gl_player] = gl_state
endfunction

function togglePocketFactory takes nothing returns nothing
    set udg_activePocketFactory[gl_player] = gl_state
endfunction

function toggleClusterRockets takes nothing returns nothing
    set udg_activeClusterRockets[gl_player] = gl_state
endfunction

function toggleRoboGoblin takes nothing returns nothing
    set udg_activeRoboGoblin[gl_player] = gl_state
endfunction

function toggleSummonBear takes nothing returns nothing
    set udg_activeSummonBear[gl_player] = gl_state
endfunction

function toggleSummonQuilbeast takes nothing returns nothing
    set udg_activeSummonQuilbeast[gl_player] = gl_state
endfunction

function toggleSummonHawk takes nothing returns nothing
    set udg_activeSummonHawk[gl_player] = gl_state
endfunction

function toggleStampede takes nothing returns nothing
    set udg_activeStampede[gl_player] = gl_state
endfunction

function toggleBreathOfFire takes nothing returns nothing
    set udg_activeBreathOfFire[gl_player] = gl_state
endfunction

function toggleDrunkenHaze takes nothing returns nothing
    set udg_activeDrunkenHaze[gl_player] = gl_state
endfunction

function toggleStormEarthFire takes nothing returns nothing
    set udg_activeStormEarthFire[gl_player] = gl_state
endfunction

function toggleSilenceNeutral takes nothing returns nothing
    set udg_activeSilenceNeutral[gl_player] = gl_state
endfunction

function toggleBlackArrow takes nothing returns nothing
    set udg_activeFrostArrows[gl_player] = gl_state
    if(gl_state == false) then
        call ToggleAutoCast(udg_DarkRangers, "blackarrowoff")
    else
        call ToggleAutoCast(udg_DarkRangers, "blackarrowon")
    endif
endfunction

function toggleLifeDrain takes nothing returns nothing
    set udg_activeLifeDrain[gl_player] = gl_state
endfunction

function toggleCharm takes nothing returns nothing
    set udg_activeCharm[gl_player] = gl_state
endfunction

function toggleSoulBurn takes nothing returns nothing
    set udg_activeSoulBurn[gl_player] = gl_state
endfunction

function toggleLavaSpawn takes nothing returns nothing
    set udg_activeLavaSpawn[gl_player] = gl_state
endfunction

function toggleVolcano takes nothing returns nothing
    set udg_activeVolcano[gl_player] = gl_state
endfunction

function toggleRainOfFire takes nothing returns nothing
    set udg_activeRainOfFire[gl_player] = gl_state
endfunction

function toggleHowlOfTerror takes nothing returns nothing
    set udg_activeHowlOfTerror[gl_player] = gl_state
endfunction

function toggleDoom takes nothing returns nothing
    set udg_activeDoom[gl_player] = gl_state
endfunction

function abilityToggleTest takes integer p_race, integer abilityID returns nothing

    if( p_race == 1 ) then
        if( abilityID == 1 ) then 
            call toggleDefend()
        elseif (abilityID == 2) then 
            call toggleSlow()
        elseif (abilityID == 3) then 
            call toggleInvisibility()
        elseif (abilityID == 4) then
            call togglePolymorph()
        elseif (abilityID == 5) then
            call toggleFlare()
        elseif (abilityID == 6) then 
            call toggleHeal()
        elseif (abilityID == 7) then 
            call toggleDispelMagic()
        elseif (abilityID == 8) then 
            call toggleInnerFire()
        elseif (abilityID == 9) then
            call toggleControlMagic() 
        elseif (abilityID == 10) then 
            call toggleAerialShackles()
        elseif (abilityID == 11) then
            call toggleHolyLight() 
        elseif (abilityID == 12) then
            call toggleDivineShield() 
        elseif (abilityID == 13) then 
            call toggleResurrection()
        elseif (abilityID == 14) then
            call toggleBlizzard() 
        elseif (abilityID == 15) then 
            call toggleWaterElemental()
        elseif (abilityID == 16) then 
            call toggleTornado()
        elseif (abilityID == 17) then 
            call toggleStormBolt()
        elseif (abilityID == 18) then 
            call toggleThunderClap()
        elseif (abilityID == 19) then 
            call toggleAvatar()
        elseif (abilityID == 20) then 
            call toggleFlamestrike()
        elseif (abilityID == 21) then 
            call toggleBanish()
        elseif (abilityID == 22) then 
            call toggleSiphonMana()
        elseif (abilityID == 23) then 
            call togglePhoenix()
        endif
    elseif( p_race == 2 ) then
        if( abilityID == 1 ) then 
            call toggleBerserk()
        elseif (abilityID == 2) then 
            call togglePurge()
        elseif (abilityID == 3) then 
            call toggleLightningShield()
        elseif (abilityID == 4) then 
            call toggleBloodlust()
        elseif (abilityID == 5) then 
            call toggleEnsnare()
        elseif (abilityID == 6) then 
            call toggleSentryWard()
        elseif (abilityID == 7) then 
            call toggleStasisTrap()
        elseif (abilityID == 8) then 
            call toggleHealingWard()
        elseif (abilityID == 9) then 
            call toggleSpiritLink()
        elseif (abilityID == 10) then 
            call toggleDisenchant()
        elseif (abilityID == 11) then 
            call toggleAncestralSpirit()
        elseif (abilityID == 12) then 
            call toggleUnstableConcoction()
        elseif (abilityID == 13) then 
            call toggleDevour()
        elseif (abilityID == 14) then 
            call toggleWindWalk()
        elseif (abilityID == 15) then 
            call toggleMirrorImage()
        elseif (abilityID == 16) then 
            call toggleBladestorm()
        elseif (abilityID == 17) then 
            call toggleChainLightning()
        elseif (abilityID == 18) then 
            call toggleFirebolt()
        elseif (abilityID == 19) then 
            call toggleSpiritWolves()
        elseif (abilityID == 20) then 
            call toggleEarthquake()
        elseif (abilityID == 21) then 
            call toggleShockwave()
        elseif (abilityID == 22) then 
            call toggleWarStomp()
        elseif (abilityID == 23) then 
            call toggleHealingWave()
        elseif (abilityID == 24) then 
            call toggleHex()
        elseif (abilityID == 25) then 
            call toggleSerpentWard()
        elseif (abilityID == 26) then 
            call toggleBigBadVoodoo()
        endif
    elseif( p_race == 3 ) then
        if( abilityID == 1 ) then 
            call toggleWeb()
        elseif (abilityID == 2) then 
            call toggleBurrow()
        elseif (abilityID == 3) then 
            call toggleCurse()
        elseif (abilityID == 4) then 
            call toggleAntiMagicShell()
        elseif (abilityID == 5) then 
            call togglePossession()
        elseif (abilityID == 6) then 
            call toggleSpiritTouch()
        elseif (abilityID == 7) then 
            call toggleEssenceOfBlight()
        elseif (abilityID == 8) then 
            call toggleStoneForm()
        elseif (abilityID == 9) then 
            call toggleRaiseDead()
        elseif (abilityID == 10) then 
            call toggleUnholyFrenzy()
        elseif (abilityID == 11) then 
            call toggleCripple()
        elseif (abilityID == 12) then 
            call toggleDevourMagic()
        elseif (abilityID == 13) then 
            call toggleOrbOfAnnihilation()
        elseif (abilityID == 14) then 
            call toggleAbsorbMana()
        elseif (abilityID == 15) then 
            call toggleDeathCoil()
        elseif (abilityID == 16) then 
            call toggleDeathPact()
        elseif (abilityID == 17) then 
            call toggleAnimateDead()
        elseif (abilityID == 18) then 
            call toggleFrostNova()
        elseif (abilityID == 19) then 
            call toggleFrostShield()
        elseif (abilityID == 20) then 
            call toggleDarkRitual()
        elseif (abilityID == 21) then 
            call toggleDeathAndDecay()
        elseif (abilityID == 22) then 
            call toggleCarrionSwarm()
        elseif (abilityID == 23) then 
            call toggleSleep()
        elseif (abilityID == 24) then 
            call toggleInferno()
        elseif (abilityID == 25) then 
            call toggleImpale()
        elseif (abilityID == 26) then 
            call toggleCarrionBeetles()
        elseif (abilityID == 27) then 
            call toggleLocustSwarm()
        endif
    elseif( p_race == 4 ) then
        if( abilityID == 1 ) then 
            call toggleSentinel()
        elseif (abilityID == 2) then 
            call toggleAbolishMagic()
        elseif (abilityID == 3) then 
            call toggleRoar()
        elseif (abilityID == 4) then 
            call toggleRejuvenation()
        elseif (abilityID == 5) then 
            call toggleBearForm()
        elseif (abilityID == 6) then 
            call toggleFaerieFire()
        elseif (abilityID == 7) then 
            call toggleCrowForm()
        elseif (abilityID == 8) then 
            call toggleCyclone()
        elseif (abilityID == 9) then 
            call togglePhaseShift()
        elseif (abilityID == 10) then 
            call toggleManaFlare()
        elseif (abilityID == 11) then 
            call toggleTaunt()
        elseif (abilityID == 12) then 
            call toggleEntanglingRoots()
        elseif (abilityID == 13) then 
            call toggleForceOfNature()
        elseif (abilityID == 14) then 
            call toggleTranquility()
        elseif (abilityID == 15) then 
            call toggleSilence()
        elseif (abilityID == 16) then 
            call toggleSearingArrows()
        elseif (abilityID == 17) then 
            call toggleStarfall()
        elseif (abilityID == 18) then 
            call toggleManaBurn()
        elseif (abilityID == 19) then 
            call toggleImmolation()
        elseif (abilityID == 20) then 
            call toggleMetamorphosis()
        elseif (abilityID == 21) then 
            call toggleFanOfKnives()
        elseif (abilityID == 22) then 
            call toggleBlink()
        elseif (abilityID == 23) then 
            call toggleShadowStrike()
        elseif (abilityID == 24) then 
            call toggleVengeance()
        endif
    endif
endfunction

function abilityToggleTestNeutral takes integer abilityID returns nothing
    if( abilityID == 1 ) then
        call toggleHealingSpray() 
    elseif (abilityID == 2) then
        call toggleChemicalRage()
    elseif (abilityID == 3) then
        call toggleAcidBomb()
    elseif (abilityID == 4) then
        call toggleTransmute()
    elseif (abilityID == 5) then
        call toggleForkedLightning()
    elseif (abilityID == 6) then
        call toggleFrostArrows()
    elseif (abilityID == 7) then
        call toggleManaShield()
    elseif (abilityID == 8) then
        call toggleTornadoNeutral()
    elseif (abilityID == 9) then
        call togglePocketFactory()
    elseif (abilityID == 10) then
        call toggleClusterRockets()
    elseif (abilityID == 11) then
        call toggleRoboGoblin()
    elseif (abilityID == 12) then
        call toggleSummonBear()
    elseif (abilityID == 13) then
        call toggleSummonQuilbeast()
    elseif (abilityID == 14) then
        call toggleSummonHawk()
    elseif (abilityID == 15) then
        call toggleStampede()
    elseif (abilityID == 16) then
        call toggleBreathOfFire()
    elseif (abilityID == 17) then
        call toggleDrunkenHaze()
    elseif (abilityID == 18) then
        call toggleStormEarthFire()
    elseif (abilityID == 19) then
        call toggleSilenceNeutral()
    elseif (abilityID == 20) then
        call toggleBlackArrow()
    elseif (abilityID == 21) then
        call toggleLifeDrain()
    elseif (abilityID == 22) then
        call toggleCharm()
    elseif (abilityID == 23) then
        call toggleSoulBurn()
    elseif (abilityID == 24) then
        call toggleLavaSpawn()
    elseif (abilityID == 25) then
        call toggleVolcano()
    elseif (abilityID == 26) then
        call toggleRainOfFire()
    elseif (abilityID == 27) then
        call toggleHowlOfTerror()
    elseif (abilityID == 28) then
        call toggleDoom()
    endif
endfunction

function AbilityToggle takes nothing returns nothing
    local integer p = GetConvertedPlayerId(GetTriggerPlayer())
    local integer r = udg_PlayerRace[p]
    local integer i 
    local integer endIndex
    set gl_state = true
    set gl_player = p + 6

    set i = 1
    set endIndex = 27
    loop
        exitwhen i > 27
        if (BlzGetTriggerFrame() == btnAbilityEnabled[r][i]) then
            set gl_state = false
            call abilityToggleTest(r, i) // custom function
            if (GetLocalPlayer() == GetTriggerPlayer()) then
                call BlzFrameSetEnable(BlzGetTriggerFrame(), false)
                call BlzFrameSetEnable(BlzGetTriggerFrame(), true)
                call BlzFrameSetVisible(btnAbilityEnabled[r][i], false)
                call BlzFrameSetVisible(btnAbilityDisabled[r][i], true)
            endif
        endif

        if (BlzGetTriggerFrame() == btnAbilityDisabled[r][i]) then
            set gl_state = true
            call abilityToggleTest(r, i) // custom function
            if (GetLocalPlayer() == GetTriggerPlayer()) then
                call BlzFrameSetEnable(BlzGetTriggerFrame(), false)
                call BlzFrameSetEnable(BlzGetTriggerFrame(), true)
                call BlzFrameSetVisible(btnAbilityDisabled[r][i], false)
                call BlzFrameSetVisible(btnAbilityEnabled[r][i], true)
            endif
        endif
        
        set i = i + 1
    endloop
endfunction

function NeutralAbilityToggle takes nothing returns nothing
    local integer p = GetConvertedPlayerId(GetTriggerPlayer())
    local integer i
    local integer endIndex
    set gl_state = true
    set gl_player = p + 6

    set i = 1
    set endIndex = 28
    loop
        exitwhen i > endIndex
        if (BlzGetTriggerFrame() == btnNeutralAbilityEnabled[i]) then
            set gl_state = false
            call abilityToggleTestNeutral(i) // custom function
            if (GetLocalPlayer() == GetTriggerPlayer()) then
                call BlzFrameSetEnable(BlzGetTriggerFrame(), false)
                call BlzFrameSetEnable(BlzGetTriggerFrame(), true)
                call BlzFrameSetVisible(btnNeutralAbilityEnabled[i], false)
                call BlzFrameSetVisible(btnNeutralAbilityDisabled[i], true)
            endif
        endif

        if (BlzGetTriggerFrame() == btnNeutralAbilityDisabled[i]) then
            set gl_state = true
            call abilityToggleTestNeutral(i) // custom function
            if (GetLocalPlayer() == GetTriggerPlayer()) then
                call BlzFrameSetEnable(BlzGetTriggerFrame(), false)
                call BlzFrameSetEnable(BlzGetTriggerFrame(), true)
                call BlzFrameSetVisible(btnNeutralAbilityDisabled[i], false)
                call BlzFrameSetVisible(btnNeutralAbilityEnabled[i], true)
            endif
        endif
        set i = i + 1
    endloop
endfunction

function AbilityHover takes nothing returns nothing
    local integer p = GetConvertedPlayerId(GetTriggerPlayer())
    local integer r = udg_PlayerRace[p]
    local integer i
    local integer endIndex

    set i = 1
    set endIndex = 27
    loop
        exitwhen i > endIndex
        if (BlzGetTriggerFrame() == btnAbilityEnabled[r][i]) then
            if (GetLocalPlayer() == GetTriggerPlayer()) then
                call BlzFrameSetText(frameAbilityName[r], abilityName[r][i])
                call BlzFrameSetText(frameAbilityState[r], "|cff03ff0bActive|r")
            endif
        endif

        if (BlzGetTriggerFrame() == btnAbilityDisabled[r][i]) then
            if (GetLocalPlayer() == GetTriggerPlayer()) then
                call BlzFrameSetText(frameAbilityName[r], abilityName[r][i])
                call BlzFrameSetText(frameAbilityState[r], "|cffff0303Disabled|r")
            endif
        endif

        set i = i + 1
    endloop
endfunction

function NeutralAbilityHover takes nothing returns nothing
    local integer p = GetConvertedPlayerId(GetTriggerPlayer())
    local integer i 
    local integer endIndex

    set i = 1
    set endIndex = 27
    loop
        exitwhen i > endIndex
        if (BlzGetTriggerFrame() == btnNeutralAbilityEnabled[i]) then
            if (GetLocalPlayer() == GetTriggerPlayer()) then
                call BlzFrameSetText(frameNeutralAbilityName, neutralAbilityName[i])
                call BlzFrameSetText(frameNeutralAbilityState, "|cff03ff0bActive|r")
            endif
        endif

        if (BlzGetTriggerFrame() == btnNeutralAbilityDisabled[i]) then
            if (GetLocalPlayer() == GetTriggerPlayer()) then
                call BlzFrameSetText(frameNeutralAbilityName, neutralAbilityName[i])
                call BlzFrameSetText(frameNeutralAbilityState, "|cffff0303Disabled|r")
            endif
        endif

        set i = i + 1
    endloop
endfunction




function SetupAbilityControl takes nothing returns nothing
    local trigger trig
    local framehandle frameUnitAbilityText
    local integer r
    local integer endIndex
    local framehandle frameNeutralHeroAbilityText
    local integer i
    local real frameXPOS
    local real frameYPOS
    local integer incr
    local integer endIndex1
    local framehandle imgFrame
    local trigger trig3
    local framehandle imgFrame2
    local trigger trig2
    local trigger trig4

    set btnAbilityControl = BlzCreateFrame("ScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
    call BlzFrameSetSize(btnAbilityControl, 0.07, 0.03)
    call BlzFrameSetAbsPoint(btnAbilityControl, FRAMEPOINT_CENTER, 0.155, 0.165)
    call BlzFrameSetText(btnAbilityControl, "Ability A.I.")
    call BlzFrameSetVisible(btnAbilityControl, false)

    // Button trigger
    set trig = CreateTrigger()
    call BlzTriggerRegisterFrameEvent(trig, btnAbilityControl, FRAMEEVENT_CONTROL_CLICK)
    call TriggerAddAction(trig, function MenuAbilities)

    set r = 1
    set endIndex = 4
    loop
        exitwhen r > endIndex
        set abilityMenu[r] = BlzCreateFrame("EscMenuBackdrop", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
        call BlzFrameSetSize(abilityMenu[r], 0.25, 0.34)
        call BlzFrameSetAbsPoint(abilityMenu[r], FRAMEPOINT_CENTER, 0.17, 0.36)
        call BlzFrameSetVisible(abilityMenu[r], false)

        set frameUnitAbilityText = BlzCreateFrameByType("BACKDROP", "", abilityMenu[r], "", 0)
        call BlzFrameSetSize(frameUnitAbilityText, 0.138, 0.048)
        call BlzFrameSetAbsPoint(frameUnitAbilityText, FRAMEPOINT_CENTER, 0.17, 0.495)
        call BlzFrameSetTexture(frameUnitAbilityText, "war3mapImported\\UnitAbilities.dds", 0, true)

        set frameHeroAbilityText = BlzCreateFrameByType("BACKDROP", "", abilityMenu[r], "", 0)
        call BlzFrameSetSize(frameHeroAbilityText, 0.138, 0.048)
        call BlzFrameSetAbsPoint(frameHeroAbilityText, FRAMEPOINT_CENTER, 0.17, 0.375)
        call BlzFrameSetTexture(frameHeroAbilityText, "war3mapImported\\HeroAbilities.tga", 0, true)

        set btnNeutralHeroes[r] = BlzCreateFrame("ScriptDialogButton", abilityMenu[r], 0, 0)
        call BlzFrameSetSize(btnNeutralHeroes[r], 0.1, 0.04)
        call BlzFrameSetAbsPoint(btnNeutralHeroes[r], FRAMEPOINT_CENTER, 0.22, 0.23)
        call BlzFrameSetText(btnNeutralHeroes[r], "Tavern Heroes")

        // Button trigger
        set trig = CreateTrigger()
        call BlzTriggerRegisterFrameEvent(trig, btnNeutralHeroes[r], FRAMEEVENT_CONTROL_CLICK)
        call TriggerAddAction(trig, function MenuNeutralAbilities)
        
        set r = r + 1
    endloop

    set neutralAbilityMenu = BlzCreateFrame("EscMenuBackdrop", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
    call BlzFrameSetSize(neutralAbilityMenu, 0.25, 0.28)
    call BlzFrameSetAbsPoint(neutralAbilityMenu, FRAMEPOINT_CENTER, 0.42, 0.330)
    call BlzFrameSetVisible(neutralAbilityMenu, false)

    set frameNeutralHeroAbilityText = BlzCreateFrameByType("BACKDROP", "", neutralAbilityMenu, "", 0)
    call BlzFrameSetSize(frameNeutralHeroAbilityText, 0.194, 0.048)
    call BlzFrameSetAbsPoint(frameNeutralHeroAbilityText, FRAMEPOINT_CENTER, 0.42, 0.430)
    call BlzFrameSetTexture(frameNeutralHeroAbilityText, "war3mapImported\\NeutralAbilities.tga", 0, true)

    set i = 1
    set endIndex = 6
    loop
        exitwhen i > endIndex
        set isVisibleAbilityMenu[i] = false
        set isVisibleNeutralAbilityMenu[i] = false
        set i = i + 1
    endloop


    // Human
    set abilityName[1][1] = "Defend:"
    set btnAbilityImgEnabled[1][1] = "ReplaceableTextures\\CommandButtons\\BTNDefend.blp"
    set btnAbilityImgDisabled[1][1] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNDefend.blp"
    set abilityName[1][2] = "Slow:"
    set btnAbilityImgEnabled[1][2] = "ReplaceableTextures\\CommandButtons\\BTNSlowOn.blp"
    set btnAbilityImgDisabled[1][2] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNSlowOn.blp"
    set abilityName[1][3] = "Invisibility:"
    set btnAbilityImgEnabled[1][3] = "ReplaceableTextures\\CommandButtons\\BTNInvisibility.blp"
    set btnAbilityImgDisabled[1][3] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNInvisibility.blp"
    set abilityName[1][4] = "Polymorph:"
    set btnAbilityImgEnabled[1][4] = "ReplaceableTextures\\CommandButtons\\BTNPolymorph.blp"
    set btnAbilityImgDisabled[1][4] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNPolymorph.blp"
    set abilityName[1][5] = "Flare:"
    set btnAbilityImgEnabled[1][5] = "ReplaceableTextures\\CommandButtons\\BTNFlare.blp"
    set btnAbilityImgDisabled[1][5] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNFlare.blp"
    set abilityName[1][6] = "Heal:"
    set btnAbilityImgEnabled[1][6] = "ReplaceableTextures\\CommandButtons\\BTNHealOn.blp"
    set btnAbilityImgDisabled[1][6] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNHealOn.blp"
    set abilityName[1][7] = "Dispel Magic:"
    set btnAbilityImgEnabled[1][7] = "ReplaceableTextures\\CommandButtons\\BTNDispelMagic.blp"
    set btnAbilityImgDisabled[1][7] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNDispelMagic.blp"
    set abilityName[1][8] = "Inner Fire:"
    set btnAbilityImgEnabled[1][8] = "ReplaceableTextures\\CommandButtons\\BTNInnerFireOn.blp"
    set btnAbilityImgDisabled[1][8] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNInnerFireOn.blp"
    set abilityName[1][9] = "Control Magic:"
    set btnAbilityImgEnabled[1][9] = "ReplaceableTextures\\CommandButtons\\BTNControlMagic.blp"
    set btnAbilityImgDisabled[1][9] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNControlMagic.blp"
    set abilityName[1][10] = "Aerial Shackles:"
    set btnAbilityImgEnabled[1][10] = "ReplaceableTextures\\CommandButtons\\BTNMagicLariet.blp"
    set btnAbilityImgDisabled[1][10] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNMagicLariet.blp"
    
    set abilityName[1][11] = "Holy Light:"
    set btnAbilityImgEnabled[1][11] = "ReplaceableTextures\\CommandButtons\\BTNHolyBolt.blp"
    set btnAbilityImgDisabled[1][11] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNHolyBolt.blp"
    set abilityName[1][12] = "Divine Shield:"
    set btnAbilityImgEnabled[1][12] = "ReplaceableTextures\\CommandButtons\\BTNDivineIntervention.blp"
    set btnAbilityImgDisabled[1][12] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNDivineIntervention.blp"
    set abilityName[1][13] = "Resurrection:"
    set btnAbilityImgEnabled[1][13] = "ReplaceableTextures\\CommandButtons\\BTNResurrection.blp"
    set btnAbilityImgDisabled[1][13] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNResurrection.blp"
    set abilityName[1][14] = "Blizzard:"
    set btnAbilityImgEnabled[1][14] = "ReplaceableTextures\\CommandButtons\\BTNBlizzard.blp"
    set btnAbilityImgDisabled[1][14] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNBlizzard.blp"
    set abilityName[1][15] = "Water Elemental:"
    set btnAbilityImgEnabled[1][15] = "ReplaceableTextures\\CommandButtons\\BTNSummonWaterElemental.blp"
    set btnAbilityImgDisabled[1][15] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNSummonWaterElemental.blp"
    set abilityName[1][16] = "Tornado:"
    set btnAbilityImgEnabled[1][16] = "ReplaceableTextures\\CommandButtons\\BTNTornado.blp"
    set btnAbilityImgDisabled[1][16] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNTornado.blp"
    set abilityName[1][17] = "Storm Bolt:"
    set btnAbilityImgEnabled[1][17] = "ReplaceableTextures\\CommandButtons\\BTNStormBolt.blp"
    set btnAbilityImgDisabled[1][17] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNStormBolt.blp"
    set abilityName[1][18] = "Thunder Clap:"
    set btnAbilityImgEnabled[1][18] = "ReplaceableTextures\\CommandButtons\\BTNThunderclap.blp"
    set btnAbilityImgDisabled[1][18] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNThunderclap.blp"
    set abilityName[1][19] = "Avatar:"
    set btnAbilityImgEnabled[1][19] = "ReplaceableTextures\\CommandButtons\\BTNAvatarOn.blp"
    set btnAbilityImgDisabled[1][19] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNAvatarOn.blp"
    set abilityName[1][20] = "Flamestrike:"
    set btnAbilityImgEnabled[1][20] = "ReplaceableTextures\\CommandButtons\\BTNWallOfFire.blp"
    set btnAbilityImgDisabled[1][20] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNWallOfFire.blp"
    set abilityName[1][21] = "Banish:"
    set btnAbilityImgEnabled[1][21] = "ReplaceableTextures\\CommandButtons\\BTNBanish.blp"
    set btnAbilityImgDisabled[1][21] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNBanish.blp"
    set abilityName[1][22] = "Siphon Mana:"
    set btnAbilityImgEnabled[1][22] = "ReplaceableTextures\\CommandButtons\\BTNManaDrain.blp"
    set btnAbilityImgDisabled[1][22] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNManaDrain.blp"
    set abilityName[1][23] = "Phoenix:"
    set btnAbilityImgEnabled[1][23] = "ReplaceableTextures\\CommandButtons\\BTNMarkOfFire.blp"
    set btnAbilityImgDisabled[1][23] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNMarkOfFire.blp"


    // Orc
    set abilityName[2][1] = "Berserker:"
    set btnAbilityImgEnabled[2][1] = "ReplaceableTextures\\CommandButtons\\BTNBerserkForTrolls.blp"
    set btnAbilityImgDisabled[2][1] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNBerserkForTrolls.blp"
    set abilityName[2][2] = "Purge:"
    set btnAbilityImgEnabled[2][2] = "ReplaceableTextures\\CommandButtons\\BTNPurge.blp"
    set btnAbilityImgDisabled[2][2] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNPurge.blp"
    set abilityName[2][3] = "Lightning Shield:"
    set btnAbilityImgEnabled[2][3] = "ReplaceableTextures\\CommandButtons\\BTNLightningShield.blp"
    set btnAbilityImgDisabled[2][3] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNLightningShield.blp"
    set abilityName[2][4] = "Bloodlust:"
    set btnAbilityImgEnabled[2][4] = "ReplaceableTextures\\CommandButtons\\BTNBloodLustOn.blp"
    set btnAbilityImgDisabled[2][4] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNBloodLustOn.blp"
    set abilityName[2][5] = "Ensnare:"
    set btnAbilityImgEnabled[2][5] = "ReplaceableTextures\\CommandButtons\\BTNEnsnare.blp"
    set btnAbilityImgDisabled[2][5] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNEnsnare.blp"
    set abilityName[2][6] = "Sentry Ward:"
    set btnAbilityImgEnabled[2][6] = "ReplaceableTextures\\CommandButtons\\BTNSentryWard.blp"
    set btnAbilityImgDisabled[2][6] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNSentryWard.blp"
    set abilityName[2][7] = "Stasis Trap:"
    set btnAbilityImgEnabled[2][7] = "ReplaceableTextures\\CommandButtons\\BTNStasisTrap.blp"
    set btnAbilityImgDisabled[2][7] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNStasisTrap.blp"
    set abilityName[2][8] = "Healing Ward:"
    set btnAbilityImgEnabled[2][8] = "ReplaceableTextures\\CommandButtons\\BTNHealingWard.blp"
    set btnAbilityImgDisabled[2][8] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNHealingWard.blp"
    set abilityName[2][9] = "Spirit Link:"
    set btnAbilityImgEnabled[2][9] = "ReplaceableTextures\\CommandButtons\\BTNSpiritLink.blp"
    set btnAbilityImgDisabled[2][9] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNSpiritLink.blp"
    set abilityName[2][10] = "Disenchant:"
    set btnAbilityImgEnabled[2][10] = "ReplaceableTextures\\CommandButtons\\BTNDisenchant.blp"
    set btnAbilityImgDisabled[2][10] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNDisenchant.blp"
    set abilityName[2][11] = "Ancestral Spirit:"
    set btnAbilityImgEnabled[2][11] = "ReplaceableTextures\\CommandButtons\\BTNAncestralSpirit.blp"
    set btnAbilityImgDisabled[2][11] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNAncestralSpirit.blp"
    set abilityName[2][12] = "Unstable Concoction:"
    set btnAbilityImgEnabled[2][12] = "ReplaceableTextures\\CommandButtons\\BTNUnstableConcoction.blp"
    set btnAbilityImgDisabled[2][12] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNUnstableConcoction.blp"
    set abilityName[2][13] = "Devour:"
    set btnAbilityImgEnabled[2][13] = "ReplaceableTextures\\CommandButtons\\BTNDevour.blp"
    set btnAbilityImgDisabled[2][13] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNDevour.blp"
    
    set abilityName[2][14] = "Wind Walk:"
    set btnAbilityImgEnabled[2][14] = "ReplaceableTextures\\CommandButtons\\BTNWindWalkOn.blp"
    set btnAbilityImgDisabled[2][14] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNWindWalkOn.blp"
    set abilityName[2][15] = "Mirror Image:"
    set btnAbilityImgEnabled[2][15] = "ReplaceableTextures\\CommandButtons\\BTNMirrorImage.blp"
    set btnAbilityImgDisabled[2][15] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNMirrorImage.blp"
    set abilityName[2][16] = "Bladestorm:"
    set btnAbilityImgEnabled[2][16] = "ReplaceableTextures\\CommandButtons\\BTNWhirlwind.blp"
    set btnAbilityImgDisabled[2][16] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNWhirlwind.blp"
    set abilityName[2][17] = "Chain Lightning:"
    set btnAbilityImgEnabled[2][17] = "ReplaceableTextures\\CommandButtons\\BTNChainLightning.blp"
    set btnAbilityImgDisabled[2][17] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNChainLightning.blp"
    set abilityName[2][18] = "Firebolt:"
    set btnAbilityImgEnabled[2][18] = "ReplaceableTextures\\CommandButtons\\BTNFireBolt.blp"
    set btnAbilityImgDisabled[2][18] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNFireBolt.blp"
    set abilityName[2][19] = "Spirit Wolves:"
    set btnAbilityImgEnabled[2][19] = "ReplaceableTextures\\CommandButtons\\BTNSpiritWolf.blp"
    set btnAbilityImgDisabled[2][19] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNSpiritWolf.blp"
    set abilityName[2][20] = "Earthquake:"
    set btnAbilityImgEnabled[2][20] = "ReplaceableTextures\\CommandButtons\\BTNEarthquake.blp"
    set btnAbilityImgDisabled[2][20] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNEarthquake.blp"
    set abilityName[2][21] = "Shockwave:"
    set btnAbilityImgEnabled[2][21] = "ReplaceableTextures\\CommandButtons\\BTNShockWave.blp"
    set btnAbilityImgDisabled[2][21] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNShockWave.blp"
    set abilityName[2][22] = "War Stomp:"
    set btnAbilityImgEnabled[2][22] = "ReplaceableTextures\\CommandButtons\\BTNWarStomp.blp"
    set btnAbilityImgDisabled[2][22] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNWarStomp.blp"
    set abilityName[2][23] = "Healing Wave:"
    set btnAbilityImgEnabled[2][23] = "ReplaceableTextures\\CommandButtons\\BTNHealingWave.blp"
    set btnAbilityImgDisabled[2][23] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNHealingWave.blp"
    set abilityName[2][24] = "Hex:"
    set btnAbilityImgEnabled[2][24] = "ReplaceableTextures\\CommandButtons\\BTNHex.blp"
    set btnAbilityImgDisabled[2][24] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNHex.blp"
    set abilityName[2][25] = "Serpent Ward:"
    set btnAbilityImgEnabled[2][25] = "ReplaceableTextures\\CommandButtons\\BTNSerpentWard.blp"
    set btnAbilityImgDisabled[2][25] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNSerpentWard.blp"
    set abilityName[2][26] = "Big Bad Voodoo:"
    set btnAbilityImgEnabled[2][26] = "ReplaceableTextures\\CommandButtons\\BTNBigBadVoodooSpell.blp"
    set btnAbilityImgDisabled[2][26] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNBigBadVoodooSpell.blp"


    // Undead
    set abilityName[3][1] = "Web:"
    set btnAbilityImgEnabled[3][1] = "ReplaceableTextures\\CommandButtons\\BTNWebOn.blp"
    set btnAbilityImgDisabled[3][1] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNWebOn.blp"
    set abilityName[3][2] = "Burrow:"
    set btnAbilityImgEnabled[3][2] = "ReplaceableTextures\\CommandButtons\\BTNCryptFiendBurrow.blp"
    set btnAbilityImgDisabled[3][2] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNCryptFiendBurrow.blp"
    set abilityName[3][3] = "Curse:"
    set btnAbilityImgEnabled[3][3] = "ReplaceableTextures\\CommandButtons\\BTNCurseOn.blp"
    set btnAbilityImgDisabled[3][3] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNCurseOn.blp"
    set abilityName[3][4] = "Anti-Magic Shell:"
    set btnAbilityImgEnabled[3][4] = "ReplaceableTextures\\CommandButtons\\BTNAntiMagicShell.blp"
    set btnAbilityImgDisabled[3][4] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNAntiMagicShell.blp"
    set abilityName[3][5] = "Possession:"
    set btnAbilityImgEnabled[3][5] = "ReplaceableTextures\\CommandButtons\\BTNPossession.blp"
    set btnAbilityImgDisabled[3][5] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNPossession.blp"
    set abilityName[3][6] = "Spirit Touch:"
    set btnAbilityImgEnabled[3][6] = "ReplaceableTextures\\CommandButtons\\BTNReplenishManaOn.blp"
    set btnAbilityImgDisabled[3][6] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNReplenishManaOn.blp"
    set abilityName[3][7] = "Essence of Blight:"
    set btnAbilityImgEnabled[3][7] = "ReplaceableTextures\\CommandButtons\\BTNReplenishHealthOn.blp"
    set btnAbilityImgDisabled[3][7] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNReplenishHealthOn.blp"
    set abilityName[3][8] = "Stone Form:"
    set btnAbilityImgEnabled[3][8] = "ReplaceableTextures\\CommandButtons\\BTNStoneForm.blp"
    set btnAbilityImgDisabled[3][8] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNStoneForm.blp"
    set abilityName[3][9] = "Raise Dead:"
    set btnAbilityImgEnabled[3][9] = "ReplaceableTextures\\CommandButtons\\BTNRaiseDeadOn.blp"
    set btnAbilityImgDisabled[3][9] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNRaiseDeadOn.blp"
    set abilityName[3][10] = "Unholy Frenzy:"
    set btnAbilityImgEnabled[3][10] = "ReplaceableTextures\\CommandButtons\\BTNUnholyFrenzy.blp"
    set btnAbilityImgDisabled[3][10] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNUnholyFrenzy.blp"
    set abilityName[3][11] = "Cripple:"
    set btnAbilityImgEnabled[3][11] = "ReplaceableTextures\\CommandButtons\\BTNCripple.blp"
    set btnAbilityImgDisabled[3][11] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNCripple.blp"
    set abilityName[3][12] = "Devour Magic:"
    set btnAbilityImgEnabled[3][12] = "ReplaceableTextures\\CommandButtons\\BTNDevourMagic.blp"
    set btnAbilityImgDisabled[3][12] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNDevourMagic.blp"
    set abilityName[3][13] = "Orb of Annihilation:"
    set btnAbilityImgEnabled[3][13] = "ReplaceableTextures\\CommandButtons\\BTNOrbOfDeathOn.blp"
    set btnAbilityImgDisabled[3][13] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNOrbOfDeathOn.blp"
    set abilityName[3][14] = "Absorb Magic:"
    set btnAbilityImgEnabled[3][14] = "ReplaceableTextures\\CommandButtons\\BTNAbsorbMagic.blp"
    set btnAbilityImgDisabled[3][14] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNAbsorbMagic.blp"

    set abilityName[3][15] = "Death Coil:"
    set btnAbilityImgEnabled[3][15] = "ReplaceableTextures\\CommandButtons\\BTNDeathCoil.blp"
    set btnAbilityImgDisabled[3][15] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNDeathCoil.blp"
    set abilityName[3][16] = "Death Pact:"
    set btnAbilityImgEnabled[3][16] = "ReplaceableTextures\\CommandButtons\\BTNDeathPact.blp"
    set btnAbilityImgDisabled[3][16] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNDeathPact.blp"
    set abilityName[3][17] = "Animate Dead:"
    set btnAbilityImgEnabled[3][17] = "ReplaceableTextures\\CommandButtons\\BTNAnimateDead.blp"
    set btnAbilityImgDisabled[3][17] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNAnimateDead.blp"
    set abilityName[3][18] = "Frost Nova:"
    set btnAbilityImgEnabled[3][18] = "ReplaceableTextures\\CommandButtons\\BTNGlacier.blp"
    set btnAbilityImgDisabled[3][18] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNGlacier.blp"
    set abilityName[3][19] = "Frost Armor:"
    set btnAbilityImgEnabled[3][19] = "ReplaceableTextures\\CommandButtons\\BTNFrostArmor.blp"
    set btnAbilityImgDisabled[3][19] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNFrostArmor.blp"
    set abilityName[3][20] = "Dark Ritual:"
    set btnAbilityImgEnabled[3][20] = "ReplaceableTextures\\CommandButtons\\BTNDarkRitual.blp"
    set btnAbilityImgDisabled[3][20] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNDarkRitual.blp"
    set abilityName[3][21] = "Death and Decay:"
    set btnAbilityImgEnabled[3][21] = "ReplaceableTextures\\CommandButtons\\BTNDeathAndDecay.blp"
    set btnAbilityImgDisabled[3][21] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNDeathAndDecay.blp"
    set abilityName[3][22] = "Carrion Swarm:"
    set btnAbilityImgEnabled[3][22] = "ReplaceableTextures\\CommandButtons\\BTNCarrionSwarm.blp"
    set btnAbilityImgDisabled[3][22] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNCarrionSwarm.blp"
    set abilityName[3][23] = "Sleep:"
    set btnAbilityImgEnabled[3][23] = "ReplaceableTextures\\CommandButtons\\BTNSleep.blp"
    set btnAbilityImgDisabled[3][23] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNSleep.blp"
    set abilityName[3][24] = "Inferno:"
    set btnAbilityImgEnabled[3][24] = "ReplaceableTextures\\CommandButtons\\BTNInfernal.blp"
    set btnAbilityImgDisabled[3][24] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNInfernal.blp"
    set abilityName[3][25] = "Impale:"
    set btnAbilityImgEnabled[3][25] = "ReplaceableTextures\\CommandButtons\\BTNImpale.blp"
    set btnAbilityImgDisabled[3][25] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNImpale.blp"
    set abilityName[3][26] = "Carrion Beetles:"
    set btnAbilityImgEnabled[3][26] = "ReplaceableTextures\\CommandButtons\\BTNCarrionScarabsOn.blp"
    set btnAbilityImgDisabled[3][26] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNCarrionScarabsOn.blp"
    set abilityName[3][27] = "Locust Swarm:"
    set btnAbilityImgEnabled[3][27] = "ReplaceableTextures\\CommandButtons\\BTNLocustSwarm.blp"
    set btnAbilityImgDisabled[3][27] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNLocustSwarm.blp"


    // Night Elf
    set abilityName[4][1] = "Sentinel:"
    set btnAbilityImgEnabled[4][1] = "ReplaceableTextures\\CommandButtons\\BTNSentinel.blp"
    set btnAbilityImgDisabled[4][1] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNSentinel.blp"
    set abilityName[4][2] = "Abolish Magic:"
    set btnAbilityImgEnabled[4][2] = "ReplaceableTextures\\CommandButtons\\BTNDryadDispelMagicOn.blp"
    set btnAbilityImgDisabled[4][2] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNDryadDispelMagicOn.blp"
    set abilityName[4][3] = "Roar:"
    set btnAbilityImgEnabled[4][3] = "ReplaceableTextures\\CommandButtons\\BTNBattleRoar.blp"
    set btnAbilityImgDisabled[4][3] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNBattleRoar.blp"
    set abilityName[4][4] = "Rejuvenation:"
    set btnAbilityImgEnabled[4][4] = "ReplaceableTextures\\CommandButtons\\BTNRejuvenation.blp"
    set btnAbilityImgDisabled[4][4] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNRejuvenation.blp"
    set abilityName[4][5] = "Bear Form:"
    set btnAbilityImgEnabled[4][5] = "ReplaceableTextures\\CommandButtons\\BTNBearForm.blp"
    set btnAbilityImgDisabled[4][5] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNBearForm.blp"
    set abilityName[4][6] = "Faerie Fire:"
    set btnAbilityImgEnabled[4][6] = "ReplaceableTextures\\CommandButtons\\BTNFaerieFireOn.blp"
    set btnAbilityImgDisabled[4][6] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNFaerieFireOn.blp"
    set abilityName[4][7] = "Storm Crow Form:"
    set btnAbilityImgEnabled[4][7] = "ReplaceableTextures\\CommandButtons\\BTNRavenForm.blp"
    set btnAbilityImgDisabled[4][7] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNRavenForm.blp"
    set abilityName[4][8] = "Cyclone:"
    set btnAbilityImgEnabled[4][8] = "ReplaceableTextures\\CommandButtons\\BTNCyclone.blp"
    set btnAbilityImgDisabled[4][8] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNCyclone.blp"
    set abilityName[4][9] = "Phase Shift:"
    set btnAbilityImgEnabled[4][9] = "ReplaceableTextures\\CommandButtons\\BTNPhaseShiftOn.blp"
    set btnAbilityImgDisabled[4][9] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNPhaseShiftOn.blp"
    set abilityName[4][10] = "Mana Flare:"
    set btnAbilityImgEnabled[4][10] = "ReplaceableTextures\\CommandButtons\\BTNManaFlare.blp"
    set btnAbilityImgDisabled[4][10] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNManaFlare.blp"
    set abilityName[4][11] = "Taunt:"
    set btnAbilityImgEnabled[4][11] = "ReplaceableTextures\\CommandButtons\\BTNTaunt.blp"
    set btnAbilityImgDisabled[4][11] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNTaunt.blp"

    set abilityName[4][12] = "Entangling Roots:"
    set btnAbilityImgEnabled[4][12] = "ReplaceableTextures\\CommandButtons\\BTNEntanglingRoots.blp"
    set btnAbilityImgDisabled[4][12] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNEntanglingRoots.blp"
    set abilityName[4][13] = "Force of Nature:"
    set btnAbilityImgEnabled[4][13] = "ReplaceableTextures\\CommandButtons\\BTNEnt.blp"
    set btnAbilityImgDisabled[4][13] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNEnt.blp"
    set abilityName[4][14] = "Tranquility:"
    set btnAbilityImgEnabled[4][14] = "ReplaceableTextures\\CommandButtons\\BTNTranquility.blp"
    set btnAbilityImgDisabled[4][14] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNTranquility.blp"
    set abilityName[4][15] = "Silence:"
    set btnAbilityImgEnabled[4][15] = "ReplaceableTextures\\CommandButtons\\BTNSilence.blp"
    set btnAbilityImgDisabled[4][15] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNSilence.blp"
    set abilityName[4][16] = "Searing Arrows:"
    set btnAbilityImgEnabled[4][16] = "ReplaceableTextures\\CommandButtons\\BTNSearingArrowsOn.blp"
    set btnAbilityImgDisabled[4][16] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNSearingArrowsOn.blp"
    set abilityName[4][17] = "Starfall:"
    set btnAbilityImgEnabled[4][17] = "ReplaceableTextures\\CommandButtons\\BTNStarfall.blp"
    set btnAbilityImgDisabled[4][17] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNStarfall.blp"
    set abilityName[4][18] = "Mana Burn:"
    set btnAbilityImgEnabled[4][18] = "ReplaceableTextures\\CommandButtons\\BTNManaBurn.blp"
    set btnAbilityImgDisabled[4][18] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNManaBurn.blp"
    set abilityName[4][19] = "Immolation:"
    set btnAbilityImgEnabled[4][19] = "ReplaceableTextures\\CommandButtons\\BTNImmolationOn.blp"
    set btnAbilityImgDisabled[4][19] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNImmolationOn.blp"
    set abilityName[4][20] = "Metamorphosis:"
    set btnAbilityImgEnabled[4][20] = "ReplaceableTextures\\CommandButtons\\BTNMetamorphosis.blp"
    set btnAbilityImgDisabled[4][20] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNMetamorphosis.blp"
    set abilityName[4][21] = "Fan of Knives:"
    set btnAbilityImgEnabled[4][21] = "ReplaceableTextures\\CommandButtons\\BTNFanOfKnives.blp"
    set btnAbilityImgDisabled[4][21] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNFanOfKnives.blp"
    set abilityName[4][22] = "Blink:"
    set btnAbilityImgEnabled[4][22] = "ReplaceableTextures\\CommandButtons\\BTNBlink.blp"
    set btnAbilityImgDisabled[4][22] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNBlink.blp"
    set abilityName[4][23] = "Shadow Strike:"
    set btnAbilityImgEnabled[4][23] = "ReplaceableTextures\\CommandButtons\\BTNShadowStrike.blp"
    set btnAbilityImgDisabled[4][23] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNShadowStrike.blp"
    set abilityName[4][24] = "Vengeance:"
    set btnAbilityImgEnabled[4][24] = "ReplaceableTextures\\CommandButtons\\BTNSpiritOfVengeance.blp"
    set btnAbilityImgDisabled[4][24] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNSpiritOfVengeance.blp"



    set frameXPOS = 0.0
    set frameYPOS = 0.0
    set incr = 1

    set r = 1
    set endIndex = 4
    loop
        exitwhen r > endIndex
        set frameAbilityName[r] = BlzCreateFrame("TasButtonTextTemplate", abilityMenu[r], 0, 0)
        call BlzFrameSetAbsPoint(frameAbilityName[r], FRAMEPOINT_LEFT, 0.075, 0.235)
        call BlzFrameSetText(frameAbilityName[r], "")
        call BlzFrameSetVisible(frameAbilityName[r], true)

        set frameAbilityState[r] = BlzCreateFrame("TasButtonTextTemplate", abilityMenu[r], 0, 0)
        call BlzFrameSetAbsPoint(frameAbilityState[r], FRAMEPOINT_LEFT, 0.075, 0.225)
        call BlzFrameSetText(frameAbilityState[r], "")
        call BlzFrameSetVisible(frameAbilityState[r], true)

        set i = 1
        set endIndex1 = 27
        loop
            exitwhen i > endIndex1
            if (btnAbilityImgEnabled[r][i] != null) then
                // Enabled
                set btnAbilityEnabled[r][i] = BlzCreateFrame("ScoreScreenBottomButtonTemplate", abilityMenu[r], 0, 0)
                call BlzFrameSetSize(btnAbilityEnabled[r][i], 0.04, 0.04)
                call BlzFrameSetAbsPoint(btnAbilityEnabled[r][i], FRAMEPOINT_CENTER, 0.09 + frameXPOS, 0.47 - frameYPOS)
                call BlzFrameSetVisible(btnAbilityEnabled[r][i], true)
                set imgFrame = BlzGetFrameByName("ScoreScreenButtonBackdrop", 0)
                call BlzFrameSetTexture(imgFrame, btnAbilityImgEnabled[r][i], 0, true)

                // Button trigger
                set trig = CreateTrigger()
                call BlzTriggerRegisterFrameEvent(trig, btnAbilityEnabled[r][i], FRAMEEVENT_CONTROL_CLICK)
                call TriggerAddAction(trig, function AbilityToggle)

                // Hover trigger
                set trig3 = CreateTrigger()
                call BlzTriggerRegisterFrameEvent(trig3, btnAbilityEnabled[r][i], FRAMEEVENT_MOUSE_ENTER)
                call TriggerAddAction(trig3, function AbilityHover)

                // Disabled
                set btnAbilityDisabled[r][i] = BlzCreateFrame("ScoreScreenBottomButtonTemplate", abilityMenu[r], 0, 0)
                call BlzFrameSetSize(btnAbilityDisabled[r][i], 0.04, 0.04)
                call BlzFrameSetAbsPoint(btnAbilityDisabled[r][i], FRAMEPOINT_CENTER, 0.09 + frameXPOS, 0.47 - frameYPOS)
                call BlzFrameSetVisible(btnAbilityDisabled[r][i], false)
                set imgFrame2 = BlzGetFrameByName("ScoreScreenButtonBackdrop", 0)
                call BlzFrameSetTexture(imgFrame2, btnAbilityImgDisabled[r][i], 0, true)

                // Button trigger
                set trig2 = CreateTrigger()
                call BlzTriggerRegisterFrameEvent(trig2, btnAbilityDisabled[r][i], FRAMEEVENT_CONTROL_CLICK)
                call TriggerAddAction(trig2, function AbilityToggle)

                // Hover trigger
                set trig4 = CreateTrigger()
                call BlzTriggerRegisterFrameEvent(trig4, btnAbilityDisabled[r][i], FRAMEEVENT_MOUSE_ENTER)
                call TriggerAddAction(trig4, function AbilityHover)

                set frameXPOS = frameXPOS + 0.032

                if (ModuloInteger(incr, 6) == 0) then
                    set frameYPOS = frameYPOS + 0.032
                    set frameXPOS = 0
                endif

                // Move hero buttons down
                if(r == 1 and i == 10) then
                    set frameYPOS = frameYPOS + 0.09
                    set frameXPOS = 0
                    set incr = 0
                elseif(r == 2 and i == 13) then
                    set frameYPOS = frameYPOS + 0.058
                    set frameXPOS = 0
                    set incr = 0
                elseif(r == 3 and i == 13) then
                    set frameYPOS = frameYPOS + 0.058
                    set frameXPOS = 0
                    set incr = 0
                elseif(r == 4 and i == 10) then
                    set frameYPOS = frameYPOS + 0.09
                    set frameXPOS = 0
                    set incr = 0
                endif
            endif
            set incr = incr + 1

            set i = i + 1
        endloop

        // reset for next race
        set frameYPOS = 0
        set frameXPOS = 0
        set incr = 1

        set r = r + 1
    endloop


    // Neutral

    set neutralAbilityName[1] = "Healing Spray:"
    set btnNeutralAbilityImgEnabled[1] = "ReplaceableTextures\\CommandButtons\\BTNHealingSpray.blp"
    set btnNeutralAbilityImgDisabled[1] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNHealingSpray.blp"
    set neutralAbilityName[2] = "Chemical Rage:"
    set btnNeutralAbilityImgEnabled[2] = "ReplaceableTextures\\CommandButtons\\BTNChemicalRage.blp"
    set btnNeutralAbilityImgDisabled[2] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNChemicalRage.blp"
    set neutralAbilityName[3] = "Acid Bomb:"
    set btnNeutralAbilityImgEnabled[3] = "ReplaceableTextures\\CommandButtons\\BTNAcidBomb.blp"
    set btnNeutralAbilityImgDisabled[3] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNAcidBomb.blp"
    set neutralAbilityName[4] = "Transmute:"
    set btnNeutralAbilityImgEnabled[4] = "ReplaceableTextures\\CommandButtons\\BTNTransmute.blp"
    set btnNeutralAbilityImgDisabled[4] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNTransmute.blp"
    set neutralAbilityName[5] = "Forked Lightning:"
    set btnNeutralAbilityImgEnabled[5] = "ReplaceableTextures\\CommandButtons\\BTNMonsoon.blp"
    set btnNeutralAbilityImgDisabled[5] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNMonsoon.blp"
    set neutralAbilityName[6] = "Frost Arrows:"
    set btnNeutralAbilityImgEnabled[6] = "ReplaceableTextures\\CommandButtons\\BTNColdArrowsOn.blp"
    set btnNeutralAbilityImgDisabled[6] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNColdArrowsOn.blp"
    set neutralAbilityName[7] = "Mana Shield:"
    set btnNeutralAbilityImgEnabled[7] = "ReplaceableTextures\\CommandButtons\\BTNNeutralManaShield.blp"
    set btnNeutralAbilityImgDisabled[7] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNNeutralManaShield.blp"
    set neutralAbilityName[8] = "Tornado:"
    set btnNeutralAbilityImgEnabled[8] = "ReplaceableTextures\\CommandButtons\\BTNTornado.blp"
    set btnNeutralAbilityImgDisabled[8] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNTornado.blp"
    set neutralAbilityName[9] = "Pocket Factory:"
    set btnNeutralAbilityImgEnabled[9] = "ReplaceableTextures\\CommandButtons\\BTNPocketFactory.blp"
    set btnNeutralAbilityImgDisabled[9] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNPocketFactory.blp"
    set neutralAbilityName[10] = "Cluster Rockets:"
    set btnNeutralAbilityImgEnabled[10] = "ReplaceableTextures\\CommandButtons\\BTNClusterRockets.blp"
    set btnNeutralAbilityImgDisabled[10] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNClusterRockets.blp"
    set neutralAbilityName[11] = "Robo Goblin:"
    set btnNeutralAbilityImgEnabled[11] = "ReplaceableTextures\\CommandButtons\\BTNROBOGOBLIN.blp"
    set btnNeutralAbilityImgDisabled[11] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNROBOGOBLIN.blp"
    set neutralAbilityName[12] = "Summon Bear:"
    set btnNeutralAbilityImgEnabled[12] = "ReplaceableTextures\\CommandButtons\\BTNGrizzlyBear.blp"
    set btnNeutralAbilityImgDisabled[12] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNGrizzlyBear.blp"
    set neutralAbilityName[13] = "Summon Quilbeast:"
    set btnNeutralAbilityImgEnabled[13] = "ReplaceableTextures\\CommandButtons\\BTNQuillBeast.blp"
    set btnNeutralAbilityImgDisabled[13] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNQuillBeast.blp"
    set neutralAbilityName[14] = "Summon Hawk:"
    set btnNeutralAbilityImgEnabled[14] = "ReplaceableTextures\\CommandButtons\\BTNWarEagle.blp"
    set btnNeutralAbilityImgDisabled[14] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNWarEagle.blp"
    set neutralAbilityName[15] = "Stampede:"
    set btnNeutralAbilityImgEnabled[15] = "ReplaceableTextures\\CommandButtons\\BTNStampede.blp"
    set btnNeutralAbilityImgDisabled[15] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNStampede.blp"
    set neutralAbilityName[16] = "Breath of Fire:"
    set btnNeutralAbilityImgEnabled[16] = "ReplaceableTextures\\CommandButtons\\BTNBreathOfFire.blp"
    set btnNeutralAbilityImgDisabled[16] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNBreathOfFire.blp"
    set neutralAbilityName[17] = "Drunken Haze:"
    set btnNeutralAbilityImgEnabled[17] = "ReplaceableTextures\\CommandButtons\\BTNStrongDrink.blp"
    set btnNeutralAbilityImgDisabled[17] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNStrongDrink.blp"
    set neutralAbilityName[18] = "Storm Earth & Fire:"
    set btnNeutralAbilityImgEnabled[18] = "ReplaceableTextures\\CommandButtons\\BTNStormEarth&Fire.blp"
    set btnNeutralAbilityImgDisabled[18] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNStormEarth&Fire.blp"
    set neutralAbilityName[19] = "Silence:"
    set btnNeutralAbilityImgEnabled[19] = "ReplaceableTextures\\CommandButtons\\BTNSilence.blp"
    set btnNeutralAbilityImgDisabled[19] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNSilence.blp"
    set neutralAbilityName[20] = "Black Arrow:"
    set btnNeutralAbilityImgEnabled[20] = "ReplaceableTextures\\CommandButtons\\BTNTheBlackArrowOnOff.blp"
    set btnNeutralAbilityImgDisabled[20] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNTheBlackArrowOnOff.blp"
    set neutralAbilityName[21] = "Life Drain:"
    set btnNeutralAbilityImgEnabled[21] = "ReplaceableTextures\\CommandButtons\\BTNLifeDrain.blp"
    set btnNeutralAbilityImgDisabled[21] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNLifeDrain.blp"
    set neutralAbilityName[22] = "Charm:"
    set btnNeutralAbilityImgEnabled[22] = "ReplaceableTextures\\CommandButtons\\BTNCharm.blp"
    set btnNeutralAbilityImgDisabled[22] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNCharm.blp"
    set neutralAbilityName[23] = "Soul Burn:"
    set btnNeutralAbilityImgEnabled[23] = "ReplaceableTextures\\CommandButtons\\BTNSoulBurn.blp"
    set btnNeutralAbilityImgDisabled[23] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNSoulBurn.blp"
    set neutralAbilityName[24] = "Lava Spawn:"
    set btnNeutralAbilityImgEnabled[24] = "ReplaceableTextures\\CommandButtons\\BTNLavaSpawn.blp"
    set btnNeutralAbilityImgDisabled[24] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNLavaSpawn.blp"
    set neutralAbilityName[25] = "Volcano:"
    set btnNeutralAbilityImgEnabled[25] = "ReplaceableTextures\\CommandButtons\\BTNVolcano.blp"
    set btnNeutralAbilityImgDisabled[25] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNVolcano.blp"
    set neutralAbilityName[26] = "Rain of Fire:"
    set btnNeutralAbilityImgEnabled[26] = "ReplaceableTextures\\CommandButtons\\BTNFire.blp"
    set btnNeutralAbilityImgDisabled[26] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNFire.blp"
    set neutralAbilityName[27] = "Howl of Terror:"
    set btnNeutralAbilityImgEnabled[27] = "ReplaceableTextures\\CommandButtons\\BTNHowlOfTerror.blp"
    set btnNeutralAbilityImgDisabled[27] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNHowlOfTerror.blp"
    set neutralAbilityName[28] = "Doom:"
    set btnNeutralAbilityImgEnabled[28] = "ReplaceableTextures\\CommandButtons\\BTNDoom.blp"
    set btnNeutralAbilityImgDisabled[28] = "ReplaceableTextures\\CommandButtonsDisabled\\DISBTNDoom.blp"


    // Neutral buttons

    set frameNeutralAbilityName = BlzCreateFrame("TasButtonTextTemplate", neutralAbilityMenu, 0, 0)
    call BlzFrameSetAbsPoint(frameNeutralAbilityName, FRAMEPOINT_LEFT, 0.323, 0.235)
    call BlzFrameSetText(frameNeutralAbilityName, "")
    call BlzFrameSetVisible(frameNeutralAbilityName, true)

    set frameNeutralAbilityState = BlzCreateFrame("TasButtonTextTemplate", neutralAbilityMenu, 0, 0)
    call BlzFrameSetAbsPoint(frameNeutralAbilityState, FRAMEPOINT_LEFT, 0.323, 0.225)
    call BlzFrameSetText(frameNeutralAbilityState, "")
    call BlzFrameSetVisible(frameNeutralAbilityState, true)

    set i = 1
    set endIndex = 28
    loop
        exitwhen i > endIndex
        if (btnNeutralAbilityImgEnabled[i] != null) then
            // Enabled
            set btnNeutralAbilityEnabled[i] = BlzCreateFrame("ScoreScreenBottomButtonTemplate", neutralAbilityMenu, 0, 0)
            call BlzFrameSetSize(btnNeutralAbilityEnabled[i], 0.04, 0.04)
            call BlzFrameSetAbsPoint(btnNeutralAbilityEnabled[i], FRAMEPOINT_CENTER, 0.34 + frameXPOS, 0.405 - frameYPOS)
            call BlzFrameSetVisible(btnNeutralAbilityEnabled[i], true)
            set imgFrame = BlzGetFrameByName("ScoreScreenButtonBackdrop", 0)
            call BlzFrameSetTexture(imgFrame, btnNeutralAbilityImgEnabled[i], 0, true)

            // Button trigger
            set trig = CreateTrigger()
            call BlzTriggerRegisterFrameEvent(trig, btnNeutralAbilityEnabled[i], FRAMEEVENT_CONTROL_CLICK)
            call TriggerAddAction(trig, function NeutralAbilityToggle)

            // Hover trigger
            set trig3 = CreateTrigger()
            call BlzTriggerRegisterFrameEvent(trig3, btnNeutralAbilityEnabled[i], FRAMEEVENT_MOUSE_ENTER)
            call TriggerAddAction(trig3, function NeutralAbilityHover)

            // Disabled
            set btnNeutralAbilityDisabled[i] = BlzCreateFrame("ScoreScreenBottomButtonTemplate", neutralAbilityMenu, 0, 0)
            call BlzFrameSetSize(btnNeutralAbilityDisabled[i], 0.04, 0.04)
            call BlzFrameSetAbsPoint(btnNeutralAbilityDisabled[i], FRAMEPOINT_CENTER, 0.34 + frameXPOS, 0.405 - frameYPOS)
            call BlzFrameSetVisible(btnNeutralAbilityDisabled[i], false)
            set imgFrame2 = BlzGetFrameByName("ScoreScreenButtonBackdrop", 0)
            call BlzFrameSetTexture(imgFrame2, btnNeutralAbilityImgDisabled[i], 0, true)

            // Button trigger
            set trig2 = CreateTrigger()
            call BlzTriggerRegisterFrameEvent(trig2, btnNeutralAbilityDisabled[i], FRAMEEVENT_CONTROL_CLICK)
            call TriggerAddAction(trig2, function NeutralAbilityToggle)

            // Hover trigger
            set trig4 = CreateTrigger()
            call BlzTriggerRegisterFrameEvent(trig4, btnNeutralAbilityDisabled[i], FRAMEEVENT_MOUSE_ENTER)
            call TriggerAddAction(trig4, function NeutralAbilityHover)

            set frameXPOS = frameXPOS + 0.032

            if (ModuloInteger(incr, 6) == 0) then
                set frameYPOS = frameYPOS + 0.032
                set frameXPOS = 0
            endif

            set incr = incr + 1
            endif

        set i = i + 1
    endloop
endfunction