state("NecroDancer")
{
	int  ZoneID     : 0x06DA40, 0x230;
	int  LevelID    : 0x06DA40, 0x2C4;
	int  LevelTime  : 0x06DA40, 0x10C;
	int  Health     : 0x18EF38, 0x014, 0x130;
	byte GamePaused : 0x06DA40, 0x088;
}

start
{
	vars.GameTime = 0;
	vars.AccumulatedTime = 0;//-current.LevelTime;
	
	var isNotInMainRoom = !(current.ZoneID == 1 && current.LevelID == -2);
	var isInANewLevel = (old.ZoneID != current.ZoneID 
						|| old.LevelID != current.LevelID);
	var isRevived = old.Health <= 0 && current.Health > 0;
	
	return isNotInMainRoom && (isInANewLevel || isRevived);
}

split
{
	return old.ZoneID != current.ZoneID
		|| old.LevelID != current.LevelID;
}

reset
{
	return current.Health <= 0
	 || (current.ZoneID == 1 && current.LevelID == -2);
}

isLoading
{
	return false;
}

gameTime
{
	if (current.LevelTime < old.LevelTime 
		&& !(current.ZoneID == 1 && current.LevelID == 1))
		current.AccumulatedTime += old.LevelTime;

	var prevGameTime = vars.GameTime;
	vars.GameTime = current.LevelTime + current.AccumulatedTime;
	if (vars.GameTime != prevGameTime || current.GamePaused != 0)
		return TimeSpan.FromMilliseconds(vars.GameTime);
}
