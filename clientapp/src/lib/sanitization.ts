import DOMPurify from 'dompurify';
import { JSDOM } from 'jsdom';

/**
 * Initialize DOMPurify with a virtual DOM environment for server-side rendering.
 * DOMPurify requires a window/document object which isn't available in Node.js by default.
 */
function getDOMPurify() {
    if (typeof window === 'undefined') {
        // Server-side: create a virtual DOM environment
        const dom = new JSDOM('<!DOCTYPE html><html><body></body></html>');
        // Return DOMPurify initialized with the JSDOM window
        return DOMPurify(dom.window);
    }
    // Client-side: return DOMPurify as-is (it will use the browser's window)
    return DOMPurify;
}

/**
 * Sanitizes HTML content to prevent XSS attacks.
 * Uses a strict whitelist approach - only explicitly allowed tags and attributes are permitted.
 *
 * @param html - The raw HTML string to sanitize
 * @returns Sanitized HTML string safe for use with dangerouslySetInnerHTML
 */
export function sanitizeHtml(html: string): string {
    if (!html || typeof html !== 'string') {
        return '';
    }

    const purify = getDOMPurify();
    return purify.sanitize(html, {
        // Strict whitelist of allowed HTML tags
        ALLOWED_TAGS: [
            // Text formatting
            'p', 'br', 'strong', 'em', 'u', 's', 'i', 'b',
            // Headings
            'h1', 'h2', 'h3', 'h4', 'h5', 'h6',
            // Lists
            'ul', 'ol', 'li',
            // Block elements
            'blockquote', 'code', 'pre',
            // Media and links
            'a', 'img',
            // Container elements
            'div', 'span',
            // Horizontal rule
            'hr',
            // Tables
            'table', 'thead', 'tbody', 'tr', 'th', 'td'
        ],
        // Strict whitelist of allowed attributes
        ALLOWED_ATTR: [
            // Links
            'href', 'target', 'rel',
            // Images
            'src', 'alt', 'title',
            // Tables
            'colspan', 'rowspan',
            // Inline styles (for layout purposes)
            'style'
        ]
    });
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
 * Sanitizes HTML allowing only <strong> and <br/> within <p> tags.
 */
function sanitizeParagraphs(html: string): string {
    const purify = getDOMPurify();
    return purify.sanitize(html, {
        ALLOWED_TAGS: ['p', 'strong', 'em', 'u', 'br'],
        ALLOWED_ATTR: ['class', 'style']
    });
}

/**
 * Renders plain text content as HTML paragraphs (SERVER-SIDE ONLY).
 * Splits content by double newlines (\\n\\n) to create paragraphs.
 * Preserves <strong> and <br/> tags within paragraphs.
 *
 * @param content - Plain text content with optional <strong> and <br/> tags
 * @returns HTML string with paragraphs wrapped in <p> tags
 */
export function renderParagraphs(content: string): string {
    if (!content || typeof content !== 'string') {
        return '';
    }
    const html = buildParagraphs(content);
    return sanitizeParagraphs(html);
}

/**
 * Sanitizes a URL to prevent protocol-based XSS attacks.
 * Uses DOMPurify.sanitize with a dummy anchor element to validate the URL.
 * 
 * @param url - The URL string to sanitize
 * @returns Sanitized URL string, or null if the URL is not safe
 */
export function sanitizeUrl(url: string | null): string | null {
    if (!url) {
        return null;
    }

    // DOMPurify.sanitize will strip dangerous protocols from href attributes
    const tempElement = document.createElement('a');
    tempElement.href = url;
    const sanitized = DOMPurify.sanitize(tempElement.outerHTML);

    // Extract the href from the sanitized HTML
    const tempContainer = document.createElement('div');
    tempContainer.innerHTML = sanitized;
    const sanitizedAnchor = tempContainer.querySelector('a');

    // If the anchor was removed or href is dangerous, return null
    if (!sanitizedAnchor || !sanitizedAnchor.href) {
        return null;
    }

    // Check if the protocol is safe
    const validProtocols = ['http:', 'https:', 'mailto:'];
    const protocol = sanitizedAnchor.protocol;

    if (!validProtocols.includes(protocol)) {
        return null;
    }

    return sanitizedAnchor.href;
}