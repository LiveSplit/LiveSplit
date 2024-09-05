state("project64")
{
	byte Stars      : 0xD6A1C, 0x33B218;
	int  GameFrames : 0xD6A1C, 0x32D5D4;
}

start
{
	vars.AccumulatedFrames = 0;
	vars.StarTime = 0;

	return old.GameFrames > 0 && current.GameFrames == 0;
}

split
{
	if (old.Stars < current.Stars)
		vars.StarTime = current.GameFrames;

	if (vars.StarTime > 0)
	{
		var delta = current.GameFrames - vars.StarTime;
		
		if (delta > 120)
		{
			vars.StarTime = 0;
			return true;
		}
	}
}

isLoading
{
	return false;
}

gameTime
{
	if (current.GameFrames < old.GameFrames)
		vars.AccumulatedFrames += old.GameFrames;

	return TimeSpan.FromSeconds((current.GameFrames + vars.AccumulatedFrames) / 30.0f);
}
