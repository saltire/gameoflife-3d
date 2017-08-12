using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEditor;

public class GameOfLifeScript : MonoBehaviour {
	public GameObject cell;
	public string pattern;

	bool[,] cells;

	void Start () {
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(File.ReadAllText(string.Format("tiled/{0}.tmx", pattern)));

		// Load the Tiled map layer from the XML document.
		XmlNode layer = xmlDoc.SelectSingleNode("map/layer");
		int width = int.Parse(layer.Attributes["width"].Value);
		int height = int.Parse(layer.Attributes["height"].Value);
		cells = new bool[width, height];

		// Parse the CSV data into an array of ints.
		string[] cellsStr = layer["data"].InnerText.Split(new string[] {","}, System.StringSplitOptions.None);

		// Instantiate a cell animation for every nonzero int.
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				cells[x, y] = int.Parse(cellsStr[y * width + x]) > 0;
				if (cells[x, y]) {
					Instantiate(cell, transform.position + new Vector3(x, y, 0), Quaternion.identity);
				}
			}
		}
	}

	void Update () {

	}
}
