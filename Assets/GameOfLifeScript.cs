using System;
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
	public CellAnimationScript cellAnim;
	public string pattern;

	public float initialDelay = 0.5f;
	public float generationDuration = 0.5f;
	float nextGenTime = 0;

	Dictionary<Cell, CellAnimationScript> cells;

	void Start () {
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(File.ReadAllText(string.Format("tiled/{0}.tmx", pattern)));

		// Load the Tiled map layer from the XML document.
		XmlNode layer = xmlDoc.SelectSingleNode("map/layer");
		int width = int.Parse(layer.Attributes["width"].Value);
		int height = int.Parse(layer.Attributes["height"].Value);
		cells = new Dictionary<Cell, CellAnimationScript>();

		// Split the CSV data into rows, and parse them from bottom to top.
		string[] rows = layer["data"].InnerText.Trim().Split(new char[] {'\n'});
		Array.Reverse(rows);
		for (int y = 0; y < height; y++) {
			// Split each row into an array of ints.
			string[] cellsStr = rows[y].Split(new char[] {','});
			for (int x = 0; x < width; x++) {
				// Create a cell in the array for every nonzero int.
				if (int.Parse(cellsStr[x]) > 0) {
					CreateCell(x, y, true);
				}
			}
		}

		// Schedule the first generation update.
		nextGenTime = initialDelay;
	}

	void Update() {
		if (Time.time < nextGenTime) {
			return;
		}

		// Destroy any dead cells from the last iteration.
		foreach (Cell cell in new List<Cell>(cells.Keys.Where(cell => cells[cell].IsDead()))) {
			Destroy(cells[cell].gameObject);
			cells.Remove(cell);
		}

		// Get the next generation.
		HashSet<Cell> nextCells = GetNextGen();

		// Kill all existing cell animations that aren't in the next gen.
		foreach (Cell cell in cells.Keys.Where(cell => !nextCells.Contains(cell))) {
			cells[cell].Kill();
		}

		// Instantiate a cell animation for every new cell in the next gen.
		foreach (Cell cell in nextCells.Where(cell => !cells.ContainsKey(cell))) {
			CreateCell(cell.x, cell.y, false);
		}

		// Schedule the next generation update.
		nextGenTime += generationDuration;
	}

	void CreateCell(int x, int y, bool full) {
		CellAnimationScript anim = Instantiate(cellAnim, transform.position + transform.rotation * new Vector3(x, y, 0), transform.rotation);
		anim.transform.parent = transform;
		cells.Add(new Cell(x, y), anim);

		if (full) {
			anim.SkipToFull();
		}
	}

	HashSet<Cell> GetNextGen() {
		HashSet<Cell> nextCells = new HashSet<Cell>();

		// Get the bounding box for the current set of cells.
		int xmin = cells.Keys.Min(cell => cell.x);
		int xmax = cells.Keys.Max(cell => cell.x);
		int ymin = cells.Keys.Min(cell => cell.y);
		int ymax = cells.Keys.Max(cell => cell.y);

		// Loop through each coordinate in the bounding box + 1 in each direction.
		for (int x = xmin - 1; x <= xmax + 1; x++) {
			for (int y = ymin - 1; y <= ymax + 1; y++) {
				Cell cell = new Cell(x, y);
				int neighbours = 0;

				// Count the number of neighbours for this cell.
				for (int nx = x - 1; nx <= x + 1; nx++) {
					for (int ny = y - 1; ny <= y + 1; ny++) {
						if ((nx != x || ny != y) && cells.ContainsKey(new Cell(nx, ny))) {
							neighbours++;
						}
					}
				}

				// Determine whether there will be a cell in the next gen based on number of neighbours.
				if ((cells.ContainsKey(cell) && (neighbours == 2 || neighbours == 3)) || (!cells.ContainsKey(cell) && neighbours == 3)) {
					nextCells.Add(cell);
				}
			}
		}

		return nextCells;
	}
}
