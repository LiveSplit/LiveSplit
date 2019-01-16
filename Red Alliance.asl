state("Red Alliance")
{
	int loading1 : "mono.dll", 0x001F50AC, 0x24, 0x8, 0x84, 0xF4, 0x614;
	int loading2 : "UnityPlayer.dll", 0x00FF23C4, 0x30, 0xF8, 0x0, 0x0, 0x54; 
	int loading3 : "UnityPlayer.dll", 0x0100FD04, 0x68, 0x0, 0x8, 0x8C, 0x164; 
	int loading4 : "UnityPlayer.dll", 0x00FF23C8, 0x8, 0x4, 0x24, 0x18, 0x2E4; 
}

isLoading
{
	if( current.loading1 == 0 || current.loading2 == 0 || current.loading3 == 0 || current.loading4 == 0 )
	{
		return false;
	}
		return true;
}
