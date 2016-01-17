
using System;

	/// <summary>
	/// An enum identifying types of instruments.
	/// </summary>
	public enum InstrumentType
	{
	NoInstrument = -1,
		Guitar = 1,
		Piano = 2,
		Timpani = 3,
		Flute = 4,
		Violin = 5,
		Keyboard = 6
	}
public static class InstrumentTypeHelpers {
	public static InstrumentType stringToInstrumentType(string str) {
		int[] values = (int[])(Enum.GetValues (typeof(InstrumentType)));
		foreach (int value in values) {
			if (Enum.GetName(typeof(InstrumentType), value) == str) {
				return (InstrumentType)value;
			}
		}
		return InstrumentType.NoInstrument;
	} 
}

