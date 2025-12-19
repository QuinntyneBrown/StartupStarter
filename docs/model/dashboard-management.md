# Dashboard Management Domain Models

## Domain Entities

### Dashboard (Aggregate Root)
- DashboardId, DashboardName, ProfileId, AccountId
- IsDefault, Template, Layout (Grid, Masonry, Freeform)
- CreatedBy, Sharing permissions
- Domain Events: Created, Updated, Deleted, Cloned, SetAsDefault, Shared, ShareRevoked

### DashboardCard (Entity)
- CardId, DashboardId, CardType
- Position (Row, Column, Width, Height)
- Configuration (JSON)
- Domain Events: CardAdded, CardUpdated, CardRemoved, CardRepositioned

### DashboardShare (Entity)
- ShareId, DashboardId, SharedWithUserId, PermissionLevel

## MediatR Commands
- CreateDashboardCommand, UpdateDashboardCommand, DeleteDashboardCommand
- CloneDashboardCommand, SetAsDefaultDashboardCommand
- ShareDashboardCommand, RevokeDashboardShareCommand
- AddCardCommand, UpdateCardCommand, RemoveCardCommand, RepositionCardCommand
- ExportDashboardCommand, ImportDashboardCommand

## MediatR Queries
- GetDashboardByIdQuery, GetDashboardsByProfileQuery
- GetSharedDashboardsQuery
