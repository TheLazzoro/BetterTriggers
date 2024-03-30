{
  "Id": 50331969,
  "Comment": "",
  "IsScript": true,
  "RunOnMapInit": true,
  "Script": "function Trig_Set_Map_Music_Actions takes nothing returns nothing\r\n    set udg_MapMusic = \"Sound\\\\Music\\\\mp3Music\\\\Human1.mp3;Sound\\\\Music\\\\mp3Music\\\\Human2.mp3;Sound\\\\Music\\\\mp3Music\\\\Human3.mp3;Sound\\\\Music\\\\mp3Music\\\\HumanX1.mp3;Sound\\\\Music\\\\mp3Music\\\\Orc1.mp3;Sound\\\\Music\\\\mp3Music\\\\Orc2.mp3;Sound\\\\Music\\\\mp3Music\\\\Orc3.mp3;Sound\\\\Music\\\\mp3Music\\\\OrcX1.mp3;Sound\\\\Music\\\\mp3Music\\\\Undead1.mp3;Sound\\\\Music\\\\mp3Music\\\\Undead2.mp3;Sound\\\\Music\\\\mp3Music\\\\Undead3.mp3;Sound\\\\Music\\\\mp3Music\\\\UndeadX1.mp3;Sound\\\\Music\\\\mp3Music\\\\NightElf1.mp3;Sound\\\\Music\\\\mp3Music\\\\NightElf2.mp3;Sound\\\\Music\\\\mp3Music\\\\NightElf3.mp3;Sound\\\\Music\\\\mp3Music\\\\NightElfX1.mp3;Sound\\\\Music\\\\mp3Music\\\\NagaTheme.mp3;Sound\\\\Music\\\\mp3Music\\\\BloodElfTheme.mp3;Sound\\\\Music\\\\mp3Music\\\\ArthasTheme.mp3;Sound\\\\Music\\\\mp3Music\\\\IllidansTheme.mp3;Sound\\\\Music\\\\mp3Music\\\\PursuitTheme.mp3;Sound\\\\Music\\\\mp3Music\\\\War3XMainScreen.mp3;Sound\\\\Music\\\\mp3Music\\\\Mainscreen.mp3;\"\r\n    call SetMapMusic(udg_MapMusic, true, 0)\r\nendfunction\r\n\r\n//===========================================================================\r\nfunction InitTrig_Set_Map_Music takes nothing returns nothing\r\n    set gg_trg_Set_Map_Music = CreateTrigger(  )\r\n    call TriggerAddAction( gg_trg_Set_Map_Music, function Trig_Set_Map_Music_Actions )\r\nendfunction\r\n\r\n",
  "Events": [],
  "LocalVariables": [],
  "Conditions": [],
  "Actions": []
}