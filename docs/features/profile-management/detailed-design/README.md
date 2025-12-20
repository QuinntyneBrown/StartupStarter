# Profile Management - Detailed Design

## Overview
User profiles within accounts for organizing dashboards, content, and preferences.

## Aggregates
- **ProfileAggregate**: User profiles with preferences and sharing

## Key Features
- Multiple profiles per account
- Profile types (Personal, Project, Department, Team)
- Profile-specific preferences
- Avatar management
- Profile sharing within account
- Default profile per user
- Dashboard association

## Dependencies
- **AccountAggregate**: Profiles belong to one account
- **UserAggregate**: Users can access shared profiles
- **DashboardAggregate**: Profiles contain dashboards

## Business Rules
1. Each user has one default profile per account
2. Profile names unique within account
3. Profiles cannot be moved between accounts
4. Sharing only within same account
5. Share permissions: View, Edit, Admin
6. Profile owner can revoke shares
7. Deleting profile deletes associated dashboards

## Data Model
**Profiles Table**
- ProfileId, ProfileName
- AccountId, CreatedBy
- ProfileType, IsDefault
- AvatarUrl
- CreatedAt, UpdatedAt, DeletedAt

**ProfilePreferences Table**
- ProfilePreferencesId, ProfileId
- Category, PreferencesJson
- CreatedAt, UpdatedAt

**ProfileShares Table**
- ProfileShareId, ProfileId
- OwnerUserId, SharedWithUserId
- PermissionLevel, SharedAt

## Sequence: Share Profile
```
User → ShareProfileCommand
→ Validate user is profile owner
→ Validate target users in same account
→ Create ProfileShare records
→ Save to database
→ Publish ProfileSharedEvent
→ Notify shared users
```

## API Endpoints
- POST /api/profiles - Create profile
- GET /api/profiles/{id} - Get profile
- GET /api/profiles - List user's profiles
- PUT /api/profiles/{id} - Update profile
- DELETE /api/profiles/{id} - Delete profile
- PUT /api/profiles/{id}/avatar - Update avatar
- PUT /api/profiles/{id}/preferences - Update preferences
- POST /api/profiles/{id}/share - Share profile
- DELETE /api/profiles/{id}/share/{userId} - Revoke share
- POST /api/profiles/{id}/default - Set as default

## Preference Categories
- Display: Theme, language, timezone
- Notifications: Email, push preferences
- Dashboard: Default layout, refresh rate
- Privacy: Visibility settings

## Profile Types
- **Personal**: Individual user workspace
- **Project**: Project-specific dashboards
- **Department**: Department-level view
- **Team**: Collaborative team space

Each type may have different default settings and permissions.
