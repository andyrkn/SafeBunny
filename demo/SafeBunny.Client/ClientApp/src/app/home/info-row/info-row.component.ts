import { Component, Input, OnInit } from '@angular/core';
import { EventData } from '../event-data.interface';

@Component({
  selector: 'app-info-row',
  templateUrl: './info-row.component.html',
  styleUrls: ['./info-row.component.scss']
})
export class InfoRowComponent implements OnInit {

  @Input()
  public data: EventData = { content: "", identity: "", type: "" }

  constructor() { }

  ngOnInit(): void {
  }

}
