state("javaw")
{
}

start
{
	vars.startTime = -1;
	vars.gameTime = -1;
	vars.realTime = TimeSpan.Zero;
	vars.realTimeDelta = TimeSpan.Zero;
}

isLoading
{
	return vars.realTimeDelta.TotalSeconds < 4.5;
}

gameTime
{
	var path = System.IO.Path.Combine(Environment.GetEnvironmentVariable("appdata"), ".minecraft\\stats");
	var source = System.IO.Directory.EnumerateFiles(path, "stats_*_unsent.dat");
	if (source.Any())
	{
		var lines = System.IO.File.ReadAllLines(source.First());
		foreach (var line in lines)
		{
			var trimmed = line.Trim();
			if (trimmed.StartsWith("{\"1100\":"))
			{
				var value = trimmed.Substring(8);
				var num = 0;
				for (int i = 0; i < value.Length; i++)
				{
					char c = value[i];
					if (!char.IsDigit(c))
					{
						break;
					}
					num = 10 * num + (int)(c - '0');
				}
				if (vars.startTime == -1)
				{
					vars.startTime = num;
				}
				var prevGameTime = vars.gameTime;
				vars.gameTime = (num - vars.startTime) * 50;
				if (vars.gameTime != prevGameTime)
				{
					var prevRealTime = vars.realTime;
					vars.realTime = timer.CurrentTime.RealTime;
					vars.realTimeDelta = vars.realTime - prevRealTime;
					return new TimeSpan(0, 0, 0, 0, vars.gameTime);
				}
			}
		}
	}
}
