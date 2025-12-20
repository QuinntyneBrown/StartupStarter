export interface Media {
  mediaId: string;
  fileName: string;
  fileType: string;
  mimeType: string;
  fileSize: number;
  uploadedBy: string;
  accountId: string;
  profileId: string;
  storageLocation: string;
  url: string;
  uploadedAt: Date;
  updatedAt?: Date;
  processingStatus: string;
  tags: string[];
  categories: string[];
  outputFormats: string[];
}

export interface UploadMediaRequest {
  file: File;
  accountId: string;
  profileId: string;
  tags?: string[];
  categories?: string[];
}

export interface UpdateMediaRequest {
  fileName?: string;
  tags?: string[];
  categories?: string[];
}
