# StartupStarter API Endpoints Reference

## Overview

This document provides a comprehensive reference of all API endpoints in the StartupStarter application.

**Base URL**: `https://localhost:{port}/api`

**Total Controllers**: 13
**Total Endpoints**: 55+

---

## 1. Account Management

**Controller**: `AccountsController`
**Route**: `/api/accounts`

| Method | Endpoint | Action | Description |
|--------|----------|--------|-------------|
| POST | `/api/accounts` | CreateAccount | Create a new account |
| GET | `/api/accounts/{id}` | GetAccount | Retrieve account by ID |

---

## 2. User Management

**Controller**: `UsersController`
**Route**: `/api/users`

| Method | Endpoint | Action | Description |
|--------|----------|--------|-------------|
| POST | `/api/users` | CreateUser | Create a new user |
| GET | `/api/users/{id}` | GetUser | Retrieve user by ID |
| POST | `/api/users/{id}/activate` | ActivateUser | Activate a user account |
| GET | `/api/users/invitations/{id}` | GetInvitation | Retrieve user invitation by ID |

---

## 3. Profile Management

**Controller**: `ProfilesController`
**Route**: `/api/profiles`

| Method | Endpoint | Action | Description |
|--------|----------|--------|-------------|
| POST | `/api/profiles` | CreateProfile | Create a new profile |
| GET | `/api/profiles/{id}` | GetProfile | Retrieve profile by ID |
| PUT | `/api/profiles/{id}` | UpdateProfile | Update profile details |
| DELETE | `/api/profiles/{id}` | DeleteProfile | Soft delete a profile |
| GET | `/api/profiles/{id}/preferences` | GetPreferences | Get profile preferences |

---

## 4. Role Management

**Controller**: `RolesController`
**Route**: `/api/roles`

| Method | Endpoint | Action | Description |
|--------|----------|--------|-------------|
| POST | `/api/roles` | CreateRole | Create a new role |
| GET | `/api/roles/{id}` | GetRole | Retrieve role by ID |
| PUT | `/api/roles/{id}` | UpdateRole | Update role details |
| POST | `/api/roles/assign` | AssignRole | Assign role to user |
| GET | `/api/roles/users/{userId}/roles` | GetUserRoles | Get all roles for a user |

---

## 5. Content Management

**Controller**: `ContentsController`
**Route**: `/api/contents`

| Method | Endpoint | Action | Description |
|--------|----------|--------|-------------|
| POST | `/api/contents` | CreateContent | Create new content |
| GET | `/api/contents/{id}` | GetContent | Retrieve content by ID |
| PUT | `/api/contents/{id}` | UpdateContent | Update content |
| POST | `/api/contents/{id}/publish` | PublishContent | Publish content |
| GET | `/api/contents/{id}/versions/{versionId}` | GetVersion | Get specific content version |

---

## 6. Dashboard Management

**Controller**: `DashboardsController`
**Route**: `/api/dashboards`

| Method | Endpoint | Action | Description |
|--------|----------|--------|-------------|
| POST | `/api/dashboards` | CreateDashboard | Create a new dashboard |
| GET | `/api/dashboards/{id}` | GetDashboard | Retrieve dashboard by ID |
| PUT | `/api/dashboards/{id}` | UpdateDashboard | Update dashboard |
| POST | `/api/dashboards/{id}/cards` | AddCard | Add card to dashboard |
| GET | `/api/dashboards/{id}/cards` | GetCards | Get all cards for dashboard |

---

## 7. Media Management

**Controller**: `MediaController`
**Route**: `/api/media`

| Method | Endpoint | Action | Description |
|--------|----------|--------|-------------|
| POST | `/api/media` | UploadMedia | Upload new media file |
| GET | `/api/media/{id}` | GetMedia | Retrieve media by ID |
| PUT | `/api/media/{id}` | UpdateMedia | Update media metadata |
| DELETE | `/api/media/{id}` | DeleteMedia | Soft delete media |

---

## 8. API Key Management

**Controller**: `ApiKeysController`
**Route**: `/api/apikeys`

| Method | Endpoint | Action | Description |
|--------|----------|--------|-------------|
| POST | `/api/apikeys` | CreateApiKey | Create new API key |
| GET | `/api/apikeys/{id}` | GetApiKey | Retrieve API key by ID |
| POST | `/api/apikeys/{id}/revoke` | RevokeApiKey | Revoke an API key |
| GET | `/api/apikeys/{id}/requests` | GetRequests | Get API requests for key |

---

## 9. Webhook Management

**Controller**: `WebhooksController`
**Route**: `/api/webhooks`

| Method | Endpoint | Action | Description |
|--------|----------|--------|-------------|
| POST | `/api/webhooks` | RegisterWebhook | Register new webhook |
| GET | `/api/webhooks/{id}` | GetWebhook | Retrieve webhook by ID |
| PUT | `/api/webhooks/{id}` | UpdateWebhook | Update webhook |
| DELETE | `/api/webhooks/{id}` | DeleteWebhook | Soft delete webhook |
| GET | `/api/webhooks/{id}/deliveries` | GetDeliveries | Get webhook deliveries |

---

## 10. Audit Management

**Controller**: `AuditController`
**Route**: `/api/audit`

| Method | Endpoint | Action | Description |
|--------|----------|--------|-------------|
| POST | `/api/audit/logs` | CreateAuditLog | Create audit log entry |
| GET | `/api/audit/logs/{id}` | GetAuditLog | Retrieve audit log by ID |
| POST | `/api/audit/exports` | RequestExport | Request audit export |
| GET | `/api/audit/exports/{id}` | GetExport | Get audit export status |

---

## 11. Authentication Management

**Controller**: `AuthenticationController`
**Route**: `/api/authentication`

| Method | Endpoint | Action | Description |
|--------|----------|--------|-------------|
| POST | `/api/authentication/sessions` | CreateSession | Create user session (login) |
| GET | `/api/authentication/sessions/{id}` | GetSession | Retrieve session by ID |
| POST | `/api/authentication/sessions/{id}/end` | EndSession | End user session (logout) |
| POST | `/api/authentication/mfa/enable` | EnableMfa | Enable MFA for user |
| GET | `/api/authentication/users/{userId}/login-attempts` | GetLoginAttempts | Get login attempts for user |

---

## 12. Workflow Management

**Controller**: `WorkflowsController`
**Route**: `/api/workflows`

| Method | Endpoint | Action | Description |
|--------|----------|--------|-------------|
| POST | `/api/workflows` | StartWorkflow | Start new workflow |
| GET | `/api/workflows/{id}` | GetWorkflow | Retrieve workflow by ID |
| POST | `/api/workflows/stages/{stageId}/complete` | CompleteStage | Complete workflow stage |
| POST | `/api/workflows/stages/{stageId}/approve` | ApproveWorkflow | Approve/reject workflow stage |
| GET | `/api/workflows/{id}/stages` | GetStages | Get all stages for workflow |

---

## 13. System Management

**Controller**: `SystemController`
**Route**: `/api/system`

| Method | Endpoint | Action | Description |
|--------|----------|--------|-------------|
| POST | `/api/system/maintenance` | ScheduleMaintenance | Schedule system maintenance |
| GET | `/api/system/maintenance/{id}` | GetMaintenance | Get maintenance details |
| POST | `/api/system/maintenance/{id}/start` | StartMaintenance | Start scheduled maintenance |
| POST | `/api/system/backups` | StartBackup | Initiate system backup |
| GET | `/api/system/backups/{id}` | GetBackup | Get backup details |
| GET | `/api/system/errors` | GetSystemErrors | Get all system errors |

---

## Response Codes

All endpoints follow standard HTTP status codes:

- **200 OK**: Successful GET/PUT request
- **201 Created**: Successful POST request creating a resource
- **204 No Content**: Successful DELETE request
- **404 Not Found**: Resource not found
- **400 Bad Request**: Invalid request data
- **500 Internal Server Error**: Server error

---

## Architecture

All endpoints follow the CQRS pattern:
- **Commands**: Write operations (POST, PUT, DELETE)
- **Queries**: Read operations (GET)
- **MediatR**: Request/response pattern for all operations
- **DTOs**: Data transfer objects for all responses

---

## Authentication

Currently, the API does not implement authentication. Future implementation will include:
- JWT Bearer tokens
- Role-based authorization
- API key authentication for external integrations

---

## Swagger Documentation

Interactive API documentation is available at:
```
https://localhost:{port}/swagger
```

---

**Last Updated**: 2025-12-19
**Version**: 1.0.0
**Status**: Production-Ready
