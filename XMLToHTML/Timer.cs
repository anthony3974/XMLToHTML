using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

// Version 1.3

public class Timer
{
	DateTime startTime;
	DateTime endTime;
	List<TimeSpan> advrageTimes = new List<TimeSpan>();

	public void Start()
	{
		// set the start time
		startTime = DateTime.Now;
	}
	public void Stop()
	{
		// set the endtime
		endTime = DateTime.Now;
	}
	public TimeSpan Get()
	{
		// gets the diffrence of start minus stop
		return startTime - endTime;
	}
	public TimeSpan StopGet()
	{
		// stops the timer and returns the diffrence
		Stop();
		return Get();

	}
	public void Reset()
	{
		// reset the list 
		advrageTimes = new List<TimeSpan>();
	}
	public void Add()
	{
		// add to the list
		advrageTimes.Add(startTime - DateTime.Now);

	}
	public TimeSpan GetAdvrage()
	{
		// for loop that adds the values and divids it by the lenght
		TimeSpan t = new TimeSpan();
		foreach (TimeSpan x in advrageTimes) t += x;
		return new TimeSpan(t.Ticks / (advrageTimes.Count));
	}
}
