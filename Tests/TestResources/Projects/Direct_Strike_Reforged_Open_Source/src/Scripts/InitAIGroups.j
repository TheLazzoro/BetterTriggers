function InitAIGroups takes nothing returns nothing

	// Setup Unit Group Table

	// Human
	call SaveGroupHandle(udg_UnitTypeTable, 'hmpr', 0, udg_Priests)
	call SaveGroupHandle(udg_UnitTypeTable, 'nemi', 0, udg_Priests)
	call SaveGroupHandle(udg_UnitTypeTable, 'nwiz', 0, udg_Priests)
	call SaveGroupHandle(udg_UnitTypeTable, 'nchp', 0, udg_Priests)
	call SaveGroupHandle(udg_UnitTypeTable, 'hsor', 0, udg_Sorceress)
	call SaveGroupHandle(udg_UnitTypeTable, 'hspt', 0, udg_Spellbreaker)
	call SaveGroupHandle(udg_UnitTypeTable, 'hdhw', 0, udg_DragonhawkRiders)
	call SaveGroupHandle(udg_UnitTypeTable, 'nws1', 0, udg_DragonhawkRiders)
	call SaveGroupHandle(udg_UnitTypeTable, 'hmtm', 0, udg_MortarTeams)
	
	call SaveGroupHandle(udg_UnitTypeTable, 'Hpal', 0, udg_Paladins)
	call SaveGroupHandle(udg_UnitTypeTable, 'Hart', 0, udg_Paladins)
	call SaveGroupHandle(udg_UnitTypeTable, 'Huth', 0, udg_Paladins)
	call SaveGroupHandle(udg_UnitTypeTable, 'Hamg', 0, udg_Archmages)
	call SaveGroupHandle(udg_UnitTypeTable, 'Haah', 0, udg_Archmages)
	call SaveGroupHandle(udg_UnitTypeTable, 'Hgam', 0, udg_Archmages)
	call SaveGroupHandle(udg_UnitTypeTable, 'H01U', 0, udg_Archmages)
	call SaveGroupHandle(udg_UnitTypeTable, 'Hmkg', 0, udg_MountainKings)
	call SaveGroupHandle(udg_UnitTypeTable, 'Hmbr', 0, udg_MountainKings)
	call SaveGroupHandle(udg_UnitTypeTable, 'Hblm', 0, udg_BloodMages)
	call SaveGroupHandle(udg_UnitTypeTable, 'Hjai', 0, udg_BloodMages)
	call SaveGroupHandle(udg_UnitTypeTable, 'Hkal', 0, udg_BloodMages)
	

	// Orc
	call SaveGroupHandle(udg_UnitTypeTable, 'otbr', 0, udg_Batriders)
	call SaveGroupHandle(udg_UnitTypeTable, 'oshm', 0, udg_Shamans)
	call SaveGroupHandle(udg_UnitTypeTable, 'orai', 0, udg_Raiders)
	call SaveGroupHandle(udg_UnitTypeTable, 'nchr', 0, udg_Raiders)
	call SaveGroupHandle(udg_UnitTypeTable, 'ospw', 0, udg_SpiritWalkers)
	
	call SaveGroupHandle(udg_UnitTypeTable, 'Obla', 0, udg_Blademasters)
	call SaveGroupHandle(udg_UnitTypeTable, 'Nbbc', 0, udg_Blademasters)
	call SaveGroupHandle(udg_UnitTypeTable, 'Osam', 0, udg_Blademasters)
	call SaveGroupHandle(udg_UnitTypeTable, 'Ogrh', 0, udg_Blademasters)
	call SaveGroupHandle(udg_UnitTypeTable, 'Opgh', 0, udg_Blademasters)
	call SaveGroupHandle(udg_UnitTypeTable, 'O00D', 0, udg_Blademasters)
	call SaveGroupHandle(udg_UnitTypeTable, 'Ofar', 0, udg_FarSeers)
	call SaveGroupHandle(udg_UnitTypeTable, 'Odrt', 0, udg_FarSeers)
	call SaveGroupHandle(udg_UnitTypeTable, 'Othr', 0, udg_FarSeers)
	call SaveGroupHandle(udg_UnitTypeTable, 'Otch', 0, udg_TaurenChieftains)
	call SaveGroupHandle(udg_UnitTypeTable, 'Ocbh', 0, udg_TaurenChieftains)
	call SaveGroupHandle(udg_UnitTypeTable, 'Oshd', 0, udg_ShadowHunters)
	call SaveGroupHandle(udg_UnitTypeTable, 'Orkn', 0, udg_ShadowHunters)
	

	// Undead
	call SaveGroupHandle(udg_UnitTypeTable, 'ucry', 0, udg_CryptFiends)
	call SaveGroupHandle(udg_UnitTypeTable, 'nnwa', 0, udg_CryptFiends)
	call SaveGroupHandle(udg_UnitTypeTable, 'nnwq', 0, udg_CryptFiends)
	call SaveGroupHandle(udg_UnitTypeTable, 'unec', 0, udg_Necromancers)
	call SaveGroupHandle(udg_UnitTypeTable, 'uktg', 0, udg_Necromancers)
	call SaveGroupHandle(udg_UnitTypeTable, 'uktn', 0, udg_Necromancers)
	call SaveGroupHandle(udg_UnitTypeTable, 'uban', 0, udg_Banshees)
	call SaveGroupHandle(udg_UnitTypeTable, 'ngh2', 0, udg_Banshees)
	call SaveGroupHandle(udg_UnitTypeTable, 'uobs', 0, udg_ObsidianStatues)
	call SaveGroupHandle(udg_UnitTypeTable, 'ubsp', 0, udg_Destroyers)
	
	call SaveGroupHandle(udg_UnitTypeTable, 'Udea', 0, udg_DeathKnights)
	call SaveGroupHandle(udg_UnitTypeTable, 'Harf', 0, udg_DeathKnights)
	call SaveGroupHandle(udg_UnitTypeTable, 'H00Z', 0, udg_DeathKnights)
	call SaveGroupHandle(udg_UnitTypeTable, 'Ulic', 0, udg_Liches)
	call SaveGroupHandle(udg_UnitTypeTable, 'Uktl', 0, udg_Liches)
	call SaveGroupHandle(udg_UnitTypeTable, 'Udre', 0, udg_Dreadlords)
	call SaveGroupHandle(udg_UnitTypeTable, 'Udth', 0, udg_Dreadlords)
	call SaveGroupHandle(udg_UnitTypeTable, 'Utic', 0, udg_Dreadlords)
	call SaveGroupHandle(udg_UnitTypeTable, 'Ucrl', 0, udg_CryptLords)
	call SaveGroupHandle(udg_UnitTypeTable, 'Uanb', 0, udg_CryptLords)
	
		
	// Night Elf
	call SaveGroupHandle(udg_UnitTypeTable, 'edry', 0, udg_Dryads)
	call SaveGroupHandle(udg_UnitTypeTable, 'edot', 0, udg_DruidOfTheTalons)
	call SaveGroupHandle(udg_UnitTypeTable, 'edoc', 0, udg_DruidOfTheClaws)
	call SaveGroupHandle(udg_UnitTypeTable, 'efdr', 0, udg_FaerieDragons)
	
	call SaveGroupHandle(udg_UnitTypeTable, 'Ekee', 0, udg_KeeperOfTheGroves)
	call SaveGroupHandle(udg_UnitTypeTable, 'Ecen', 0, udg_KeeperOfTheGroves)
	call SaveGroupHandle(udg_UnitTypeTable, 'Ekgg', 0, udg_KeeperOfTheGroves)
	call SaveGroupHandle(udg_UnitTypeTable, 'Emns', 0, udg_KeeperOfTheGroves)
	call SaveGroupHandle(udg_UnitTypeTable, 'Emoo', 0, udg_PriestessOfTheMoons)
	call SaveGroupHandle(udg_UnitTypeTable, 'Etyr', 0, udg_PriestessOfTheMoons)
	call SaveGroupHandle(udg_UnitTypeTable, 'Edem', 0, udg_DemonHunters)
	call SaveGroupHandle(udg_UnitTypeTable, 'Eevi', 0, udg_DemonHunters)
	call SaveGroupHandle(udg_UnitTypeTable, 'Ewar', 0, udg_Wardens)
	call SaveGroupHandle(udg_UnitTypeTable, 'E00R', 0, udg_Wardens)
	call SaveGroupHandle(udg_UnitTypeTable, 'Ewrd', 0, udg_Wardens)
	
		
	// Tavern
	call SaveGroupHandle(udg_UnitTypeTable, 'Nalc', 0, udg_Alchemists)
	call SaveGroupHandle(udg_UnitTypeTable, 'Nngs', 0, udg_SeaWitches)
	call SaveGroupHandle(udg_UnitTypeTable, 'Ntin', 0, udg_Tinkers)
	call SaveGroupHandle(udg_UnitTypeTable, 'Nbst', 0, udg_Beastmasters)
	call SaveGroupHandle(udg_UnitTypeTable, 'Npbm', 0, udg_Brewmasters)
	call SaveGroupHandle(udg_UnitTypeTable, 'Nbrn', 0, udg_DarkRangers)
	call SaveGroupHandle(udg_UnitTypeTable, 'Nfir', 0, udg_Firelords)
	call SaveGroupHandle(udg_UnitTypeTable, 'Nplh', 0, udg_PitLords)

endfunction