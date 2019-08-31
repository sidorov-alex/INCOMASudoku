using INCOMASudoku.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace INCOMASudoku
{
	/// <summary>
	/// SignalR-хаб для управления игрой.
	/// </summary>
	public class GameHub : Hub
	{
		private readonly IGameService gameService;

		/// <summary>
		/// Инициализирует новый экземпляр класса.
		/// </summary>
		/// <param name="gameService"></param>
		public GameHub(IGameService gameService)
		{
			this.gameService = gameService;
		}

		/// <summary>
		/// Начинает новую игру.
		/// </summary>
		public async Task NewGame()
		{
			this.gameService.GenerateNewSudoku();

			await this.Clients.All.SendAsync("NewGame", this.gameService.GetCells());
		}

		/// <summary>
		/// Присоединяет игрока к текущей игре.
		/// </summary>
		/// <param name="playerName">Имя игрока.</param>
		public async Task JoinGame(string playerName)
		{
			// Сохраняем имя игрока.

			this.Context.Items["playerName"] = playerName;

			// Отправляем вызывающему текущее игровое поле.

			await this.Clients.Caller.SendAsync("NewGame", this.gameService.GetCells());
		}

		/// <summary>
		/// Устанавливает цифру в указанную клетку, если это возможно.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public async Task SetCell(int x, int y, int n)
		{
			if (this.gameService.SetCell(x, y, n))
			{
				// Отправляем всем уведомление о том, что в клетку установлена цифра.

				await this.Clients.All.SendAsync("SetCell", x, y, n);

				// Проверяем, решен ли судоку.

				if (this.gameService.IsSudokuSolved())
				{
					string playerName = (string)this.Context.Items["playerName"];

					// Добавляем игрока в таблицу результатов.

					this.gameService.AddResult(DateTime.Now, playerName);

					// Отправляем игроку уведомление, что он победил.

					await this.Clients.Caller.SendAsync("Win");

					// Отправляем остальным уведомление о том, что игра окончена.

					await this.Clients.Others.SendAsync("GameOver", playerName);
				}
			}
		}

		/// <summary>
		/// Получение таблицы результатов.
		/// </summary>
		/// <returns></returns>
		public async Task GetResults()
		{
			await this.Clients.Caller.SendAsync("GetResults", this.gameService.GetResults());
		}
	}
}
