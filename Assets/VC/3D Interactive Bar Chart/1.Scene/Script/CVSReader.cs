using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BarGraph.VittorCloud;

public class CSVReader
{
	static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	static char[] TRIM_CHARS = { '\"' };

	public static BarGraphDataSet ReadDataset(String file)
	{
		var entry = new BarGraphDataSet();
		entry.GroupName = file;
		entry.barColor = new Color(0, 143, 215);
		entry.barMaterial = null;
		entry.ListOfBars = new List<XYBarValues>();

		TextAsset data = Resources.Load (file) as TextAsset;
		var lines = Regex.Split (data.text, LINE_SPLIT_RE);

		if(lines.Length <= 1) return entry;

		// assuming first line has header information:
		var header = Regex.Split(lines[0], SPLIT_RE);

		// for each line (record) in the file
		for(var i=1; i < lines.Length; i++) {
			// split into multiple values
			var values = Regex.Split(lines[i], SPLIT_RE);
			if(values.Length == 0 || values.Length != 2 || values[0] == "") continue;

			var barValues = new XYBarValues();
			barValues.XValue = values[0];
			float f;
			if (float.TryParse(values[1], out f)) {
				barValues.YValue = f;
			} else {
				barValues.YValue = 0;
			}
			entry.ListOfBars.Add(barValues);
		}
		return entry;
	}

	public static List<Dictionary<string, object>> Read(string file)
	{
		var list = new List<Dictionary<string, object>>();
		TextAsset data = Resources.Load (file) as TextAsset;

		var lines = Regex.Split (data.text, LINE_SPLIT_RE);

		if(lines.Length <= 1) return list;

		var header = Regex.Split(lines[0], SPLIT_RE);
		for(var i=1; i < lines.Length; i++) {

			var values = Regex.Split(lines[i], SPLIT_RE);
			if(values.Length == 0 ||values[0] == "") continue;

			var entry = new Dictionary<string, object>();
			for(var j=0; j < header.Length && j < values.Length; j++ ) {
				string value = values[j];
				value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
				object finalvalue = value;
				int n;
				float f;
				if(int.TryParse(value, out n)) {
					finalvalue = n;
				} else if (float.TryParse(value, out f)) {
					finalvalue = f;
				}
				entry[header[j]] = finalvalue;
			}
			list.Add (entry);
		}
		return list;
	}
}
