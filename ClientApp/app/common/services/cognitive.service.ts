import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import { AzureHttpClient } from './azureHttpClient';
import { BingSearchResponse } from '../models/bingSearchResponse';
import { ComputerVisionRequest, ComputerVisionResponse } from '../models/computerVisionResponse';
@Injectable()
export class CognitiveService {
    bingSearchAPIKey = 'e1717b9c993d430e92cd20c5954fbbb3';
    computerVisionAPIKey = 'c40f2a31aab147d0a54afb9b589cce28';
    searchUrl = '';
    imageJson = '';
    constructor(private http: AzureHttpClient) { }

    searchImages(searchTerm: string): Observable<BingSearchResponse> {
        this.searchUrl = 'https://api.cognitive.microsoft.com/bing/v7.0/images/search?q=$' + searchTerm;
        return this.http.get(this.searchUrl, this.bingSearchAPIKey)
            .map(response => response.json() as BingSearchResponse)
            .catch(this.handleError);
    }

    analyzeImage(request: ComputerVisionRequest): Observable<ComputerVisionResponse> {
        this.imageJson = '{"url":"' + request.url + '"}'
        return this.http.post('https://westeurope.api.cognitive.microsoft.com/vision/v1.0/analyze?visualFeatures=Description,Tags', this.computerVisionAPIKey, this.imageJson)
            .map((res)  => res.json() as ComputerVisionResponse)
            .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {
        console.error('An error occurred', error); // for demo purposes only
        return Promise.reject(error.message || error);
    }
}