# About Page Photo Upload Feature

## Overview
Add photo upload capability to /admin/content/edit/about mirroring the painting edit pattern.

## Design
- PhotoUrl as nullable property on PageContent entity
- Base64 data URL storage (same as paintings)
- PagePhotoUrl Value Object in Domain layer
- PageContent component renders photo when present

## Implementation Steps

### Backend - Domain Layer
1. Create PagePhotoUrl.cs (ValueObject extending StringValueObject, MaxLength=2000)
2. Update PageContent.cs - add PhotoUrl property, update Update() method
3. Update IPageContentFactory.cs / PageContentFactory.cs - add photoUrl parameter

### Backend - Infrastructure Layer
4. Update PageContentConfiguration.cs - add PhotoUrl column mapping (nvarchar(max), nullable)
5. Create EF Migration for PhotoUrl column
6. Update PageContentsSeedData.cs - extract photo URL from about page HTML into PhotoUrl field

### Backend - Application Layer
7. Update PageContentDto.cs - add PhotoUrl property
8. Update UpdatePageContentRequest.cs - add PhotoUrl property
9. Update UpdatePageContent.cs command - add PhotoUrl field
10. Update UpdatePageContentHandler.cs - pass PhotoUrl to entity.Update()

### Backend - API Layer
11. Update PageContentController.cs - pass photoUrl from request to command

### Frontend - Types and API
12. Update page-content.ts - add photoUrl to PageContent interface
13. Update api.ts - map photoUrl in getPageContent and getAllPageContents
14. Update UpdatePageContentRequest interface - add photoUrl

### Frontend - Components
15. Update PageContent.tsx - render photo when photoUrl exists
16. Update /admin/content/edit/[slug]/page.tsx - add file upload UI matching painting edit pattern

## File Changes Summary
- New: ServerApp.Domain/ValueObjects/Page/PagePhotoUrl.cs
- New: EF Migration file
- Modified: 15 existing files across Domain, Infrastructure, Application, API