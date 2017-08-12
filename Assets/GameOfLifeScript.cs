using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEditor;

// A simple 2D coordinate struct with value equality.
public struct Cell
{
	public int x, y;
	public Cell(int x, int y) {
		this.x = x;
		this.y = y;
	}
	public static bool operator ==(Cell c1, Cell c2) {
		return c1.x == c2.x && c1.y == c2.y;
	}
	public static bool operator !=(Cell c1, Cell c2) {
		return c1.x != c2.x || c1.y != c2.y;
	}
	public override bool Equals(object obj) {
		if (!(obj is Cell)) {
			return false;
		}
		Cell c = (Cell)obj;
		return c.x == x && c.y == y;
	}
	public override int GetHashCode() {
		return x << 16 ^ y;
	}
}

public class GameOfLifeScript : MonoBehaviour {
	public GameObject cell;
	public string pattern;

	HashSet<Cell> cells;

	float frameTime = 1;
	float nextFrame;

	void Start () {
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(File.ReadAllText(string.Format("tiled/{0}.tmx", pattern)));

		// Load the Tiled map layer from the XML document.
		XmlNode layer = xmlDoc.SelectSingleNode("map/layer");
		int width = int.Parse(layer.Attributes["width"].Value);
		int height = int.Parse(layer.Attributes["height"].Value);
		cells = new HashSet<Cell>();

		// Parse the CSV data into an array of ints.
		string[] cellsStr = layer["data"].InnerText.Split(new string[] {","}, System.StringSplitOptions.None);

		// Create a cell in the array for every nonzero int.
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				if (int.Parse(cellsStr[y * width + x]) > 0) {
					cells.Add(new Cell(x, y));
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
		foreach (Cell c in cells) {
			Instantiate(cell, transform.position + new Vector3(c.x, c.y, 0), Quaternion.identity);
		}

		// Schedule the next frame.
		nextFrame += frameTime;
	}

	HashSet<Cell> GetNextGen() {
		HashSet<Cell> nextCells = new HashSet<Cell>();

		// Get the bounding box for the current set of cells.
		int xmin = cells.Select(cell => cell.x).Min();
		int xmax = cells.Select(cell => cell.x).Max();
		int ymin = cells.Select(cell => cell.y).Min();
		int ymax = cells.Select(cell => cell.y).Max();

		// Loop through each coordinate in the bounding box + 1 in each direction.
		for (int x = xmin - 1; x <= xmax + 1; x++) {
			for (int y = ymin - 1; y <= ymax + 1; y++) {
				Cell cell = new Cell(x, y);
				int neighbours = 0;

				// Count the number of neighbours for this cell.
				for (int nx = x - 1; nx <= x + 1; nx++) {
					for (int ny = y - 1; ny <= y + 1; ny++) {
						if ((nx != x || ny != y) && cells.Contains(new Cell(nx, ny))) {
							neighbours++;
						}
					}
				}

				// Determine whether there will be a cell in the next gen based on number of neighbours.
				if ((cells.Contains(cell) && (neighbours == 2 || neighbours == 3)) || (!cells.Contains(cell) && neighbours == 3)) {
					nextCells.Add(cell);
				}
			}
		}

		return nextCells;
	}
}
