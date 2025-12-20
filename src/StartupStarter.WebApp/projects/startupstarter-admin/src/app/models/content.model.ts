export enum ContentStatus {
  Draft = 'Draft',
  Review = 'Review',
  Approved = 'Approved',
  Published = 'Published',
  Archived = 'Archived'
}

export enum ContentType {
  Article = 'Article',
  Page = 'Page',
  BlogPost = 'BlogPost',
  News = 'News',
  Documentation = 'Documentation'
}

export interface Content {
  contentId: string;
  title: string;
  body: string;
  contentType: ContentType;
  accountId: string;
  profileId?: string;
  authorId: string;
  authorName?: string;
  status: ContentStatus;
  currentVersion: number;
  publishedAt?: Date;
  scheduledPublishDate?: Date;
  createdAt: Date;
  updatedAt?: Date;
  deletedAt?: Date;
}

export interface ContentVersion {
  contentVersionId: string;
  contentId: string;
  versionNumber: number;
  title: string;
  body: string;
  createdBy: string;
  createdByName?: string;
  changeDescription?: string;
  createdAt: Date;
}

export interface CreateContentRequest {
  title: string;
  body: string;
  contentType: ContentType;
  profileId?: string;
}

export interface UpdateContentRequest {
  title?: string;
  body?: string;
  changeDescription?: string;
}

export interface PublishContentRequest {
  scheduledDate?: Date;
}

export interface UnpublishContentRequest {
  reason: string;
}
