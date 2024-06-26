export interface BlogMetadata {
  title: string;
  date: number;
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
  cameraModel?: string;
  fNumber?: string;
  shutterSpeed?: string;
  focalLength?: string;
  iso?: string;
  lens?: string;
  title?: string;
  caption?: string;
  lastModified: string;
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
