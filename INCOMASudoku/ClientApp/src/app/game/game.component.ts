import { Component, ViewChild, ElementRef } from '@angular/core';
import { GameBoardComponent, Point } from '../game-board/game-board.component';
import { HubConnection, HubConnectionBuilder } from "@aspnet/signalr";

@Component({
  selector: 'app-home',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent {

  @ViewChild(GameBoardComponent)
  private board: GameBoardComponent;

  isGameStarted: boolean;

  @ViewChild('playerName') playerName: ElementRef;

  private hubConnection: HubConnection

  ngOnInit() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('https://localhost:5001/game')
      .build();
  }

  startNewGame() {

    // Начинаем новую игру.
    this.hubConnection.invoke("NewGame");
  }

  joinGame(playerName: string) {

    if (!playerName) {
      return;
    }

    this.hubConnection
      .start()
      .then(() => {
        console.log('Соединение установлено.');

        this.isGameStarted = true;

        this.hubConnection.on("NewGame", (cells) => {

          // Задаем начальные клетки на поле.
          this.board.setCells(cells);
        });

        this.hubConnection.on("SetCell", (x, y, n) => {

          // Устаналиваем цифру в указанную клетку.
          this.board.setCell(x, y, n);
        });

        this.hubConnection.on("Win", () => {

          // Победа.
          alert("Вы победили.");
        });

        this.hubConnection.on("GameOver", (winner) => {

          // Поражение. Выводим имя победителя.
          alert("Игра закончена. Победил " + winner + ".");
        });

        this.hubConnection.on("GetResults", (list: string[]) => {

          // Отображаем таблицу результатов, если они есть.

          if (list.length == 0) {
            alert("Нет результатов.");
          }
          else {
            alert(list.map((name, i) => i+1 + ". " + name).join("\r\n"));
          }
        });

        // Присоединяемся к текущей игре. Передаем имя игрока, которое сервер сохранит.

        this.hubConnection.invoke("JoinGame", playerName);
      })
      .catch(err => console.log('Ошибка установки соединения: ' + err))
  }

  onSetDigit(n: number) {

    // Задаем цифру в выбранной ранее пользователем клетке через сервер.
    // Сервер проверит, что эта цифра не конфликтует с другими в строке, колонке и блоке.
    
    let cell = this.board.getSelected();

    if (cell != null) {
      this.hubConnection.invoke("SetCell", cell.X, cell.Y, n);
    }
  }

  showTopPlayers() {
    this.hubConnection.invoke("GetResults");
  }
}
