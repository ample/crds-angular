"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require('@angular/core');
var http_1 = require('@angular/http');
var streamspot_service_1 = require('./streamspot.service');
var StreamingComponent = (function () {
    function StreamingComponent(streamspotService) {
        this.streamspotService = streamspotService;
        this.events = [];
    }
    StreamingComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.streamspotService.get()
            .then(function (events) {
            console.log(events);
            _this.events = events;
        });
    };
    StreamingComponent = __decorate([
        core_1.Component({
            selector: 'streaming',
            template: "\n    <h1>Upcoming Events</h1>\n    <div *ngFor=\"let event of events\">\n      <span>{{event.title}} @ {{event.start}}</span>\n    </div>\n  ",
            providers: [streamspot_service_1.StreamspotService, http_1.HTTP_PROVIDERS]
        }), 
        __metadata('design:paramtypes', [streamspot_service_1.StreamspotService])
    ], StreamingComponent);
    return StreamingComponent;
}());
exports.StreamingComponent = StreamingComponent;
