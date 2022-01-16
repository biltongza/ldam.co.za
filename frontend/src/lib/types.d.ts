export interface BlogMetadata {
    title: string;
    date: string;
    excerpt: string;
    slug: string;
}

export interface BlogResponse {
    metadata: BlogMetadata;
    content: string;
}

export interface ImageMetadata {
    id: string;
    hrefs: { [key: string]: string };
    captureDate: Date;
    cameraMake: string;
    cameraModel: string;
    fNumber: string;
    shutterSpeed: string;
    focalLength: string;
    iso: string;
    lens: string;
    title: string;
    caption: string;
    lastModified: Date;
    width: number;
    height: number;
    aspectRatio: string;
}

export interface Album {
    id: string;
    title: string;
    images: { [key: string]: ImageMetadata };
}

export interface Manifest {
	lastModified: Date;
	albums: { [key: string]: Album };
}