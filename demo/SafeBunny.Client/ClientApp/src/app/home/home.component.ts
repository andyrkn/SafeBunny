import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import * as signalR from "@aspnet/signalr";
import { EventData } from './event-data.interface';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  private connection: signalR.HubConnection;
  public data: any = {}
  constructor(private readonly httpClient: HttpClient) {
    this.connection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Information)
      .withUrl("https://localhost:5001/hub")
      .build();
  }

  public async ngOnInit(): Promise<void> {
    await this.connection.start();

    this.connection.on("event", (data: EventData) => {
      if (!this.data[data.identity]) {
        this.data[data.identity] = []
      }
      this.data[data.identity].push({ type: data.type, content: data.content });
    });
  }

  public trigger(): void {
    this.httpClient.post("https://localhost:5001/api/safebunny", {
      name: "andrei"
    }).subscribe();
  }

  public clean(): void {
    this.data = {};
  }

  public get keys() {
    return Object.keys(this.data);
  }

}
