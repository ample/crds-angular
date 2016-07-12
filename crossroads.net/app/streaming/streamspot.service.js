"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var core_1 = require('@angular/core');
var http_1 = require('@angular/http');
require('rxjs/add/operator/toPromise');
var StreamspotService = (function () {
    function StreamspotService(http) {
        this.http = http;
        //
        // #TODO - move to ENV file?
        //
        this.url = 'https://api.streamspot.com/broadcaster'; // URL to web api
        this.apiKey = '82437b4d-4e38-42e2-83b6-148fcfaf36fb';
        this.id = 'crossr4915';
    }
    StreamspotService.prototype.get = function () {
        var headers = new http_1.Headers({
            'Content-Type': 'application/json',
            'x-API-Key': this.apiKey
        });
        var url = this.url + "/" + this.id + "/events";
        return this.http.get(url, { headers: headers })
            .toPromise()
            .then(function (response) { return response.json().data.events
            .filter(function (event) {
            return moment() <= moment(event.start) && event.deleted === null;
        })
            .map(function (event) {
            event.date = moment(event.start);
            event.dayOfYear = event.date.dayOfYear();
            event.time = event.date.format('LT [EST]');
            return event;
        }); })
            .catch(this.handleError);
    };
    StreamspotService.prototype.byDate = function () {
        return this.get().then(function (response) {
            return _.chain(response)
                .sortBy('date')
                .groupBy('dayOfYear')
                .value();
        });
    };
    StreamspotService.prototype.handleError = function (error) {
        console.error('An error occurred', error);
        return Promise.reject(error.message || error);
    };
    StreamspotService = __decorate([
        core_1.Injectable()
    ], StreamspotService);
    return StreamspotService;
}());
exports.StreamspotService = StreamspotService;
