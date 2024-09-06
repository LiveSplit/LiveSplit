state("OctodadDadliestCatch")
{
	string48 levelName  : 0x23FD88, 0xd8, 0x0C, 0x4C, 0x0;
	bool     isLoading  : 0x23FD8C, 0x4C, 0x8C; 
	float    levelTimer : 0x23FD88, 0xD8, 0x08;
}

start
{
	return (current.levelName == "" || current.levelName.StartsWith("MainScreen_Background")) 
		&& current.isLoading && !old.isLoading;
}

split
{
	return !old.isLoading && current.isLoading
		&& !old.levelName.StartsWith("MainScreen_Background")
		&& !old.levelName.StartsWith("OpeningCredits")
		&& !(timer.CurrentSplitIndex >= 5 && timer.CurrentSplitIndex <= 7 && old.levelName.StartsWith("Aquarium_Hub"));
}

isLoading
{
	return current.isLoading;
}
