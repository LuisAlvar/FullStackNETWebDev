import { Component, OnInit, ViewChild } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from './../../envrionments/environment';

import { MatTable, MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';

import { City } from './city';

@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrl: './cities.component.scss'
})
export class CitiesComponent implements OnInit {

  public displayedColumns: string[] = ['id', 'name', 'lat', 'lon'];

  public cities!: MatTableDataSource<City>;

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    var pageEvent = new PageEvent(); // generated a page event to call getData for the initial component load
    pageEvent.pageIndex = 0;
    pageEvent.pageSize = 10;
    this.getData(pageEvent);
  }

  // Used by mat-paginator element and the ngOnInit()
  getData(event: PageEvent) {
    var url = environment.baseURL + 'api/Cities';
    var params = new HttpParams()
      .set("pageIndex", event.pageIndex.toString())
      .set("pageSize", event.pageSize.toString());
    this.http.get<any>(url, { params })
      .subscribe(result => {
        this.paginator.length = result.totalCount;
        this.paginator.pageIndex = result.pageIndex;
        this.paginator.pageSize = result.pageSize;
        this.cities = new MatTableDataSource<City>(result.data);
      }, error => console.error(error));
  }

}
