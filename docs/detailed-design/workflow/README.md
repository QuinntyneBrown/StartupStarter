# Workflow - Detailed Design

## Overview
Content approval workflow system supporting multi-stage approvals and reassignments.

## Key Features
- Multi-stage approval workflows
- Configurable workflow types per account
- Approval/rejection with comments
- Task reassignment capabilities
- Workflow duration tracking
- Email notifications at each stage

## Workflow Stages
- Draft → Review → Approval → Published
- Support for multiple approval levels
- Parallel and sequential approvals
- Conditional routing based on content type

## Technology Stack
- .NET 8, MediatR, Entity Framework Core
- Azure Service Bus for workflow events
- SignalR for real-time status updates
- Email service for notifications

## Database Schema
- Workflows table
- WorkflowStages table
- WorkflowApprovals table
- WorkflowAssignments table
