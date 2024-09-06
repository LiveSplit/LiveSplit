state("snes9x")
{
	byte  isInALevel  : 0x2EFBA4, 0x2904A;
	short levelFrames : 0x2EFBA4, 0x003A9;
}

start
{
	vars.gameTime = TimeSpan.Zero;
	return old.isInALevel == 4 && current.isInALevel == 0;
}

split
{
	if (current.levelFrames < old.levelFrames)
		vars.gameTime += TimeSpan.FromSeconds(old.levelFrames / 60.0f);	

	return old.isInALevel == 1 && current.isInALevel == 0;
}

isLoading
{
	return true;
}

gameTime
{
	return vars.gameTime + TimeSpan.FromSeconds(current.levelFrames / 60.0f);
}
