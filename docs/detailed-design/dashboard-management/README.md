# Dashboard Management - Detailed Design

## Overview
Customizable dashboard system with drag-and-drop cards, sharing, and multiple layout options.

## Key Features
- Multiple dashboards per profile
- Customizable cards (widgets)
- Drag-and-drop positioning
- Three layout types (Grid, Masonry, Freeform)
- Dashboard sharing with permission levels
- Template system for quick creation
- Export/Import functionality
- Default dashboard per user

## Card Types
- Metrics cards
- Chart cards (line, bar, pie)
- Table/list cards
- Custom content cards
- Real-time data cards

## Technology Stack
- .NET 8, MediatR, Entity Framework Core
- Angular 21 with Angular CDK (drag-drop)
- SignalR for real-time updates
- JSON storage for card configuration

## Database Schema
- Dashboards table
- DashboardCards table
- DashboardShares table
- CardTemplates table
