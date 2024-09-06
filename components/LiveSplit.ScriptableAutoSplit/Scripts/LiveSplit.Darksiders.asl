state("DarksidersPC")
{
	int time : 0x122E594, 0x53C, 0x74;
}

start
{
	return current.time == 1;
}

isLoading
{
	return true;
}

gameTime
{
	return TimeSpan.FromSeconds(current.time);
}
