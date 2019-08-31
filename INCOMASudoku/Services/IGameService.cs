using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INCOMASudoku.Services
{
	public interface IGameService
	{
		Cell[][] GetCells();

		void GenerateNewSudoku();

		void AddResult(DateTime dateTime, string playerName);

		string[] GetResults();

		bool SetCell(int x, int y, int n);

		bool IsSudokuSolved();
	}
}
