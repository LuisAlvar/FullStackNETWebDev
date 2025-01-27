import { Component, OnInit } from '@angular/core';
import { AuthService } from './auth/auth.service';
import { ConnectionService } from 'angular-connection-service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {

  title = 'WorldCities';

  hasNetworkConnection: boolean = true;
  hasInternetAccess: boolean = true;

  constructor(private connectionService: ConnectionService, private authService: AuthService) {
    this.connectionService.monitor().subscribe((currentState: any) => {
      this.hasNetworkConnection = currentState.hasNetworkConnection;
      this.hasInternetAccess = currentState.HasInternetAccess;
    });
  }

  ngOnInit(): void {
    this.authService.isAuthenticated();
  }

  public isOnline() {
    return this.hasNetworkConnection && this.hasInternetAccess;
  }
}
