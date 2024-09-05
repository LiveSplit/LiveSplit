state("project64")
{
	//Addresses in Little Endian instead of Big Endian
	byte400 Data                        : 0xD6A1C, 0x11A570;
	int     GameFrames                  : 0xD6A1C, 0x11F568;
	byte    SceneID                     : 0xD6A1C, 0x1C8546;
	sbyte   GohmasHealth                : 0xD6A1C, 0x1E840C;
	sbyte   GanonsHealth                : 0xD6A1C, 0x1FA2DC;
	short   GanonsAnimation             : 0xD6A1C, 0x1FA374;
	short   DialogID                    : 0xD6A1C, 0x1D8872;
	byte    IsOnTitleScreenOrFileSelect : 0xD6A1C, 0x11B92C;
	float   X                           : 0xD6A1C, 0x1C8714;
	float   Y                           : 0xD6A1C, 0x1C8718;
	float   Z                           : 0xD6A1C, 0x1C871C;
}

start
{	
	//Set Start Up State
	current.GameTime = TimeSpan.Zero;
	current.AccumulatedFrames = -current.GameFrames;
	
	current.EntranceID = 0xFFFF;
	current.CutsceneID = 0x0;
	
	current.EyeBallFrogCount = 
	current.LastActualDialog =
	current.LastActualDialogTime =
		0;
	
	current.HasSword =
	current.HasBottle =
	current.HasIceArrows =
	current.HasFaroresWind =
	current.HasSlingshot =
	current.HasBombchus =
	current.HasHookshot =
	current.HasIronBoots =
	current.HasHoverBoots =
	current.HasSongOfStorms =
	current.HasBombs =
	current.HasBoleroOfFire =
	current.HasRequiemOfSpirit =
	current.HasEyeBallFrog =
	current.HasMasterSword =
	current.HasBiggoronSword =
	current.HasBow =
	current.IsInFairyOcarinaCutscene =
	current.DidMidoSkip = 
		false;
	
	//Check for Timer Start
	return (old.SceneID == 0x17 || old.SceneID == 0x18)
		&& !(current.SceneID == 0x17 || current.SceneID == 0x18);
}

split
{
	//Functions
	Func<byte, byte, bool> check = (x, y) => (x & y) != 0x0;
	Func<int, byte> readFixed = x => current.Data[x ^ 0x3];
	Func<int, byte> getInventoryItem = slot => readFixed(0xD4 + slot);
	Func<short> getEntranceID = () => (short)(current.Data[0x61] << 8 | current.Data[0x60]);
	Func<ushort> getCutsceneID = () => (ushort)(current.Data[0x69] << 8 | current.Data[0x68]);
	Func<short, short, bool> checkEntrance = (x, y) => (x | 0x3) == (y | 0x3);
	
	//General Constants
	const byte INVENTORY_WIDTH = 6;
	
	//Flags
	const byte HAS_KOKIRI_SWORD = 0x1;
	const byte HAS_MASTER_SWORD = 0x2;
	const byte HAS_BIGGORON_SWORD = 0x4;
	const byte HAS_IRON_BOOTS = 0x20;
	const byte HAS_HOVER_BOOTS = 0x40;
	const byte HAS_REQUIEM_OF_SPIRIT = 0x2;
	const byte HAS_SONG_OF_STORMS = 0x2;
	const byte HAS_BOLERO_OF_FIRE = 0x80;
	
	const byte IS_ON_TITLE_SCREEN = 0x1;
	const byte IS_ON_FILE_SELECT = 0x2;
	
	//Scene IDs
	const byte DEKU_TREE = 0x00;
	const byte KAKARIKO = 0x52;
	const byte KOKIRI_FOREST = 0x55;
	const byte GOHMA = 0x11;
	
	//Entrance IDs
	//const short DEKU_TREE = 0x0;
	const short FOREST_TO_RIVER = 0x1DD;
	const short BRIDGE_BETWEEN_FIELD_AND_FOREST = 0x011E;
	const short WRONG_WARP_ENTRANCE = 0x256;
	const short VOLVAGIA_BATTLE = 0x305;
	const short GANONDORF_DEAD = 0x330;
	//const short GOHMA = 0x40F;
	const short GANON_BATTLE = 0x517;
	const short DODONGO_BATTLE = 0x40B;
	
	//Item IDs
	const byte BOMBS = 0x02;
	const byte BOW = 0x03;
	const byte SLINGSHOT = 0x06;
	const byte BOMBCHUS = 0x09;
	const byte HOOKSHOT = 0x0A;
	const byte ICE_ARROWS = 0x0C;
	const byte FARORES_WIND = 0x0D;
	const byte EMPTY_BOTTLE = 0x14;
	const byte EYE_BALL_FROG = 0x35;
	
	//Inventory Slots
	const byte BOMBS_SLOT = 2 + 0 * INVENTORY_WIDTH;
	const byte BOW_SLOT = 3 + 0 * INVENTORY_WIDTH;
	const byte SLINGSHOT_SLOT = 0 + 1 * INVENTORY_WIDTH;
	const byte OCARINA_SLOT = 1 + 1 * INVENTORY_WIDTH;
	const byte BOMBCHUS_SLOT = 2 + 1 * INVENTORY_WIDTH;
	const byte HOOKSHOT_SLOT = 3 + 1 * INVENTORY_WIDTH;
	const byte ICE_ARROWS_SLOT = 4 + 1 * INVENTORY_WIDTH;
	const byte FARORES_WIND_SLOT = 5 + 1 * INVENTORY_WIDTH;
	const byte BOTTLE_1 = 0 + 3 * INVENTORY_WIDTH;
	const byte BOTTLE_2 = 1 + 3 * INVENTORY_WIDTH;
	const byte BOTTLE_3 = 2 + 3 * INVENTORY_WIDTH;
	const byte BOTTLE_4 = 3 + 3 * INVENTORY_WIDTH;
	const byte ADULT_TRADE_ITEM = 4 + 3 * INVENTORY_WIDTH;
	const byte CHILD_TRADE_ITEM = 5 + 3 * INVENTORY_WIDTH;
	
	//Dialog IDs
	const short NO_DIALOG = 0x0000;
	const short FARORES_WIND_DIALOG = 0x003B;
	const short REQUIEM_OF_SPIRIT_DIALOG = 0x0076;
	const short SONG_OF_STORMS_DIALOG = 0x00D6;
	
	//Animation IDs
	const short GANON_FINAL_HIT = 0x3B1C;
	
	//Cutscene IDs
	const ushort FAIRY_OCARINA_CUTSCENE = 0xFFF0;
	
	//Check for Split
	var segment = timer.CurrentSplit.Name.ToLower();
	
	//Don't split on Title Screen or File Select because
	//Title Screen Link and Third File Link are loaded
	//and they might cause some splits to happen
	if (current.IsOnTitleScreenOrFileSelect != 0x0)
	{
		//TODO Except some segments
		return false;
	}
	
	if (segment == "sword" || segment == "kokiri sword")
	{
		var swordsAndShieldsUnlocked = current.Data[0xFE];
		current.HasSword = check(swordsAndShieldsUnlocked, HAS_KOKIRI_SWORD);
		return !old.HasSword && current.HasSword;
	}
	else if (segment == "master sword")
	{
		var swordsAndShieldsUnlocked = current.Data[0xFE];
		current.HasMasterSword = check(swordsAndShieldsUnlocked, HAS_MASTER_SWORD);
		return !old.HasMasterSword && current.HasMasterSword;
	}
	else if (segment == "biggoron sword" || segment == "biggoron's sword")
	{
		var swordsAndShieldsUnlocked = current.Data[0xFE];
		current.HasBiggoronSword = check(swordsAndShieldsUnlocked, HAS_BIGGORON_SWORD);
		return !old.HasBiggoronSword && current.HasBiggoronSword;
	}
	else if (segment == "hover boots")
	{
		var bootsAndTunicsUnlocked = current.Data[0xFF];
		current.HasHoverBoots = check(bootsAndTunicsUnlocked, HAS_HOVER_BOOTS);
		return !old.HasHoverBoots && current.HasHoverBoots;
	}
	else if (segment == "iron boots")
	{
		var bootsAndTunicsUnlocked = current.Data[0xFF];
		current.HasIronBoots = check(bootsAndTunicsUnlocked, HAS_IRON_BOOTS);
		return !old.HasIronBoots && current.HasIronBoots;
	}
	else if (segment == "song of storms")
	{
		var songsAndEmeraldsUnlocked = current.Data[0x106];
		current.HasSongOfStorms = check(songsAndEmeraldsUnlocked, HAS_SONG_OF_STORMS);
		return old.DialogID == SONG_OF_STORMS_DIALOG 
			&& current.DialogID == NO_DIALOG
			&& current.HasSongOfStorms;
	}
	else if (segment == "bolero of fire")
	{
		var songsAndMedallionsUnlocked = current.Data[0x104];
		current.HasBoleroOfFire = check(songsAndMedallionsUnlocked, HAS_BOLERO_OF_FIRE);
		return !old.HasBoleroOfFire && current.HasBoleroOfFire;
	}
	else if (segment == "requiem of spirit")
	{
		var songsUnlocked = current.Data[0x105];
		current.HasRequiemOfSpirit = check(songsUnlocked, HAS_REQUIEM_OF_SPIRIT);
		return old.DialogID == REQUIEM_OF_SPIRIT_DIALOG 
			&& current.DialogID == NO_DIALOG
			&& current.HasRequiemOfSpirit;
	}
	else if (segment == "bottle")
	{
		current.HasBottle = getInventoryItem(BOTTLE_1) == EMPTY_BOTTLE;
		return !old.HasBottle && current.HasBottle;
	}
	else if (segment.StartsWith("ice arrow"))
	{
		current.HasIceArrows = getInventoryItem(ICE_ARROWS_SLOT) == ICE_ARROWS;
		return !old.HasIceArrows && current.HasIceArrows;
	}
	else if (segment.StartsWith("farore's wind"))
	{
		current.HasFaroresWind = getInventoryItem(FARORES_WIND_SLOT) == FARORES_WIND;
		return !old.HasFaroresWind && current.HasFaroresWind;
	}
	else if (segment.EndsWith("bow"))
	{
		current.HasBow = getInventoryItem(BOW_SLOT) == BOW;
		return !old.HasBow && current.HasBow;
		//TODO Test
	}
	else if (segment.EndsWith("frog"))
	{
		current.HasEyeBallFrog = getInventoryItem(ADULT_TRADE_ITEM) == EYE_BALL_FROG;
		if (!old.HasEyeBallFrog && current.HasEyeBallFrog)
			current.EyeBallFrogCount++;
		return current.EyeBallFrogCount == 2 && old.EyeBallFrogCount < 2;
	}
	else if (segment == "slingshot")
	{
		current.HasSlingshot = getInventoryItem(SLINGSHOT_SLOT) == SLINGSHOT;
		return !old.HasSlingshot && current.HasSlingshot;
	}
	else if (segment == "bombchus")
	{
		current.HasBombchus = getInventoryItem(BOMBCHUS_SLOT) == BOMBCHUS;
		return !old.HasBombchus && current.HasBombchus;
	}
	else if (segment == "hookshot")
	{
		current.HasHookshot = getInventoryItem(HOOKSHOT_SLOT) == HOOKSHOT;
		return !old.HasHookshot && current.HasHookshot;
	}
	else if (segment == "bombs")
	{
		current.HasBombs = getInventoryItem(BOMBS_SLOT) == BOMBS;
		return !old.HasBombs && current.HasBombs;
	}
	else if (segment == "forest escape" || segment == "escape")
	{
		current.EntranceID = getEntranceID();
		current.CutsceneID = getCutsceneID();
		
		var escapedToRiver = 
			!checkEntrance(old.EntranceID, FOREST_TO_RIVER) 
			&& checkEntrance(current.EntranceID, FOREST_TO_RIVER);
		
		current.IsInFairyOcarinaCutscene = 
			checkEntrance(current.EntranceID, BRIDGE_BETWEEN_FIELD_AND_FOREST)
			&& current.CutsceneID == FAIRY_OCARINA_CUTSCENE;
		
		var escapedToSaria = 
			!old.IsInFairyOcarinaCutscene 
			&& current.IsInFairyOcarinaCutscene;
		
		return escapedToRiver || escapedToSaria;
	}
	else if (segment == "kakariko")
	{
		return old.SceneID != KAKARIKO && current.SceneID == KAKARIKO;
	}
	else if (segment == "mido skip")
	{
		current.DidMidoSkip = 
			current.SceneID == KOKIRI_FOREST
			&& current.X > 1600
			&& current.Y >= 0;
	
		return !old.DidMidoSkip && current.DidMidoSkip;
	}
	else if (segment == "deku tree")
	{
		return old.SceneID == KOKIRI_FOREST && current.SceneID == DEKU_TREE;
	}
	else if (segment == "gohma")
	{
		return current.SceneID == GOHMA
			&& old.GohmasHealth > 0 
			&& current.GohmasHealth <= 0;
	}
	else if (segment == "ganondorf" || segment == "wrong warp")
	{
		current.EntranceID = getEntranceID();
		return checkEntrance(old.EntranceID, WRONG_WARP_ENTRANCE) 
			&& !checkEntrance(current.EntranceID, WRONG_WARP_ENTRANCE);
	}
	else if (segment == "fire temple")
	{
		current.EntranceID = getEntranceID();
		return checkEntrance(old.EntranceID, VOLVAGIA_BATTLE) 
			&& !checkEntrance(current.EntranceID, VOLVAGIA_BATTLE);
		//TODO Test with wrong warp
	}
	else if (segment == "collapse" || segment == "tower collapse")
	{
		current.EntranceID = getEntranceID();
		return !checkEntrance(old.EntranceID, GANON_BATTLE) 
			&& checkEntrance(current.EntranceID, GANON_BATTLE);
	}
	else if (segment.EndsWith("warp in fire"))
	{
		if (current.DialogID != NO_DIALOG)
		{
			current.LastActualDialog = current.DialogID;
			current.LastActualDialogTime = current.GameFrames;
		}

		if (current.LastActualDialog == FARORES_WIND_DIALOG)
		{
			var delta = current.GameFrames - current.LastActualDialogTime;
			
			if (delta >= 30)
			{
				current.LastActualDialog = NO_DIALOG;
			}
			
			return current.X == 0 
				&& current.Y == 0 
				&& current.Z == 0;
		}
	}
	else if (segment.EndsWith("dodongo hc") || segment.EndsWith("dodongo heart container"))
	{
		current.EntranceID = getEntranceID();
		current.HeartContainers = current.Data[0x8c] >> 4;
		return checkEntrance(current.EntranceID, DODONGO_BATTLE)
			&& current.HeartContainers > old.HeartContainers;
	}
	else if (segment == "ganon")
	{
		current.EntranceID = getEntranceID();
		return checkEntrance(current.EntranceID, GANON_BATTLE)
			&& current.GanonsHealth <= 0
			&& old.GanonsAnimation != GANON_FINAL_HIT
			&& current.GanonsAnimation == GANON_FINAL_HIT;
	}
}

isLoading
{
	return true;
}

gameTime
{
	if (current.GameFrames < old.GameFrames)
		current.AccumulatedFrames += old.GameFrames;

	return TimeSpan.FromSeconds((current.GameFrames + current.AccumulatedFrames) / 20.0f);
}
