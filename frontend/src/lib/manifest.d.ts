export interface ImageMetadata {
    Id: string;
    Hrefs: { [key: string]: string };
    CaptureDate: Date;
    CameraMake: string;
    CameraModel: string;
    FNumber: string;
    ShutterSpeed: string;
    FocalLength: string;
    ISO: string;
    Lens: string;
    Title: string;
    Caption: string;
    LastModified: Date;
    Width: number;
    Height: number;
    AspectRatio: string;
}

export interface Album {
    Id: string;
    Title: string;
    Images: { [key: string]: ImageMetadata };
}

export interface Manifest {
	LastModified: Date;
	Albums: { [key: string]: Album };
}
