# ESLint Warnings Remediation Plan

## Overview

This document tracks the ESLint warnings found in the Next.js client application and provides specific mitigation strategies. This plan addresses unused imports, unused state variables, and adds an upload spinner modal component.

**Note:** `<img>` element warnings (`@next/next/no-img-element`) are intentionally **NOT** being fixed per user request. These are acceptable for admin panel image previews where dynamic sizing is needed.

---

## Warnings Being Fixed

| # | File | Line | Rule | Issue | Action |
|---|------|------|------|-------|--------|
| 1 | [`painting-actions.ts`](clientapp/src/actions/painting-actions.ts:5) | 5 | `@typescript-eslint/no-unused-vars` | `CacheTags` imported but never used | Remove import |
| 2 | [`edit/[slug]/page.tsx`](clientapp/src/app/(admin)/admin/content/edit/[slug]/page.tsx:25) | 25 | `@typescript-eslint/no-unused-vars` | `isUploading` state never used | Replace with UploadSpinnerModal |
| 3 | [`add/[slug]/page.tsx`](clientapp/src/app/(admin)/admin/paintings/add/[slug]/page.tsx:19) | 19 | `@typescript-eslint/no-unused-vars` | `uploadedFileName` state never used | Remove state |
| 4 | [`edit/.../page.tsx`](clientapp/src/app/(admin)/admin/paintings/edit/[categorySlug]/[paintingSlug]/page.tsx:27) | 27 | `@typescript-eslint/no-unused-vars` | `uploadedFileName` state never used | Remove state |
| 5 | [`edit/.../page.tsx`](clientapp/src/app/(admin)/admin/paintings/edit/[categorySlug]/[paintingSlug]/page.tsx:35) | 35 | `@typescript-eslint/no-unused-vars` | `isLandscape` state never used | Remove state |
| 6 | [`[[...slug]]/page.tsx`](clientapp/src/app/(public)/paintings/[[...slug]]/page.tsx:1) | 1 | `@typescript-eslint/no-unused-vars` | `getCategoryData` imported but never used | Remove import |

## Warnings Intentionally Ignored

| # | File | Line | Rule | Reason |
|---|------|------|------|--------|
| 4 | [`add/[slug]/page.tsx`](clientapp/src/app/(admin)/admin/paintings/add/[slug]/page.tsx:256) | 256 | `@next/next/no-img-element` | Admin preview - dynamic sizing needed |
| 7 | [`edit/.../page.tsx`](clientapp/src/app/(admin)/admin/paintings/edit/[categorySlug]/[paintingSlug]/page.tsx:319) | 319 | `@next/next/no-img-element` | Admin preview - dynamic sizing needed |
| 8 | [`edit/.../page.tsx`](clientapp/src/app/(admin)/admin/paintings/edit/[categorySlug]/[paintingSlug]/page.tsx:345) | 345 | `@next/next/no-img-element` | Admin preview - dynamic sizing needed |
| 10 | [`AdminHeader.tsx`](clientapp/src/components/AdminHeader.tsx:64) | 64 | `@next/next/no-img-element` | External Google avatar URL |

---

## New Feature: UploadSpinnerModal Component

### Overview

A modal overlay component that displays a spinner while images are uploading. The modal freezes the entire page to prevent user interaction during upload.

### Component Specification

**File:** [`clientapp/src/components/UploadSpinnerModal.tsx`](clientapp/src/components/UploadSpinnerModal.tsx) (new)

**Props:**
```typescript
interface UploadSpinnerModalProps {
    isVisible: boolean;
}
```

**Behavior:**
- When `isVisible` is true, displays a full-screen overlay with a centered spinner
- Blocks all interaction with the underlying page
- Prevents body scroll when visible
- No close button (disappears automatically when upload completes)

**Implementation:**
```tsx
'use client';

interface UploadSpinnerModalProps {
    isVisible: boolean;
}

export default function UploadSpinnerModal({ isVisible }: UploadSpinnerModalProps) {
    if (!isVisible) return null;

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50 backdrop-blur-sm">
            <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-8 flex flex-col items-center gap-4 shadow-xl">
                {/* Spinner */}
                <div className="w-12 h-12 border-4 border-blue-600 border-t-transparent rounded-full animate-spin" />
                <p className="text-gray-200 text-lg font-medium">Uploading image...</p>
            </div>
        </div>
    );
}
```

---

## Detailed Implementation Changes

### 1. Create UploadSpinnerModal Component

**File:** [`clientapp/src/components/UploadSpinnerModal.tsx`](clientapp/src/components/UploadSpinnerModal.tsx) (new file)

See component specification above.

---

### 2. Fix: Remove `CacheTags` import in painting-actions.ts

**File:** [`clientapp/src/actions/painting-actions.ts`](clientapp/src/actions/painting-actions.ts:5)

**Change:**
```typescript
// BEFORE:
import { CacheTags, paintingMutationTags, categoryAssignmentTags } from '@/lib/cache-tags';

// AFTER:
import { paintingMutationTags, categoryAssignmentTags } from '@/lib/cache-tags';
```

---

### 3. Fix: Remove `getCategoryData` import in [[...slug]]/page.tsx

**File:** [`clientapp/src/app/(public)/paintings/[[...slug]]/page.tsx`](clientapp/src/app/(public)/paintings/[[...slug]]/page.tsx:1)

**Change:**
```typescript
// BEFORE:
import { getPaintingBySlug, getCategoryData } from "@/lib/api";

// AFTER:
import { getPaintingBySlug } from "@/lib/api";
```

---

### 4. Fix: Remove `uploadedFileName` state in add painting page

**File:** [`clientapp/src/app/(admin)/admin/paintings/add/[slug]/page.tsx`](clientapp/src/app/(admin)/admin/paintings/add/[slug]/page.tsx:19)

**Changes:**
1. Remove state declaration (line 19):
```typescript
// REMOVE:
const [uploadedFileName, setUploadedFileName] = useState<string | null>(null);
```

2. Remove all `setUploadedFileName` calls in `handleFileUpload` function.

3. Add UploadSpinnerModal import and usage:
```typescript
import UploadSpinnerModal from '@/components/UploadSpinnerModal';
```

Replace `isUploading` inline text with modal:
```tsx
// Add near the end of the JSX, before closing </div> or </main>:
<UploadSpinnerModal isVisible={isUploading} />
```

---

### 5. Fix: Remove `uploadedFileName` and `isLandscape` states in edit painting page

**File:** [`clientapp/src/app/(admin)/admin/paintings/edit/[categorySlug]/[paintingSlug]/page.tsx`](clientapp/src/app/(admin)/admin/paintings/edit/[categorySlug]/[paintingSlug]/page.tsx)

**Changes:**
1. Remove state declarations:
```typescript
// REMOVE line 27:
const [uploadedFileName, setUploadedFileName] = useState<string | null>(null);

// REMOVE line 35:
const [isLandscape, setIsLandscape] = useState(true);
```

2. Remove all `setUploadedFileName` and `setIsLandscape` calls.

3. Add UploadSpinnerModal import and usage:
```typescript
import UploadSpinnerModal from '@/components/UploadSpinnerModal';
```

```tsx
<UploadSpinnerModal isVisible={isUploading} />
```

---

### 6. Fix: Replace `isUploading` with UploadSpinnerModal in content edit page

**File:** [`clientapp/src/app/(admin)/admin/content/edit/[slug]/page.tsx`](clientapp/src/app/(admin)/admin/content/edit/[slug]/page.tsx)

**Changes:**
1. The `isUploading` state already exists but is unused. Keep it and wire it to the modal.

2. Add UploadSpinnerModal import:
```typescript
import UploadSpinnerModal from '@/components/UploadSpinnerModal';
```

3. Add modal to JSX:
```tsx
<UploadSpinnerModal isVisible={isUploading} />
```

---

## Pages Requiring UploadSpinnerModal Integration

| Page | File | Has `isUploading` state | Action |
|------|------|------------------------|--------|
| Add Painting | `admin/paintings/add/[slug]/page.tsx` | Yes (line 29) | Wire to modal |
| Edit Painting | `admin/paintings/edit/[categorySlug]/[paintingSlug]/page.tsx` | Yes (line 37) | Wire to modal |
| Edit Content | `admin/content/edit/[slug]/page.tsx` | Yes (line 25) | Wire to modal |

---

## Implementation Checklist

- [ ] Create [`UploadSpinnerModal.tsx`](clientapp/src/components/UploadSpinnerModal.tsx) component
- [ ] Fix #1: Remove `CacheTags` import in [`painting-actions.ts`](clientapp/src/actions/painting-actions.ts:5)
- [ ] Fix #2: Remove `getCategoryData` import in [`[[...slug]]/page.tsx`](clientapp/src/app/(public)/paintings/[[...slug]]/page.tsx:1)
- [ ] Fix #3: Remove `uploadedFileName` state in [`add/[slug]/page.tsx`](clientapp/src/app/(admin)/admin/paintings/add/[slug]/page.tsx:19)
- [ ] Fix #4: Remove `uploadedFileName` state in [`edit/[categorySlug]/[paintingSlug]/page.tsx`](clientapp/src/app/(admin)/admin/paintings/edit/[categorySlug]/[paintingSlug]/page.tsx:27)
- [ ] Fix #5: Remove `isLandscape` state in [`edit/[categorySlug]/[paintingSlug]/page.tsx`](clientapp/src/app/(admin)/admin/paintings/edit/[categorySlug]/[paintingSlug]/page.tsx:35)
- [ ] Wire UploadSpinnerModal in [`add/[slug]/page.tsx`](clientapp/src/app/(admin)/admin/paintings/add/[slug]/page.tsx)
- [ ] Wire UploadSpinnerModal in [`edit/[categorySlug]/[paintingSlug]/page.tsx`](clientapp/src/app/(admin)/admin/paintings/edit/[categorySlug]/[paintingSlug]/page.tsx)
- [ ] Wire UploadSpinnerModal in [`edit/[slug]/page.tsx`](clientapp/src/app/(admin)/admin/content/edit/[slug]/page.tsx)
- [ ] Run `npm run lint` to confirm warnings resolved (4 remaining `<img>` warnings expected)
- [ ] Run `npm run build` to confirm no build errors
