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

	int width;
	int height;
	bool[,] cells;

	float frameTime = 1;
	float nextFrame;

	void Start () {
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(File.ReadAllText(string.Format("tiled/{0}.tmx", pattern)));

		// Load the Tiled map layer from the XML document.
		XmlNode layer = xmlDoc.SelectSingleNode("map/layer");
		width = int.Parse(layer.Attributes["width"].Value);
		height = int.Parse(layer.Attributes["height"].Value);
		cells = new bool[width, height];

		// Parse the CSV data into an array of ints.
		string[] cellsStr = layer["data"].InnerText.Split(new string[] {","}, System.StringSplitOptions.None);

		// Create a cell in the array for every nonzero int.
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				cells[x, y] = int.Parse(cellsStr[y * width + x]) > 0;
				if (cells[x, y]) {
					Instantiate(cell, transform.position + new Vector3(x, y, 0), Quaternion.identity);
				}
			}
		}

		// Schedule the next frame.
		nextFrame = frameTime;
	}

	void Update () {
		if (Time.time < nextFrame) {
			return;
		}

		// Destroy each cell before generating new ones. (Temporary quick and dirty method)
		foreach (GameObject c in GameObject.FindGameObjectsWithTag("Cell")) {
			Destroy(c);
		}

		// Get the next generation.
		cells = GetNextGen();

		// Instantiate a cell animation for every cell in the array.
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				if (cells[x, y]) {
					Instantiate(cell, transform.position + new Vector3(x, y, 0), Quaternion.identity);
				}
			}
		}

		// Schedule the next frame.
		nextFrame += frameTime;
	}

	bool[,] GetNextGen() {
		bool[,] nextCells = new bool[width, height];

		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				int neighbours = 0;

				// Count the number of neighbours for this cell.
				for (int ny = Mathf.Max(0, y - 1); ny <= y + 1 && ny < height; ny++) {
					for (int nx = Mathf.Max(0, x - 1); nx <= x + 1 && nx < width; nx++) {
						Debug.Log(string.Format("{0} {1} {2} {3}", x, y, nx, ny));
						if ((x != nx || y != ny) && cells[nx, ny]) {
							neighbours++;
						}
					}
				}

				// Determine whether there will be a cell in the next gen based on number of neighbours.
				nextCells[x, y] =
					(cells[x, y] && neighbours > 1 && neighbours < 4) ||
					(!cells[x, y] && neighbours == 3);
			}
		}

		return nextCells;
	}
}
