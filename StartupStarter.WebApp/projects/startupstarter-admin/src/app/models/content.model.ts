export interface Content {
  contentId: string;
  contentType: string;
  title: string;
  body: string;
  authorId: string;
  accountId: string;
  profileId: string;
  status: ContentStatus;
  createdAt: Date;
  updatedAt?: Date;
  publishedAt?: Date;
  unpublishedAt?: Date;
  scheduledPublishDate?: Date;
  currentVersion: number;
  versions: ContentVersion[];
}

export interface ContentVersion {
  contentVersionId: string;
  contentId: string;
  versionNumber: number;
  title: string;
  body: string;
  createdBy: string;
  changeDescription: string;
  createdAt: Date;
}

export enum ContentStatus {
  Draft = 'Draft',
  Review = 'Review',
  Approved = 'Approved',
  Published = 'Published',
  Unpublished = 'Unpublished',
  Archived = 'Archived',
  Deleted = 'Deleted'
}

export interface CreateContentRequest {
  contentType: string;
  title: string;
  body: string;
  accountId: string;
  profileId: string;
}

export interface UpdateContentRequest {
  title?: string;
  body?: string;
  changeDescription?: string;
}

export interface PublishContentRequest {
  publishDate?: Date;
}

export interface ScheduleContentRequest {
  scheduledPublishDate: Date;
}
