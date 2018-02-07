export interface ImagePostRequest {
    userId: string;
    description: string;
    tags: string[];
    url: string;
    id: string;
    encodingFormat: string;
    noofpeople: number;
    gender: string[];
    age: number[];
}