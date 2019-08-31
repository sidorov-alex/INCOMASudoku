using System;
using System.Collections.Generic;
using System.Linq;

namespace INCOMASudoku.Services
{
	/// <summary>
	/// Содержит игровую логику.
	/// </summary>
	public class GameService : IGameService
	{
		/// <summary>
		/// Игровое поле.
		/// </summary>
		private Cell[][] cells;

		/// <summary>
		/// Таблица результатов.
		/// </summary>
		private List<ResultEntry> results = new List<ResultEntry>();

		const int BoardSize = 9;
		const int BlockSize = 3;
		readonly int[] BlockBaseIndexes = new[] { 0, 3, 6 };
		
		/// <summary>
		/// Инициализирует новый экземпляр класса.
		/// </summary>
		public GameService()
		{
			GenerateNewSudoku();
		}
		
		/// <summary>
		/// Возвращает текущее игровое поле.
		/// </summary>
		/// <returns></returns>
		public Cell[][] GetCells()
		{
			return this.cells;
		}
		
		/// <summary>
		/// Генерирует новую головоломку судоку.
		/// </summary>
		public void GenerateNewSudoku()
		{
			int[][] board = new int[][]
			{
				new int[] { 2, 0, 6,  9, 0, 7,  8, 0, 4 },	// 2 1 6  9 5 7  8 3 4
				new int[] { 0, 9, 0,  0, 0, 0,  0, 5, 0 },	// 7 9 4  3 1 8  2 5 6
				new int[] { 0, 0, 0,  2, 4 ,6,  0, 0, 0 },	// 5 3 8  2 4 6  7 9 1

				new int[] { 0, 0, 3,  0, 8, 0,  6, 0, 0 },	// 1 7 3  5 8 2  6 4 9
				new int[] { 8, 6, 0,  0, 0, 0,  0, 7, 3 },	// 8 6 2  4 9 1  5 7 3
				new int[] { 0, 0, 5,  0, 7, 0,  1, 0, 0 },	// 9 4 5  6 7 3  1 8 2

				new int[] { 0, 0, 0,  8, 3, 4,  0, 0, 0 },	// 6 5 1  8 3 4  9 2 7
				new int[] { 0, 2, 0,  0, 0, 0,  0, 1, 0 },	// 4 2 9  7 6 5  3 1 8
				new int[] { 3, 0, 7,  1, 0, 9,  4, 0, 5 }	// 3 8 7  1 2 9  4 6 5
			};

			// Заполняем игровое поле и для всех клеток, где есть цифра, указываем, что это - базовая клетка,
			// чтобы она выделялась жирным шрифтом.

			this.cells = new Cell[9][];

			for (int x = 0; x < this.cells.Length; x++)
			{
				this.cells[x] = new Cell[9];

				for (int y = 0; y < this.cells[x].Length; y++)
				{
					this.cells[x][y] = new Cell()
					{
						N = board[x][y],
						IsBase = board[x][y] != 0
					};
				}
			}
		}

		/// <summary>
		/// Добавляет результат в таблицу результатов.
		/// </summary>
		/// <param name="dateTime">Дата и время победы.</param>
		/// <param name="playerName">Имя игрока.</param>
		public void AddResult(DateTime dateTime, string playerName)
		{
			this.results.Add(new ResultEntry() { DateTime = dateTime, PlayerName = playerName });
		}

		/// <summary>
		/// Возвращает список победителей.
		/// </summary>
		/// <returns></returns>
		public string[] GetResults()
		{
			return this.results.OrderByDescending(r => r.DateTime).Select(r => r.PlayerName).ToArray();
		}

		/// <summary>
		/// Проверяет, решено ли судоку.
		/// </summary>
		public bool IsSudokuSolved()
		{
			// Проверяем, что в каждой строке содержатся все уникальные цифры.

			for (int x = 0; x < BoardSize; x++)
			{
				if (this.cells[x].Any(c => c.N == 0))
					return false;

				if (this.cells[x].Distinct().Count() != BoardSize)
					return false;
			}

			// Проверяем, что в каждой колонку содержаться все уникальные цифры.

			for (int y = 0; y < BoardSize; y++)
			{
				var items = new List<int>(BoardSize);

				for (int x = 0; x < BoardSize; x++)
				{
					if (this.cells[x][y].N == 0)
						return false;

					items.Add(this.cells[x][y].N);
				}

				if (items.Distinct().Count() != BoardSize)
					return false;
			}

			// Проверяем, что в каждом блоке содержаться все уникальные цифры.

			foreach (int x in BlockBaseIndexes)
			{
				foreach (int y in BlockBaseIndexes)
				{
					var items = new List<int>(BlockSize*BlockSize);

					for (int i = 0; i < BlockSize; i++)
					{
						for (int j = 0; j < BlockSize; j++)
						{
							if (this.cells[x + i][y + j].N == 0)
								return false;

							items.Add(this.cells[x + i][y + j].N);
						}
					}

					if (items.Distinct().Count() != BlockSize * BlockSize)
						return false;
				}
			}

			return true;
		}
		
		/// <summary>
		/// Проверяет будет ли конфликт в строке, если поставить указанную цифру в указанную клетку.
		/// </summary>
		/// <param name="x">X координата клетки.</param>
		/// <param name="y">Y координата клетки.</param>
		/// <param name="n">Цифра.</param>
		private bool CheckForConflictInRow(int x, int y, int n)
		{
			for (int i = 0; i < BoardSize; i++)
			{
				if (i != x &&
					this.cells[i][y].N == n)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Проверяет будет ли конфликт в колонке, если поставить указанную цифру в указанную клетку.
		/// </summary>
		/// <param name="x">X координата клетки.</param>
		/// <param name="y">Y координата клетки.</param>
		/// <param name="n">Цифра.</param>
		private bool CheckForConflictInColumn(int x, int y, int n)
		{
			for (int i = 0; i < BoardSize; i++)
			{
				if (i != y &&
					this.cells[x][i].N == n)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Проверяет будет ли конфликт в блоке, если поставить указанную цифру в указанную клетку.
		/// </summary>
		/// <param name="x">X координата клетки.</param>
		/// <param name="y">Y координата клетки.</param>
		/// <param name="n">Цифра.</param>
		private bool CheckForConflictInBlock(int x, int y, int n)
		{
			int blockX = ((int)(x / 3)) * 3;
			int blockY = ((int)(y / 3)) * 3;

			for (int i = blockX; i < blockX + BlockSize; i++)
			{
				for (int j = blockY; j < blockY + BlockSize; j++)
				{
					if (i != x && j != y &&
						this.cells[i][j].N == n)
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Ставит цифру в указанную клетку.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public bool SetCell(int x, int y, int n)
		{
			// Проверяем правильность аргументов.

			if (x < 0 || x >= BoardSize || y < 0 || y > BoardSize)
				return false;

			if (n < 1 || n > 9)
				return false;

			if (this.cells == null || this.cells[x][y].N != 0)
				return false;

			// Проверяем, что не будет конфликта в строке, колонке и блоке.

			if (!CheckForConflictInRow(x, y, n) || !CheckForConflictInColumn(x, y, n) || !CheckForConflictInBlock(x, y, n))
				return false;

			// Ставим цифру в клетку.

			this.cells[x][y].N = n;

			return true;
		}
	}

	public class ResultEntry
	{
		public DateTime DateTime;

		public string PlayerName;
	}

	public class Cell
	{
		public int N;

		public bool IsBase;
	}
}
