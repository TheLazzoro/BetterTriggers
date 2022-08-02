local define = System.defStc
local setmetatable = setmetatable

local handle = define("War3Api.Common.handle", {
})
local agent = define("War3Api.Common.agent", {
  base = { handle }
})
local event = define("War3Api.Common.event", {
  base = { agent }
})
local player = define("War3Api.Common.player", {
  base = { agent }
})
local widget = define("War3Api.Common.widget", {
  base = { agent }
})
local unit = define("War3Api.Common.unit", {
  base = { widget }
})
local destructable = define("War3Api.Common.destructable", {
  base = { widget }
})
local item = define("War3Api.Common.item", {
  base = { widget }
})
local ability = define("War3Api.Common.ability", {
  base = { agent }
})
local buff = define("War3Api.Common.buff", {
  base = { ability }
})
local force = define("War3Api.Common.force", {
  base = { agent }
})
local group = define("War3Api.Common.group", {
  base = { agent }
})
local trigger = define("War3Api.Common.trigger", {
  base = { agent }
})
local triggercondition = define("War3Api.Common.triggercondition", {
  base = { agent }
})
local triggeraction = define("War3Api.Common.triggeraction", {
  base = { handle }
})
local timer = define("War3Api.Common.timer", {
  base = { agent }
})
local location = define("War3Api.Common.location", {
  base = { agent }
})
local region = define("War3Api.Common.region", {
  base = { agent }
})
local rect = define("War3Api.Common.rect", {
  base = { agent }
})
local boolexpr = define("War3Api.Common.boolexpr", {
  base = { agent }
})
local sound = define("War3Api.Common.sound", {
  base = { agent }
})
local conditionfunc = define("War3Api.Common.conditionfunc", {
  base = { boolexpr }
})
local filterfunc = define("War3Api.Common.filterfunc", {
  base = { boolexpr }
})
local unitpool = define("War3Api.Common.unitpool", {
  base = { handle }
})
local itempool = define("War3Api.Common.itempool", {
  base = { handle }
})
local race = define("War3Api.Common.race", {
  base = { handle }
})
local alliancetype = define("War3Api.Common.alliancetype", {
  base = { handle }
})
local racepreference = define("War3Api.Common.racepreference", {
  base = { handle }
})
local gamestate = define("War3Api.Common.gamestate", {
  base = { handle }
})
local igamestate = define("War3Api.Common.igamestate", {
  base = { gamestate }
})
local fgamestate = define("War3Api.Common.fgamestate", {
  base = { gamestate }
})
local playerstate = define("War3Api.Common.playerstate", {
  base = { handle }
})
local playerscore = define("War3Api.Common.playerscore", {
  base = { handle }
})
local playergameresult = define("War3Api.Common.playergameresult", {
  base = { handle }
})
local unitstate = define("War3Api.Common.unitstate", {
  base = { handle }
})
local aidifficulty = define("War3Api.Common.aidifficulty", {
  base = { handle }
})
local eventid = define("War3Api.Common.eventid", {
  base = { handle }
})
local gameevent = define("War3Api.Common.gameevent", {
  base = { eventid }
})
local playerevent = define("War3Api.Common.playerevent", {
  base = { eventid }
})
local playerunitevent = define("War3Api.Common.playerunitevent", {
  base = { eventid }
})
local unitevent = define("War3Api.Common.unitevent", {
  base = { eventid }
})
local limitop = define("War3Api.Common.limitop", {
  base = { eventid }
})
local widgetevent = define("War3Api.Common.widgetevent", {
  base = { eventid }
})
local dialogevent = define("War3Api.Common.dialogevent", {
  base = { eventid }
})
local unittype = define("War3Api.Common.unittype", {
  base = { handle }
})
local gamespeed = define("War3Api.Common.gamespeed", {
  base = { handle }
})
local gamedifficulty = define("War3Api.Common.gamedifficulty", {
  base = { handle }
})
local gametype = define("War3Api.Common.gametype", {
  base = { handle }
})
local mapflag = define("War3Api.Common.mapflag", {
  base = { handle }
})
local mapvisibility = define("War3Api.Common.mapvisibility", {
  base = { handle }
})
local mapsetting = define("War3Api.Common.mapsetting", {
  base = { handle }
})
local mapdensity = define("War3Api.Common.mapdensity", {
  base = { handle }
})
local mapcontrol = define("War3Api.Common.mapcontrol", {
  base = { handle }
})
local minimapicon = define("War3Api.Common.minimapicon", {
  base = { handle }
})
local playerslotstate = define("War3Api.Common.playerslotstate", {
  base = { handle }
})
local volumegroup = define("War3Api.Common.volumegroup", {
  base = { handle }
})
local camerafield = define("War3Api.Common.camerafield", {
  base = { handle }
})
local camerasetup = define("War3Api.Common.camerasetup", {
  base = { handle }
})
local playercolor = define("War3Api.Common.playercolor", {
  base = { handle }
})
local placement = define("War3Api.Common.placement", {
  base = { handle }
})
local startlocprio = define("War3Api.Common.startlocprio", {
  base = { handle }
})
local raritycontrol = define("War3Api.Common.raritycontrol", {
  base = { handle }
})
local blendmode = define("War3Api.Common.blendmode", {
  base = { handle }
})
local texmapflags = define("War3Api.Common.texmapflags", {
  base = { handle }
})
local effect = define("War3Api.Common.effect", {
  base = { agent }
})
local effecttype = define("War3Api.Common.effecttype", {
  base = { handle }
})
local weathereffect = define("War3Api.Common.weathereffect", {
  base = { handle }
})
local terraindeformation = define("War3Api.Common.terraindeformation", {
  base = { handle }
})
local fogstate = define("War3Api.Common.fogstate", {
  base = { handle }
})
local fogmodifier = define("War3Api.Common.fogmodifier", {
  base = { agent }
})
local dialog = define("War3Api.Common.dialog", {
  base = { agent }
})
local button = define("War3Api.Common.button", {
  base = { agent }
})
local quest = define("War3Api.Common.quest", {
  base = { agent }
})
local questitem = define("War3Api.Common.questitem", {
  base = { agent }
})
local defeatcondition = define("War3Api.Common.defeatcondition", {
  base = { agent }
})
local timerdialog = define("War3Api.Common.timerdialog", {
  base = { agent }
})
local leaderboard = define("War3Api.Common.leaderboard", {
  base = { agent }
})
local multiboard = define("War3Api.Common.multiboard", {
  base = { agent }
})
local multiboarditem = define("War3Api.Common.multiboarditem", {
  base = { agent }
})
local trackable = define("War3Api.Common.trackable", {
  base = { agent }
})
local gamecache = define("War3Api.Common.gamecache", {
  base = { agent }
})
local version = define("War3Api.Common.version", {
  base = { handle }
})
local itemtype = define("War3Api.Common.itemtype", {
  base = { handle }
})
local texttag = define("War3Api.Common.texttag", {
  base = { handle }
})
local attacktype = define("War3Api.Common.attacktype", {
  base = { handle }
})
local damagetype = define("War3Api.Common.damagetype", {
  base = { handle }
})
local weapontype = define("War3Api.Common.weapontype", {
  base = { handle }
})
local soundtype = define("War3Api.Common.soundtype", {
  base = { handle }
})
local lightning = define("War3Api.Common.lightning", {
  base = { handle }
})
local pathingtype = define("War3Api.Common.pathingtype", {
  base = { handle }
})
local mousebuttontype = define("War3Api.Common.mousebuttontype", {
  base = { handle }
})
local animtype = define("War3Api.Common.animtype", {
  base = { handle }
})
local subanimtype = define("War3Api.Common.subanimtype", {
  base = { handle }
})
local image = define("War3Api.Common.image", {
  base = { handle }
})
local ubersplat = define("War3Api.Common.ubersplat", {
  base = { handle }
})
local hashtable = define("War3Api.Common.hashtable", {
  base = { agent }
})
local framehandle = define("War3Api.Common.framehandle", {
  base = { handle }
})
local originframetype = define("War3Api.Common.originframetype", {
  base = { handle }
})
local framepointtype = define("War3Api.Common.framepointtype", {
  base = { handle }
})
local textaligntype = define("War3Api.Common.textaligntype", {
  base = { handle }
})
local frameeventtype = define("War3Api.Common.frameeventtype", {
  base = { handle }
})
local oskeytype = define("War3Api.Common.oskeytype", {
  base = { handle }
})
local abilityintegerfield = define("War3Api.Common.abilityintegerfield", {
  base = { handle }
})
local abilityrealfield = define("War3Api.Common.abilityrealfield", {
  base = { handle }
})
local abilitybooleanfield = define("War3Api.Common.abilitybooleanfield", {
  base = { handle }
})
local abilitystringfield = define("War3Api.Common.abilitystringfield", {
  base = { handle }
})
local abilityintegerlevelfield = define("War3Api.Common.abilityintegerlevelfield", {
  base = { handle }
})
local abilityreallevelfield = define("War3Api.Common.abilityreallevelfield", {
  base = { handle }
})
local abilitybooleanlevelfield = define("War3Api.Common.abilitybooleanlevelfield", {
  base = { handle }
})
local abilitystringlevelfield = define("War3Api.Common.abilitystringlevelfield", {
  base = { handle }
})
local abilityintegerlevelarrayfield = define("War3Api.Common.abilityintegerlevelarrayfield", {
  base = { handle }
})
local abilityreallevelarrayfield = define("War3Api.Common.abilityreallevelarrayfield", {
  base = { handle }
})
local abilitybooleanlevelarrayfield = define("War3Api.Common.abilitybooleanlevelarrayfield", {
  base = { handle }
})
local abilitystringlevelarrayfield = define("War3Api.Common.abilitystringlevelarrayfield", {
  base = { handle }
})
local unitintegerfield = define("War3Api.Common.unitintegerfield", {
  base = { handle }
})
local unitrealfield = define("War3Api.Common.unitrealfield", {
  base = { handle }
})
local unitbooleanfield = define("War3Api.Common.unitbooleanfield", {
  base = { handle }
})
local unitstringfield = define("War3Api.Common.unitstringfield", {
  base = { handle }
})
local unitweaponintegerfield = define("War3Api.Common.unitweaponintegerfield", {
  base = { handle }
})
local unitweaponrealfield = define("War3Api.Common.unitweaponrealfield", {
  base = { handle }
})
local unitweaponbooleanfield = define("War3Api.Common.unitweaponbooleanfield", {
  base = { handle }
})
local unitweaponstringfield = define("War3Api.Common.unitweaponstringfield", {
  base = { handle }
})
local itemintegerfield = define("War3Api.Common.itemintegerfield", {
  base = { handle }
})
local itemrealfield = define("War3Api.Common.itemrealfield", {
  base = { handle }
})
local itembooleanfield = define("War3Api.Common.itembooleanfield", {
  base = { handle }
})
local itemstringfield = define("War3Api.Common.itemstringfield", {
  base = { handle }
})
local movetype = define("War3Api.Common.movetype", {
  base = { handle }
})
local targetflag = define("War3Api.Common.targetflag", {
  base = { handle }
})
local armortype = define("War3Api.Common.armortype", {
  base = { handle }
})
local heroattribute = define("War3Api.Common.heroattribute", {
  base = { handle }
})
local defensetype = define("War3Api.Common.defensetype", {
  base = { handle }
})
local regentype = define("War3Api.Common.regentype", {
  base = { handle }
})
local unitcategory = define("War3Api.Common.unitcategory", {
  base = { handle }
})
local pathingflag = define("War3Api.Common.pathingflag", {
  base = { handle }
})
local commandbuttoneffect = define("War3Api.Common.commandbuttoneffect", {
  base = { handle }
})
