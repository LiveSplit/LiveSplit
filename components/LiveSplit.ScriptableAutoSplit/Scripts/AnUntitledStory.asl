state("Anuntitledstory")
{
	double time : 0x189720, 0x4, 0x358;
}

start
{
	return current.time == 1 && old.time == 0;
}

reset
{
	return old.time > current.time;
}

isLoading
{
	return false;
}

gameTime
{
	return TimeSpan.FromSeconds(current.time);
}
