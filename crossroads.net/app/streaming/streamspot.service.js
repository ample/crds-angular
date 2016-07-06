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
        this.url = 'https://api.streamspot.com/broadcaster';
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
            .then(function (response) { return response.json().data.events; })
            .catch(this.handleError);
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
