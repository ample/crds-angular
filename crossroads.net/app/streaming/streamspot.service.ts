import { Injectable }    from '@angular/core';
import { Headers, Http } from '@angular/http';

import 'rxjs/add/operator/toPromise';

import { Event } from './event';

@Injectable()
export class StreamspotService {

  private url    = 'https://api.streamspot.com/broadcaster';  // URL to web api
  private apiKey = '82437b4d-4e38-42e2-83b6-148fcfaf36fb';
  private id     = 'crossr4915'

  constructor(private http: Http) { }

  get(): Promise<Event[]> {
    let headers = new Headers({
      'Content-Type': 'application/json',
      'x-API-Key': this.apiKey
    })
    let url = `${this.url}/${this.id}/events`;
    return this.http.get(url, {headers: headers})
      .toPromise()
      .then(response => response.json().data.events)
      .catch(this.handleError);
  }

  private handleError(error: any) {
    console.error('An error occurred', error);
    return Promise.reject(error.message || error);
  }
}
