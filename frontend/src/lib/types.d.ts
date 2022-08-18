import type { Dayjs } from 'dayjs';

export interface BlogMetadata {
	title: string;
	date: Dayjs;
	excerpt: string;
	slug: string;
	tags: string[];
}

export interface BlogResponse {
	metadata: BlogMetadata;
	content: string;
}

export interface ImageMetadata {
	id: string;
	hrefs: { [key: string]: string };
	captureDate: string;
	cameraMake: string;
	cameraModel: string;
	fNumber: string;
	shutterSpeed: string;
	focalLength: string;
	iso: string;
	lens: string;
	title: string;
	caption: string;
	lastModified: string;
	width: number;
	height: number;
	aspectRatio: string;
}

export interface Album {
	id: string;
	title: string;
	isPortfolio: boolean;
	created: Date;
	updated: Date;
	images: { [key: string]: ImageMetadata };
}

export interface Manifest {
	lastModified: Date;
	albums: Album[];
}
