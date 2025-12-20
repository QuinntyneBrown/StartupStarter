export enum MediaType {
  Image = 'Image',
  Video = 'Video',
  Document = 'Document',
  Audio = 'Audio',
  Other = 'Other'
}

export enum ProcessingStatus {
  Pending = 'Pending',
  Processing = 'Processing',
  Completed = 'Completed',
  Failed = 'Failed'
}

export interface Media {
  mediaId: string;
  fileName: string;
  fileType: MediaType;
  mimeType: string;
  fileSize: number;
  accountId: string;
  profileId?: string;
  uploadedBy: string;
  uploadedByName?: string;
  storageLocation: string;
  thumbnailUrl?: string;
  processingStatus: ProcessingStatus;
  tags: string[];
  categories: string[];
  uploadedAt: Date;
  deletedAt?: Date;
}

export interface UploadMediaRequest {
  file: File;
  profileId?: string;
  tags?: string[];
  categories?: string[];
}

export interface UpdateMediaRequest {
  fileName?: string;
  tags?: string[];
  categories?: string[];
}

export interface MediaSearchParams {
  query?: string;
  fileType?: MediaType;
  tags?: string[];
  categories?: string[];
  page?: number;
  pageSize?: number;
}
