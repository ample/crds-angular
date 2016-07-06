"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var core_1 = require('@angular/core');
// import { Headers, Http } from '@angular/http';
// import 'rxjs/add/operator/toPromise';
var mock_events_1 = require('./mock-events');
var StreamspotService = (function () {
    // private url    = 'https://api.streamspot.com/broadcaster';  // URL to web api
    // private apiKey = '82437b4d-4e38-42e2-83b6-148fcfaf36fb';
    // private id     = 'crossr4915'
    // constructor(private http: Http) { }
    function StreamspotService() {
    }
    // get(): Promise<Event[]> {
    //   let headers = new Headers({
    //     'Content-Type': 'application/json',
    //     'x-API-Key': this.apiKey
    //   })
    //   let url = `${this.url}/${this.id}/events`;
    //   return this.http.get(url, {headers: headers})
    //     .toPromise()
    //     .then(response => response.json().data.events)
    //     .catch(this.handleError);
    // }
    // private handleError(error: any) {
    //   console.error('An error occurred', error);
    //   return Promise.reject(error.message || error);
    // }
    StreamspotService.prototype.get = function () {
        return mock_events_1.EVENTS;
    };
    StreamspotService = __decorate([
        core_1.Injectable()
    ], StreamspotService);
    return StreamspotService;
}());
exports.StreamspotService = StreamspotService;
