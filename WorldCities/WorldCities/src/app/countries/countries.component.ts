//import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';


import { environment } from '../../envrionments/environment';
import { Country } from './country';
import { CountryService } from './country.service';
import { ApiResult } from '../base.service';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-countries',
  templateUrl: './countries.component.html',
  styleUrl: './countries.component.scss'
})
export class CountriesComponent implements OnInit {
  public displayedColumns: string[] = ['id', 'name', 'iso2', 'iso3','totCities'];

  public countries!: MatTableDataSource<Country>;
  public loadingCountryData: Country[] = [];

  public userRegistered: boolean = false;

  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;
  defaultFilterColumn: string = "name";
  filterQuery?: string;
  public defaultSortColumn: string = "id";
  public defaultSortOrder: "asc" | "desc" = "asc";

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  filterTextChanged: Subject<string> = new Subject<string>();

  constructor(
    private countryService: CountryService,
    private authService: AuthService
  ) {
  }

  ngOnInit() {
    this.loadingDummyData();
    this.loadData();
    this.userRegistered = this.authService.isAuthenticated();
  }

  loadingDummyData() {
    for (var i = 0; i < 4; i++) {
      this.loadingCountryData.push({ id: 1, name: "", iso2: "", iso3: "" , totcities: 0});
    }
  }

  loadData(query?: string) {
    var pageEvent = new PageEvent(); // generated a page event to call getData for the initial component load
    pageEvent.pageIndex = this.defaultPageIndex;
    pageEvent.pageSize = this.defaultPageSize;
    this.filterQuery = query;
    console.log(query);
    this.getData(pageEvent);
  }

  // Used by mat-paginator element and the ngOnInit()
  getData(event: PageEvent) {

    var sortColumn = (this.sort) ? this.sort.active : this.defaultSortColumn;

    var sortOrder = (this.sort) ? this.sort.direction : this.defaultSortOrder;

    var filterColumn = (this.filterQuery) ? this.defaultFilterColumn : null;

    var filterQuery = (this.filterQuery) ? this.filterQuery : null;

    this.countryService.getData(
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
        this.countries = new MatTableDataSource<Country>(result.data);
        this.loadingCountryData = [];
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
