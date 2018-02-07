import { Component, OnInit } from '@angular/core';
import { CognitiveService } from '../../common/services/cognitive.service';
import { AzureToolkitService } from '../../common/services/azureToolkit.service';
import { ImageResult } from '../../common/models/bingSearchResponse';
import { ComputerVisionRequest, ComputerVisionResponse } from '../../common/models/computerVisionResponse';
import { ImagePostRequest } from '../../common/models/imagePostRequest';
 import { UserService } from '../../common/services/user.service';
 import { User } from '../../common/models/user';

 @Component({
     selector: 'search',
     templateUrl: './search.component.html',
     styleUrls: ['./search.component.css']
 })
 export class SearchComponent {
    searchResults: ImageResult[] | null;
    isSearching = false;
    currentAnalytics: ComputerVisionResponse | null;
    currentItem: ImageResult | null;
    isAnalyzing = false;
    currentItemSaved: boolean;
    imagePostRequest: ImagePostRequest | null;
    user: User;

    constructor(private cognitiveService: CognitiveService, private azureToolkitService: AzureToolkitService, private userService: UserService) { }
    
     ngOnInit(): void {
         this.userService.getUser().subscribe(user => this.user = user );
     } 
    search(searchTerm: string) {
        this.searchResults = null;
        this.currentAnalytics = null;
        this.isSearching = true;
        this.cognitiveService.searchImages(searchTerm).subscribe(result => {
            this.searchResults = result.value;
            this.isSearching = false;
        });
    }

    analyze(result: ImageResult) {
        this.currentItem = result;
        this.currentItemSaved = false;
        this.currentAnalytics = null;
        this.isAnalyzing = true;
        this.cognitiveService.analyzeImage({ url: result.thumbnailUrl } as ComputerVisionRequest).subscribe(result => {
            this.currentAnalytics = result;
            this.isAnalyzing = false;
        });
        window.scroll(0, 0);
    }

    saveImage() {
        if( this.currentItem != null && this.currentAnalytics != null){
            let transferObject: ImagePostRequest = {
                userId: this.user.userId,
                url: this.currentItem.thumbnailUrl,
                encodingFormat: this.currentItem.encodingFormat,
                id: this.currentItem.imageId,
                description: this.currentAnalytics.description.captions[0].text,
                tags: this.currentAnalytics.tags.map(tag => tag.name),
                age: this.currentAnalytics.faces.map(face => face.age), 
                gender: this.currentAnalytics.faces.map(face => face.gender),
                noofpeople: this.currentAnalytics.faces.length,                               
            }
            this.azureToolkitService.saveImage(transferObject).subscribe(saveSuccessful => {
                this.currentItemSaved = saveSuccessful;
            });
        } else {
            console.info("No image choosen to save.");
        }
    }
 }