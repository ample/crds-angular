import { Component, OnInit } from '@angular/core';
import { HTTP_PROVIDERS } from '@angular/http';

import { Event } from './event';
import { StreamspotService } from './streamspot.service';

@Component({
  selector: 'streaming',
  template: `
    <h1>Upcoming Events</h1>
    <div *ngFor="let event of events">
      <span>{{event.title}} @ {{event.start}}</span>
    </div>
  `,
  providers: [StreamspotService, HTTP_PROVIDERS]
})

export class StreamingComponent implements OnInit {
  events: Event[] = [];

  constructor(private streamspotService: StreamspotService) { }

  ngOnInit() {
    this.streamspotService.get()
      .then(events => {
        console.log(events);
        this.events = events
      })
  }
}
