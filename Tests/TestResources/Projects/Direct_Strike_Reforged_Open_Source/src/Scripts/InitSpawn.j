function InitSpawns takes nothing returns nothing
		
	// Human
	call SaveInteger(udg_SpawnTable, 0, 'h000', 'hfoo') // Footman
	call SaveInteger(udg_SpawnTable, 0, 'hfoo', 'h000')
	call SaveInteger(udg_SpawnTable, 0, 'h001', 'hrif') // Rifleman
	call SaveInteger(udg_SpawnTable, 0, 'hrif', 'h001')
	call SaveInteger(udg_SpawnTable, 0, 'h002', 'hkni') // Knight
	call SaveInteger(udg_SpawnTable, 0, 'hkni', 'h002')
	call SaveInteger(udg_SpawnTable, 0, 'h003', 'hmtm') // Mortar Team
	call SaveInteger(udg_SpawnTable, 0, 'hmtm', 'h003')
	call SaveInteger(udg_SpawnTable, 0, 'h01Q', 'hsor') // Sorceress
	call SaveInteger(udg_SpawnTable, 0, 'hsor', 'h01Q')
	call SaveInteger(udg_SpawnTable, 0, 'h01P', 'hmpr') // Priest
	call SaveInteger(udg_SpawnTable, 0, 'hmpr', 'h01P')
	call SaveInteger(udg_SpawnTable, 0, 'h006', 'hspt') // Spellbreaker
	call SaveInteger(udg_SpawnTable, 0, 'hspt', 'h006')
	call SaveInteger(udg_SpawnTable, 0, 'h007', 'hgyr') // Flying Machine
	call SaveInteger(udg_SpawnTable, 0, 'hgyr', 'h007')
	call SaveInteger(udg_SpawnTable, 0, 'h008', 'hmtt') // Siege Engine
	call SaveInteger(udg_SpawnTable, 0, 'hmtt', 'h008')
	call SaveInteger(udg_SpawnTable, 0, 'h009', 'hgry') // Gryphon Rider
	call SaveInteger(udg_SpawnTable, 0, 'hgry', 'h009')
	call SaveInteger(udg_SpawnTable, 0, 'h00A', 'hdhw') // Dragonhawk Rider
	call SaveInteger(udg_SpawnTable, 0, 'hdhw', 'h00A')

	call SaveInteger(udg_SpawnTable, 0, 'H01F', 'Hpal') // Paladin
	call SaveInteger(udg_SpawnTable, 0, 'Hpal', 'H01F')
	call SaveInteger(udg_SpawnTable, 0, 'H01I', 'Hamg') // Archmage
	call SaveInteger(udg_SpawnTable, 0, 'Hamg', 'H01I')
	call SaveInteger(udg_SpawnTable, 0, 'H01J', 'Hmkg') // Mountain King
	call SaveInteger(udg_SpawnTable, 0, 'Hmkg', 'H01J')
	call SaveInteger(udg_SpawnTable, 0, 'H01L', 'Hblm') // Blood Mage
	call SaveInteger(udg_SpawnTable, 0, 'Hblm', 'H01L')


	// Orc
	call SaveInteger(udg_SpawnTable, 0, 'h00B', 'ogru') // Grunt
	call SaveInteger(udg_SpawnTable, 0, 'ogru', 'h00B')
	call SaveInteger(udg_SpawnTable, 0, 'h00D', 'ohun') // Headhunter
	call SaveInteger(udg_SpawnTable, 0, 'ohun', 'h00D')
	call SaveInteger(udg_SpawnTable, 0, 'h00E', 'orai') // Raider
	call SaveInteger(udg_SpawnTable, 0, 'orai', 'h00E')
	call SaveInteger(udg_SpawnTable, 0, 'h00F', 'otau') // Tauren
	call SaveInteger(udg_SpawnTable, 0, 'otau', 'h00F')
	call SaveInteger(udg_SpawnTable, 0, 'h00G', 'ocat') // Demolisher
	call SaveInteger(udg_SpawnTable, 0, 'ocat', 'h00G')
	call SaveInteger(udg_SpawnTable, 0, 'h00H', 'okod') // Kodo Rider
	call SaveInteger(udg_SpawnTable, 0, 'okod', 'h00H')
	call SaveInteger(udg_SpawnTable, 0, 'h00I', 'owyv') // Wyvern
	call SaveInteger(udg_SpawnTable, 0, 'owyv', 'h00I')
	call SaveInteger(udg_SpawnTable, 0, 'h00J', 'otbr') // Batrider
	call SaveInteger(udg_SpawnTable, 0, 'otbr', 'h00J')
	call SaveInteger(udg_SpawnTable, 0, 'h00K', 'odoc') // Witch Doctor
	call SaveInteger(udg_SpawnTable, 0, 'odoc', 'h00K')
	call SaveInteger(udg_SpawnTable, 0, 'h00L', 'oshm') // Shaman
	call SaveInteger(udg_SpawnTable, 0, 'oshm', 'h00L')
	call SaveInteger(udg_SpawnTable, 0, 'h00M', 'ospw') // Spirit Walker
	call SaveInteger(udg_SpawnTable, 0, 'ospw', 'h00M')

	call SaveInteger(udg_SpawnTable, 0, 'O002', 'Obla') // Blademaster
	call SaveInteger(udg_SpawnTable, 0, 'Obla', 'O002')
	call SaveInteger(udg_SpawnTable, 0, 'O003', 'Ofar') // Far Seer
	call SaveInteger(udg_SpawnTable, 0, 'Ofar', 'O003')
	call SaveInteger(udg_SpawnTable, 0, 'O004', 'Otch') // Tauren Chieftain
	call SaveInteger(udg_SpawnTable, 0, 'Otch', 'O004')
	call SaveInteger(udg_SpawnTable, 0, 'O005', 'Oshd') // Shadow Hunter
	call SaveInteger(udg_SpawnTable, 0, 'Oshd', 'O005')


	// Undead
	call SaveInteger(udg_SpawnTable, 0, 'h00N', 'ugho') // Ghoul
	call SaveInteger(udg_SpawnTable, 0, 'ugho', 'h00N')
	call SaveInteger(udg_SpawnTable, 0, 'h00O', 'ucry') // Crypt Fiend
	call SaveInteger(udg_SpawnTable, 0, 'ucry', 'h00O')
	call SaveInteger(udg_SpawnTable, 0, 'h00P', 'uabo') // Abomination
	call SaveInteger(udg_SpawnTable, 0, 'uabo', 'h00P')
	call SaveInteger(udg_SpawnTable, 0, 'h00Q', 'umtw') // Meat Wagon
	call SaveInteger(udg_SpawnTable, 0, 'umtw', 'h00Q')
	call SaveInteger(udg_SpawnTable, 0, 'h00R', 'ugar') // Gargoyle
	call SaveInteger(udg_SpawnTable, 0, 'ugar', 'h00R')
	call SaveInteger(udg_SpawnTable, 0, 'h00S', 'uban') // Banshee
	call SaveInteger(udg_SpawnTable, 0, 'uban', 'h00S')
	call SaveInteger(udg_SpawnTable, 0, 'h00T', 'unec') // Necromancer
	call SaveInteger(udg_SpawnTable, 0, 'unec', 'h00T')
	call SaveInteger(udg_SpawnTable, 0, 'h00U', 'uobs') // Obsidian Statue
	call SaveInteger(udg_SpawnTable, 0, 'uobs', 'h00U')
	call SaveInteger(udg_SpawnTable, 0, 'h00V', 'ufro') // Frost Wyrm
	call SaveInteger(udg_SpawnTable, 0, 'ufro', 'h00V')
	call SaveInteger(udg_SpawnTable, 0, 'h00X', 'ubsp') // Destroyer
	call SaveInteger(udg_SpawnTable, 0, 'ubsp', 'h00X')
	call SaveInteger(udg_SpawnTable, 0, 'h00W', 'ushd') // Shade
	call SaveInteger(udg_SpawnTable, 0, 'ushd', 'h00W')

	call SaveInteger(udg_SpawnTable, 0, 'U005', 'Udea') // Death Knight
	call SaveInteger(udg_SpawnTable, 0, 'Udea', 'U005')
	call SaveInteger(udg_SpawnTable, 0, 'U006', 'Ulic') // Lich
	call SaveInteger(udg_SpawnTable, 0, 'Ulic', 'U006')
	call SaveInteger(udg_SpawnTable, 0, 'U007', 'Udre') // Dreadlord
	call SaveInteger(udg_SpawnTable, 0, 'Udre', 'U007')
	call SaveInteger(udg_SpawnTable, 0, 'U008', 'Ucrl') // Crypt Lord
	call SaveInteger(udg_SpawnTable, 0, 'Ucrl', 'U008')


	// Night Elf
	call SaveInteger(udg_SpawnTable, 0, 'e000', 'earc') // Archer
	call SaveInteger(udg_SpawnTable, 0, 'earc', 'e000')
	call SaveInteger(udg_SpawnTable, 0, 'e001', 'esen') // Huntress
	call SaveInteger(udg_SpawnTable, 0, 'esen', 'e001')
	call SaveInteger(udg_SpawnTable, 0, 'e002', 'edry') // Dryad
	call SaveInteger(udg_SpawnTable, 0, 'edry', 'e002')
	call SaveInteger(udg_SpawnTable, 0, 'e003', 'ebal') // Glaive Thrower
	call SaveInteger(udg_SpawnTable, 0, 'ebal', 'e003')
	call SaveInteger(udg_SpawnTable, 0, 'e004', 'ehip') // Hippogryph
	call SaveInteger(udg_SpawnTable, 0, 'ehip', 'e004')
	call SaveInteger(udg_SpawnTable, 0, 'e005', 'ehpr') // Hippogryph Rider
	call SaveInteger(udg_SpawnTable, 0, 'ehpr', 'e005')
	call SaveInteger(udg_SpawnTable, 0, 'e006', 'echm') // Chimaera
	call SaveInteger(udg_SpawnTable, 0, 'echm', 'e006')
	call SaveInteger(udg_SpawnTable, 0, 'e007', 'edot') // Druid of the Talon
	call SaveInteger(udg_SpawnTable, 0, 'edot', 'e007')
	call SaveInteger(udg_SpawnTable, 0, 'e008', 'edoc') // Druid of the Claw
	call SaveInteger(udg_SpawnTable, 0, 'edoc', 'e008')
	call SaveInteger(udg_SpawnTable, 0, 'e009', 'emtg') // Mountain Giant
	call SaveInteger(udg_SpawnTable, 0, 'emtg', 'e009')
	call SaveInteger(udg_SpawnTable, 0, 'e00A', 'efdr') // Faerie Dragon
	call SaveInteger(udg_SpawnTable, 0, 'efdr', 'e00A')

	call SaveInteger(udg_SpawnTable, 0, 'E00H', 'Ekee') // Keeper of the Grove
	call SaveInteger(udg_SpawnTable, 0, 'Ekee', 'E00H')
	call SaveInteger(udg_SpawnTable, 0, 'E00I', 'Emoo') // Priestess of the Moon
	call SaveInteger(udg_SpawnTable, 0, 'Emoo', 'E00I')
	call SaveInteger(udg_SpawnTable, 0, 'E00J', 'Edem') // Demon Hunter
	call SaveInteger(udg_SpawnTable, 0, 'Edem', 'E00J')
	call SaveInteger(udg_SpawnTable, 0, 'E00K', 'Ewar') // Warden
	call SaveInteger(udg_SpawnTable, 0, 'Ewar', 'E00K')


	// Tavern Heroes
	call SaveInteger(udg_SpawnTable, 0, 'N012', 'Nalc') // Alchemist
	call SaveInteger(udg_SpawnTable, 0, 'Nalc', 'N012')
	call SaveInteger(udg_SpawnTable, 0, 'N013', 'Nngs') // Sea Witch
	call SaveInteger(udg_SpawnTable, 0, 'Nngs', 'N013')
	call SaveInteger(udg_SpawnTable, 0, 'N014', 'Ntin') // Tinker
	call SaveInteger(udg_SpawnTable, 0, 'Ntin', 'N014')
	call SaveInteger(udg_SpawnTable, 0, 'N015', 'Nbst') // Beastmaster
	call SaveInteger(udg_SpawnTable, 0, 'Nbst', 'N015')
	call SaveInteger(udg_SpawnTable, 0, 'N016', 'Npbm') // Brewmaster
	call SaveInteger(udg_SpawnTable, 0, 'Npbm', 'N016')
	call SaveInteger(udg_SpawnTable, 0, 'N017', 'Nbrn') // Dark Ranger
	call SaveInteger(udg_SpawnTable, 0, 'Nbrn', 'N017')
	call SaveInteger(udg_SpawnTable, 0, 'N018', 'Nfir') // Firelord
	call SaveInteger(udg_SpawnTable, 0, 'Nfir', 'N018')
	call SaveInteger(udg_SpawnTable, 0, 'N019', 'Nplh') // Pit Lord
	call SaveInteger(udg_SpawnTable, 0, 'Nplh', 'N019')

endfunction