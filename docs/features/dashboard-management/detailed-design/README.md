# Dashboard Management - Detailed Design

## Overview
Customizable dashboard system with drag-drop cards, layouts, and sharing.

## Aggregates
- **DashboardAggregate**: Dashboard with cards and layout

## Key Features
- Multiple dashboards per profile
- Customizable layouts (Grid, Masonry, Freeform)
- Dashboard cards (widgets)
- Card positioning and sizing
- Dashboard sharing with permissions
- Dashboard templates
- Export/Import dashboards
- Clone dashboards

## Dependencies
- **ProfileAggregate**: Dashboards belong to profiles
- **AccountAggregate**: For account-level permissions
- **UserAggregate**: For sharing with users

## Business Rules
1. Each profile has one default dashboard
2. Dashboard cards positioned using grid system
3. Shared dashboards can be View or Edit permission
4. Dashboard owner can revoke shares anytime
5. Cloning creates independent copy
6. Export includes card configurations
7. Moving dashboard changes profile ownership

## Data Model
**Dashboards Table**
- DashboardId, DashboardName
- ProfileId, AccountId
- CreatedBy, IsDefault
- Template, LayoutType

**DashboardCards Table**
- CardId, DashboardId
- CardType, ConfigurationJson
- Row, Column, Width, Height
- CreatedAt, UpdatedAt

**DashboardShares Table**
- DashboardShareId, DashboardId
- OwnerUserId, SharedWithUserId
- PermissionLevel, SharedAt

## Card Position System
- Grid-based layout (12 columns)
- Card dimensions in grid units
- Position: {Row, Column, Width, Height}
- Collision detection
- Auto-arrange on conflicts

## Sequence: Add Card to Dashboard
```
User → AddDashboardCardCommand
→ Validate user has edit permission
→ Check position available (no collision)
→ Create DashboardCard
→ Calculate layout
→ Save to database
→ Publish DashboardCardAddedEvent
→ Return updated dashboard
```

## API Endpoints
- POST /api/dashboards - Create dashboard
- GET /api/dashboards/{id} - Get dashboard
- PUT /api/dashboards/{id} - Update dashboard
- DELETE /api/dashboards/{id} - Delete dashboard
- POST /api/dashboards/{id}/clone - Clone dashboard
- POST /api/dashboards/{id}/default - Set as default
- POST /api/dashboards/{id}/share - Share dashboard
- DELETE /api/dashboards/{id}/share/{userId} - Revoke share
- POST /api/dashboards/{id}/cards - Add card
- PUT /api/dashboards/{id}/cards/{cardId} - Update card
- DELETE /api/dashboards/{id}/cards/{cardId} - Remove card
- PUT /api/dashboards/{id}/cards/{cardId}/position - Move card
- PUT /api/dashboards/{id}/layout - Change layout
- POST /api/dashboards/{id}/export - Export dashboard
- POST /api/dashboards/import - Import dashboard

## Card Types
- Chart (line, bar, pie)
- Metric (single value)
- Table (data grid)
- Text (markdown)
- Image
- Calendar
- Timeline
- Custom (extensible)

## Performance
- Dashboard data cached per user
- Lazy load card data
- Pagination for large card sets
- WebSocket for real-time updates
- Debounce position changes
