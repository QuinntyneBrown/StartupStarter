# Profile Management Domain Models

## Domain Entities

### Profile (Aggregate Root)
- ProfileId, ProfileName, AccountId
- ProfileType (Personal, Project, Department, Team)
- IsDefault, AvatarUrl, Preferences (JSON)
- CreatedBy, OwnerUserId
- Domain Events: Created, Updated, Deleted, AvatarUpdated, PreferencesUpdated, Shared, ShareRevoked, SetAsDefault

### ProfileShare (Entity)
- ShareId, ProfileId, SharedWithUserId
- PermissionLevel (View, Edit, Admin)

### ProfilePreferences (Value Object)
- Theme, Language, Timezone, Notifications

## MediatR Commands
- CreateProfileCommand, UpdateProfileCommand, DeleteProfileCommand
- UpdateAvatarCommand, UpdatePreferencesCommand
- ShareProfileCommand, RevokeProfileShareCommand
- SetAsDefaultProfileCommand
- AddDashboardToProfileCommand, RemoveDashboardFromProfileCommand

## MediatR Queries
- GetProfileByIdQuery, GetProfilesByAccountQuery
- GetSharedProfilesQuery, GetDefaultProfileQuery
