export interface ApiKey {
  apiKeyId: string;
  name: string;
  keyName: string;
  keyPrefix?: string;
  accountId: string;
  keyHash?: string;
  createdBy: string;
  createdAt: Date;
  expiresAt?: Date;
  lastUsedAt?: Date;
  revokedAt?: Date;
  revokedBy?: string;
  revocationReason?: string;
  isActive: boolean;
  status: ApiKeyStatus;
  permissions: string[];
}

export enum ApiKeyStatus {
  Active = 'Active',
  Expired = 'Expired',
  Revoked = 'Revoked'
}

export interface ApiRequest {
  requestId: string;
  endpoint: string;
  method: HttpMethod;
  apiKeyId: string;
  accountId: string;
  ipAddress: string;
  timestamp: Date;
  responseStatusCode: number;
  responseTimeMs: number;
  wasRateLimited: boolean;
}

export interface Webhook {
  webhookId: string;
  name: string;
  url: string;
  accountId: string;
  registeredBy: string;
  registeredAt: Date;
  deletedAt?: Date;
  deletedBy?: string;
  isActive: boolean;
  status: WebhookStatus;
  events: string[];
  deliveries: WebhookDelivery[];
}

export enum WebhookStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  Failed = 'Failed'
}

export interface WebhookDelivery {
  webhookDeliveryId: string;
  webhookId: string;
  eventType: string;
  payloadJson: string;
  responseStatus: number;
  success: boolean;
  failureReason?: string;
  retryCount: number;
  timestamp: Date;
}

export enum HttpMethod {
  GET = 'GET',
  POST = 'POST',
  PUT = 'PUT',
  PATCH = 'PATCH',
  DELETE = 'DELETE'
}

export interface CreateApiKeyRequest {
  keyName: string;
  accountId: string;
  permissions: string[];
  expiresAt?: Date;
}

export interface RegisterWebhookRequest {
  url: string;
  accountId: string;
  events: string[];
}
