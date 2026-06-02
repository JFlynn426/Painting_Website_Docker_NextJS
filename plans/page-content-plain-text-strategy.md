# Page Content Plain Text Strategy

## Overview

Transform page content from HTML-based storage to plain-text paragraphs, making content editable by HTML-naive users while maintaining consistent styling through the frontend.

## Current State

### Seed Data Structure
Each page content stores HTML with:
- **Outer wrapper**: `<div style='text-align: left; max-width: 800px; margin: 0 auto;'>...</div>`
- **Paragraph wrapper**: `<p style='margin-bottom: 16px;'>text</p>`
- **Inline formatting**: `<strong>`, `<br/>` for emphasis and line breaks

### Rendering
- [`PageContent.tsx`](clientapp/src/components/PageContent.tsx) uses `dangerouslySetInnerHTML` with raw HTML
- Home page [`page.tsx`](clientapp/src/app/(public)/page.tsx) does the same

### Problem
Users must edit raw HTML in the admin panel, which is not user-friendly.

## Proposed Solution

### Strategy: Frontend-Generated HTML from Plain Text

Store plain text in the database where each paragraph is separated by double newlines (`\n\n`). The frontend generates the HTML structure with consistent styling.

### Data Flow

```mermaid
flowchart LR
    A[Admin edits plain text] --> B[Database stores plain text]
    B --> C[Frontend splits by newlines]
    C --> D[Each paragraph wrapped in styled p tag]
    D --> E[Container div wraps all paragraphs]
```

### Implementation Steps

#### 1. Update Seed Data
Remove HTML wrappers from [`PageContentsSeedData.cs`](ServerApp/ServerApp.Infrastructure/SeedData/PageContentsSeedData.cs):

**Before:**
```html
<div style='text-align: left; max-width: 800px; margin: 0 auto;'>
<p style='margin-bottom: 16px;'>Paragraph 1</p>
<p style='margin-bottom: 16px;'>Paragraph 2</p>
</div>
```

**After:**
```
Paragraph 1

Paragraph 2
```

#### 2. Create Paragraph Rendering Helper
Create a new helper function in [`sanitization.ts`](clientapp/src/lib/sanitization.ts):

```typescript
export function renderParagraphs(content: string): string {
    if (!content) return '';
    // Split by double newline, filter empty paragraphs
    const paragraphs = content.split('\n\n')
        .map(p => p.trim())
        .filter(p => p.length > 0)
        .map(p => `<p class='pb-4'>${escapeHtml(p)}</p>`);
    return paragraphs.join('');
}
```

#### 3. Update PageContent Component
Modify [`PageContent.tsx`](clientapp/src/components/PageContent.tsx):

```tsx
return (
    <div className='flex flex-col items-center justify-center p-4 sm:p-2 md:p-1 text-[var(--foreground)]'>
        {pageContent.title && <h1 className={titleClassName}>{pageContent.title}</h1>}
        {pageContent.photoUrl && (
            <div className='relative place-self-center mb-6 w-[300px] h-[300px]'>
                <Image src={pageContent.photoUrl} alt='...' fill className='rounded-lg object-contain' />
            </div>
        )}
        <div className={contentClassName} style={{ textAlign: 'left', maxWidth: '800px', margin: '0 auto' }}>
            <div dangerouslySetInnerHTML={{ __html: renderParagraphs(pageContent.content) }} />
        </div>
    </div>
);
```

#### 4. Update Home Page
Modify [`page.tsx`](clientapp/src/app/(public)/page.tsx) to use the same pattern.

#### 5. Update Admin Edit Page
Replace the HTML textarea with a plain text textarea in [`page.tsx`](clientapp/src/app/(admin)/admin/content/edit/[slug]/page.tsx):

- Remove HTML editor
- Add plain text textarea with instructions
- Show preview of rendered paragraphs

#### 6. Update Backend
- No schema changes required (Content remains nvarchar(max))
- Update [`PageContentText.cs`](ServerApp/ServerApp.Domain/ValueObjects/Page/PageContentText.cs) validation if needed

### Benefits
1. **User-friendly**: Users edit plain text with double newlines for paragraphs
2. **Consistent styling**: All pages use the same paragraph styling
3. **No HTML injection**: Frontend generates safe HTML from plain text
4. **Simple migration**: Just remove HTML tags from existing content

### Migration Notes
- Existing HTML content needs one-time cleanup to extract plain text
- Double newlines (`\n\n`) separate paragraphs
- Single newlines (`\n`) preserved within paragraphs
- No inline HTML formatting supported (users can use *emphasis* if needed in future)

## Files to Modify

| File | Change |
|------|--------|
| `PageContentsSeedData.cs` | Remove HTML wrappers, store plain text |
| `sanitization.ts` | Add `renderParagraphs` helper |
| `PageContent.tsx` | Use helper, add container styling |
| `page.tsx` (home) | Use helper, add container styling |
| `page.tsx` (admin edit) | Plain text textarea instead of HTML |

## Formatting Support

- **Allowed inline tags**: `<strong>` for bold, `<br/>` for line breaks
- **Toolbar buttons**: Admin edit page provides buttons to insert `<strong>` and `<br/>` tags
- **Paragraph separation**: Double newline (`\n\n`) creates new paragraph
- **Single newline**: Preserved within paragraphs (converted to `<br/>` on render)

## Admin Toolbar Design

Simple toolbar above the textarea with:
- **B** button - wraps selection in `<strong>...</strong>`
- **BR** button - inserts `<br/>` at cursor
- Live preview panel showing rendered output

## Questions for Confirmation

1. Should we support any inline formatting (bold, italic) or keep it plain text only?
2. Should the admin edit page show a live preview of the rendered paragraphs?
3. Should we preserve single newlines within paragraphs or treat all newlines as paragraph separators?