"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var core_1 = require('@angular/core');
var streamspot_service_1 = require('./streamspot.service');
var StreamingComponent = (function () {
    function StreamingComponent(streamspotService) {
        this.streamspotService = streamspotService;
        this.events = [];
    }
    StreamingComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.streamspotService.get()
            .then(function (events) { return _this.events = events; });
    };
    StreamingComponent = __decorate([
        core_1.Component({
            selector: 'streaming',
            template: "\n    <h1>Upcoming Events</h1>\n    <div *ngFor=\"let event of events\">\n      <span>{{event.title}} @ {{event.start}}</span>\n    </div>\n  ",
            providers: [streamspot_service_1.StreamspotService]
        })
    ], StreamingComponent);
    return StreamingComponent;
}());
exports.StreamingComponent = StreamingComponent;
