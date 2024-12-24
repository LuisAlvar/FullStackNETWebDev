import { Component, OnInit, ViewChild } from '@angular/core';
//import { HttpClient, HttpParams } from '@angular/common/http';
import { MatTable, MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

import { environment } from './../../envrionments/environment';
import { City } from './city';
import { CityService } from './city.service';
import { ApiResult } from '../base.service';

@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrl: './cities.component.scss'
})
export class CitiesComponent implements OnInit {

  public displayedColumns: string[] = ['id', 'name', 'lat', 'lon', 'countryName'];

  public cities!: MatTableDataSource<City>;
  public loadingDummyCityData: City[] = [];

  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;
  defaultFilterColumn: string = "name";
  filterQuery?: string;
  public defaultSortColumn: string = "id";
  public defaultSortOrder: "asc" | "desc" = "asc";

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  filterTextChanged: Subject<string> = new Subject<string>();

  constructor(private cityService: CityService) { }

  ngOnInit() {
    this.loadingDummyData();
    this.loadData();
  }

  loadingDummyData() {
    for (var i = 0; i < 4; i++) {
      this.loadingDummyCityData!.push({ id: 1, name: "", lat: 0, lon: 0, countryId: 0 , countryName: ""});
    }
  }

  loadData(query?: string) {
    var pageEvent = new PageEvent(); // generated a page event to call getData for the initial component load
    pageEvent.pageIndex = this.defaultPageIndex;
    pageEvent.pageSize = this.defaultPageSize;
    this.filterQuery = query;
    this.getData(pageEvent);
  }

  // Used by mat-paginator element and the ngOnInit()
  getData(event: PageEvent) {
    var sortColumn = (this.sort) ? this.sort.active : this.defaultSortColumn;
    var sortOrder = (this.sort) ? this.sort.direction : this.defaultSortOrder;

    var filterColumn = (this.filterQuery) ? this.defaultFilterColumn : null;
    var filterQuery = (this.filterQuery) ? this.filterQuery : null;

    this.cityService.getData(
      event.pageIndex,
      event.pageSize,
      sortColumn,
      sortOrder,
      filterColumn,
      filterQuery
    ).subscribe(result => {
      this.paginator.length = result.totalCount;
      this.paginator.pageIndex = result.pageIndex;
      this.paginator.pageSize = result.pageSize;
      this.cities = new MatTableDataSource<City>(result.data);
    }, error => console.error(error));

  }

  // debounce filter text changes
  onFilterTextChanged(filterText: string) {
    if (this.filterTextChanged.observers.length == 0) {
      this.filterTextChanged
        .pipe(debounceTime(1000), distinctUntilChanged())
        .subscribe(query => {
          this.loadData(query);
        });
    }
    this.filterTextChanged.next(filterText);
  }
}
