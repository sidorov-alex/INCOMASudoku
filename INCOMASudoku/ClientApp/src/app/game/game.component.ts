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
    this.hubConnection.invoke("NewGame");
  }

  joinGame() {

    this.hubConnection
      .start()
      .then(() => {
        console.log('Соединение установлено.');

        this.isGameStarted = true;

        this.hubConnection.on("NewGame", (data) => {
          this.board.setCells(data);
        });

        this.hubConnection.on("SetCell", (x, y, n) => {
          this.board.setCell(x, y, n);
        });

        this.hubConnection.on("Win", () => {
          alert("Вы победили.");
        });

        this.hubConnection.on("GameOver", (winner) => {
          alert("Игра закончена. Победил " + winner);
        });

        this.hubConnection.on("GetResults", (list: string[]) => {
          alert(list.join("\r\n"));
        });

        this.hubConnection.invoke("JoinGame", this.playerName.nativeElement.value);
      })
      .catch(err => console.log('Ошибка установки соединения: ' + err))
  }

  onSetDigit(n: number) {
    let cell = this.board.getSelected();

    if (cell != null) {
      this.hubConnection.invoke("SetCell", cell.X, cell.Y, n);
    }
  }

  showTopPlayers() {
    this.hubConnection.invoke("GetResults");
  }
}
