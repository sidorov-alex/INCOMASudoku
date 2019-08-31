import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-game-board',
  templateUrl: './game-board.component.html',
  styleUrls: ['./game-board.component.css']
})
export class GameBoardComponent implements OnInit {

  cells: Cell[][];

  baseCells: Point[] = [];

  constructor() {
  }
    
  ngOnInit() {
  }

  selectedX: number = undefined;
  selectedY: number = undefined

  public setCells(cells: Cell[][]) {
    this.cells = cells;

    //this.baseCells = [];

    //for (var x: number = 0; x < cells.length; x++) {
    //  for (var y: number = 0; y < cells[x].length; y++) {
    //    if (cells[x][y] != 0) {
    //      this.baseCells.push(new Point(x, y));
    //    }
    //  }
    //}
  }

  onCellClick(x: number, y: number) {

    if (this.cells[x][y].n == 0) {
      this.selectedX = x;
      this.selectedY = y;
   }
  }

  public setCell(x: number, y: number, n: number) {

    //if (this.selectedX != undefined && this.selectedY != undefined) {
      this.cells[x][y].n = n;

    //this.baseCells.push(new Point(x, y));

    if (this.selectedX == x && this.selectedY == y) {
      this.selectedX = undefined;
      this.selectedY = undefined;
    }
    //}
  }

  //isBaseCell(x: number, y: number) : boolean {
  //  for (var p of this.baseCells) {
  //    if (p.X == x && p.Y == y)
  //      return true;
  //  }

  //  return false;
  //}

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
