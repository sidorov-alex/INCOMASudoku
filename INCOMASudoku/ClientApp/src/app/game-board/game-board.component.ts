import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-game-board',
  templateUrl: './game-board.component.html',
  styleUrls: ['./game-board.component.css']
})
export class GameBoardComponent implements OnInit {

  cells: Cell[][];

  constructor() {
  }
    
  ngOnInit() {
  }

  selectedX: number = undefined;
  selectedY: number = undefined

  public setCells(cells: Cell[][]) {
    this.cells = cells;
  }

  onCellClick(x: number, y: number) {

    // Запоминаем пустую клетку, по которой кликнул пользователь. Она будет выделена цветом.
    if (this.cells[x][y].n == 0) {
      this.selectedX = x;
      this.selectedY = y;
   }
  }

  public setCell(x: number, y: number, n: number) {

    // Ставим цифру в указанную ячейку.
    this.cells[x][y].n = n;

    // Если эта ячейка была выделена, то снимаем выделение.
    if (this.selectedX == x && this.selectedY == y) {
      this.selectedX = undefined;
      this.selectedY = undefined;
    }
  }

  getSelected(): Point | null {
    if (this.selectedX != undefined && this.selectedY != undefined)
      return new Point(this.selectedX, this.selectedY);
    else
      return null;
  }

  isSelected(x: number, y: number) {
    return this.selectedX == x && this.selectedY == y;
  }
}

export class Point {
  public X: number;
  public Y: number;

  constructor(x: number, y: number) {
    this.X = x;
    this.Y = y;
  }
}

class Cell {
  public n: number;
  public isBase: boolean;
}
