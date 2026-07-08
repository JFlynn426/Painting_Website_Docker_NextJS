import DOMPurify from 'dompurify';

/**
 * Client-side DOMPurify instance.
 * Uses the browser's native window/document - no JSDOM needed.
 */
function getClientDOMPurify() {
    // DOMPurify works natively in the browser
    return DOMPurify;
}

/**
 * Internal helper - splits content into paragraphs and wraps in <p> tags.
 * Normalizes line endings (\r\n -> \n) before splitting to handle Windows C# verbatim strings.
 * Supports [align:center], [align:left], [align:right] markers at the start of content.
 */
function buildParagraphs(content: string): string {
    // Normalize Windows line endings to Unix line endings
    let normalized = content.replace(/\r\n/g, '\n');

    // Check for alignment marker at the start
    let alignment = '';
    const alignMatch = normalized.match(/^\[align:(center|left|right)\]\s*\n?/);
    if (alignMatch) {
        alignment = ` style="text-align: ${alignMatch[1]}"`;
        normalized = normalized.replace(/^\[align:(center|left|right)\]\s*\n?/, '');
    }

    const paragraphs = normalized.split('\n\n')
        .map(p => p.trim())
        .filter(p => p.length > 0)
        .map(p => {
            // Convert single newlines to <br> tags within paragraphs
            const withBreaks = p.replace(/\n/g, '<br>');
            return `<p class='pb-4'${alignment}>${withBreaks}</p>`;
        });
    return paragraphs.join('');
}

/**
 * Renders content as HTML paragraphs (CLIENT-SAFE).
 * If content is already HTML (contains <p> tags), sanitizes it directly.
 * If content is plain text, splits by double newlines to create paragraphs.
 * Uses DOMPurify with browser's native DOM for proper sanitization.
 * Preserves <strong>, <em>, <u>, and <br/> tags within paragraphs.
 * Supports [align:center], [align:left], [align:right] markers.
 *
 * @param content - HTML or plain text content with optional formatting tags
 * @returns Sanitized HTML string with paragraphs wrapped in <p> tags
 */
export function renderParagraphsClient(content: string): string {
    if (!content || typeof content !== 'string') {
        return '';
    }

    const purify = getClientDOMPurify();

    // If content already contains <p> tags, it's HTML - sanitize directly
    if (content.includes('<p ')) {
        return purify.sanitize(content, {
            ALLOWED_TAGS: ['p', 'strong', 'em', 'u', 'br'],
            ALLOWED_ATTR: ['class', 'style']
        });
    }

    // Otherwise, treat as plain text and build paragraphs
    const html = buildParagraphs(content);

    return purify.sanitize(html, {
        ALLOWED_TAGS: ['p', 'strong', 'em', 'u', 'br'],
        ALLOWED_ATTR: ['class', 'style']
    });
}

/**
 * Converts HTML from contentEditable back to plain text format for storage.
 * Extracts alignment from the first <p> tag's style attribute.
 * Converts <p> tags back to double newlines.
 * Preserves <strong>, <em>, <u>, and <br/> tags.
 *
 * @param html - HTML string from contentEditable div
 * @returns Plain text content with [align:xxx] marker and double newline paragraph separators
 */
export function htmlToPlainText(html: string): string {
    if (!html || typeof html !== 'string') {
        return '';
    }

    // Create a temporary container to parse the HTML
    const temp = document.createElement('div');
    temp.innerHTML = html;

    // Check for alignment on the first paragraph
    const firstP = temp.querySelector('p');
    let alignMarker = '';
    if (firstP) {
        const textAlign = firstP.style.textAlign;
        if (textAlign && ['center', 'left', 'right'].includes(textAlign)) {
            alignMarker = `[align:${textAlign}]\n`;
            // Remove alignment style from all paragraphs
            temp.querySelectorAll('p').forEach(p => {
                (p as HTMLElement).style.textAlign = '';
            });
        }
    }

    // Extract text content from each paragraph, preserving inline HTML tags
    const paragraphs: string[] = [];
    temp.querySelectorAll('p').forEach(p => {
        // Get innerHTML to preserve <strong>, <em>, <u> tags
        let inner = p.innerHTML.trim();
        // Convert <br> tags back to newlines FIRST (before whitespace cleanup)
        inner = inner.replace(/<br\s*\/?>/gi, '\n');
        // Convert <br> non-self-closing tags as well
        inner = inner.replace(/<br>/gi, '\n');
        // Clean up extra whitespace on each line individually to preserve newline structure
        inner = inner.split('\n')
            .map(line => line.replace(/\s+/g, ' ').trim())
            .filter(line => line.length > 0)
            .join('\n');
        if (inner) {
            paragraphs.push(inner);
        }
    });

    // If no paragraphs found, return the raw HTML stripped of <p> wrapper
    if (paragraphs.length === 0) {
        const text = temp.textContent || '';
        return text.trim();
    }

    return alignMarker + paragraphs.join('\n\n');
}

/**
 * Sanitizes HTML from contentEditable for safe storage.
 * Uses DOMPurify with a strict whitelist of allowed tags and attributes.
 * Preserves <p>, <strong>, <em>, <u>, <br>, and style/class attributes.
 *
 * @param html - HTML string from contentEditable div
 * @returns Sanitized HTML string safe for storage and rendering
 */
export function sanitizeHtmlForStorage(html: string): string {
    if (!html || typeof html !== 'string') {
        return '';
    }

    const purify = getClientDOMPurify();
    return purify.sanitize(html, {
        ALLOWED_TAGS: ['p', 'strong', 'em', 'u', 'br', 'b', 'i', 's'],
        ALLOWED_ATTR: ['class', 'style']
    });
}