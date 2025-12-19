# Role Management - Detailed Design

## Overview
Role-based access control (RBAC) system for managing user permissions across the platform.

## Key Features
- Dynamic role creation per account
- Permission-based authorization
- Role assignment/revocation with audit trail
- Hierarchical permission inheritance
- Built-in and custom roles

## Technology Stack
- .NET 8, MediatR, Entity Framework Core
- ASP.NET Core Authorization policies
- Claims-based authentication
- Redis for permission caching

## RBAC Pattern
- Roles contain sets of permissions
- Users are assigned roles
- Permissions checked via middleware
- Authorization policies enforce access

## Database Schema
- Roles table
- RolePermissions table (many-to-many)
- UserRoles table (many-to-many)
- Permissions table (predefined)
