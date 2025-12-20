export interface ApiKey {
  apiKeyId: string;
  keyName: string;
  keyPrefix: string;
  accountId: string;
  permissions: string[];
  createdAt: Date;
  expiresAt?: Date;
  lastUsedAt?: Date;
  revokedAt?: Date;
  isActive: boolean;
}

export interface CreateApiKeyRequest {
  keyName: string;
  permissions: string[];
  expiresAt?: Date;
}

export interface CreateApiKeyResponse {
  apiKeyId: string;
  keyName: string;
  apiKey: string;
  expiresAt?: Date;
}

export interface Webhook {
  webhookId: string;
  accountId: string;
  url: string;
  events: string[];
  isActive: boolean;
  failureCount: number;
  createdAt: Date;
  updatedAt?: Date;
}

export interface WebhookDelivery {
  webhookDeliveryId: string;
  webhookId: string;
  eventType: string;
  payload: Record<string, unknown>;
  responseStatus?: number;
  success: boolean;
  retryCount: number;
  timestamp: Date;
}

export interface CreateWebhookRequest {
  url: string;
  events: string[];
}

export interface UpdateWebhookRequest {
  url?: string;
  events?: string[];
  isActive?: boolean;
}

export const WEBHOOK_EVENTS = [
  'account.created', 'account.updated', 'account.deleted', 'account.suspended',
  'user.created', 'user.updated', 'user.deleted', 'user.activated', 'user.deactivated',
  'content.created', 'content.updated', 'content.published', 'content.unpublished',
  'workflow.started', 'workflow.approved', 'workflow.rejected', 'workflow.completed',
  'media.uploaded', 'media.deleted'
];
