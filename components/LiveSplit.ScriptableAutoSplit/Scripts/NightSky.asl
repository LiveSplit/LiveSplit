state("NightSky")
{
	bool isLoading : 0x211490, 0x498, 0x035;
	int  chapterID : 0x211490, 0x0A4, 0x1B0;
}

start
{
	return old.chapterID == 20239 && current.chapterID == 0;
}

split
{
	return !old.isLoading && current.isLoading && current.chapterID != 0;
}

isLoading
{
	return current.isLoading;
}
