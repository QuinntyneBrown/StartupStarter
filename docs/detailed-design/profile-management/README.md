# Profile Management - Detailed Design

## Overview
Multi-profile system allowing users to organize content and dashboards by projects, departments, or teams.

## Key Features
- Multiple profiles per account
- Four profile types (Personal, Project, Department, Team)
- Profile-specific dashboards
- Custom avatars and preferences
- Profile sharing within account
- Default profile per user
- Theme and preference management

## Profile Types
- **Personal**: Individual user workspace
- **Project**: Project-specific collaboration
- **Department**: Department-wide resources
- **Team**: Team collaboration space

## Sharing Model
- Owner has full control
- Three permission levels (View, Edit, Admin)
- Share revocation capability
- Audit trail of sharing changes

## Technology Stack
- .NET 8, MediatR, Entity Framework Core
- Azure Blob Storage for avatars
- JSON for flexible preferences
- Redis for preference caching

## Database Schema
- Profiles table
- ProfileShares table
- ProfileDashboards table (junction)
- UserDefaultProfiles table
