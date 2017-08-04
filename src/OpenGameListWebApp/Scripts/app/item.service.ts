import { Injectable } from "@angular/core";
import { Http, Response } from "@angular/http";
import { Observable } from "rxjs/Observable";
import { Item } from "./item";

@Injectable()
export class ItemService {
    constructor(
        private http: Http
    ){}

    private baseUrl = "api/items/";  // web api URL

    // calls the GET: api/items/GetLatest/{n}
    getLatest(num?: number) {
        var url = this.baseUrl + "GetLatest/";
        if (num != null) { 
            url += num;
        }
        return this.http.get(url)
            .map(response => response.json())
            .catch(this.handleError);
    }

    // calls the GET: /api/items/GetMostViewed/{n}
    getMostViewed(num?: number) {
        var url = this.baseUrl + "GetMostViewed/";
        if (num != null) {
            url += num;
        }
        return this.http.get(url)
            .map(response => response.json())
            .catch(this.handleError);
    }

    // calls the GET: /api/items/GetRandom/{n}
    getRandom(num?: number) {
        var url = this.baseUrl + "GetRandom/";
        if (num != null) {
            url += num;
        }
        return this.http.get(url)
            .map(response => response.json())
            .catch(this.handleError);
    }

    // calls the GET: /api/items/{id}
    get(id: number) {
        if (id == null) {
            throw new Error("id is required.");
        }
        var url = this.baseUrl + id;
        return this.http.get(url)
            .map(res => <Item>res.json())
            .catch(this.handleError);
    }

    private handleError(error: Response) {
        console.error(error);
        return Observable.throw(error.json().error || "Server error");
    }

} 