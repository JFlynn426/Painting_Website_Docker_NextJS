import { PaintingImageItem } from '@/components/PaintingGrid';

/**
 * Represents a row of paintings in the grid layout.
 */
export interface PaintingRow {
    items: PaintingImageItem[];
    type: 'landscape' | 'portrait' | 'mixed';
}

/**
 * Builds smart rows from paintings, optimizing for visual balance
 * and minimizing gaps in the grid layout.
 * 
 * Grid constraints:
 * - Total columns: 6
 * - Landscape width: 3 columns (2 per row = 6)
 * - Portrait/Square width: 2 columns (3 per row = 6)
 * 
 * @param images - Array of painting images with orientation
 * @returns Array of painting rows, each containing optimally grouped paintings
 */
export function buildSmartRows(images: PaintingImageItem[]): PaintingRow[] {
    const rows: PaintingRow[] = [];

    // Separate by orientation
    let landscapes: PaintingImageItem[] = images.filter(i => i.orientation === 'landscape');
    let portraits: PaintingImageItem[] = images.filter(i => i.orientation === 'portrait' || i.orientation === 'square');

    // Track which orientation to prefer for alternating rows
    // Start with the orientation that has more paintings
    let preferLandscape = landscapes.length >= portraits.length;

    while (landscapes.length > 0 || portraits.length > 0) {
        const row = buildOptimalRow(landscapes, portraits, preferLandscape);

        if (row.items.length > 0) {
            rows.push(row);

            // Remove used items from arrays
            landscapes = landscapes.filter(i => !row.items.includes(i));
            portraits = portraits.filter(i => !row.items.includes(i));
        } else {
            // No more items can be placed
            break;
        }

        // Alternate preference for visual rhythm
        preferLandscape = !preferLandscape;
    }

    return rows;
}

/**
 * Builds a single optimal row based on available paintings and preference.
 * 
 * @param landscapes - Available landscape paintings
 * @param portraits - Available portrait/square paintings
 * @param preferLandscape - Whether to prefer landscape rows
 * @returns A painting row with optimally selected items
 */
function buildOptimalRow(
    landscapes: PaintingImageItem[],
    portraits: PaintingImageItem[],
    preferLandscape: boolean
): PaintingRow {
    const items: PaintingImageItem[] = [];

    // Priority 1: If preferring landscape and we have 2, use them (3+3=6 columns)
    if (preferLandscape && landscapes.length >= 2) {
        items.push(landscapes[0], landscapes[1]);
        return { items, type: 'landscape' };
    }

    // Priority 2: If preferring portrait and we have 3, use them (2+2+2=6 columns)
    if (!preferLandscape && portraits.length >= 3) {
        items.push(portraits[0], portraits[1], portraits[2]);
        return { items, type: 'portrait' };
    }

    // Priority 3: Try landscape row anyway if available
    if (landscapes.length >= 2) {
        items.push(landscapes[0], landscapes[1]);
        return { items, type: 'landscape' };
    }

    // Priority 4: Try portrait row anyway if available
    if (portraits.length >= 3) {
        items.push(portraits[0], portraits[1], portraits[2]);
        return { items, type: 'portrait' };
    }

    // Priority 5: Mixed row - 1 landscape + up to 2 portraits (3+2+2=7, use 3+2=5)
    if (landscapes.length === 1 && portraits.length >= 1) {
        items.push(landscapes[0]);
        const portraitCount = Math.min(portraits.length, 2);
        for (let i = 0; i < portraitCount; i++) {
            items.push(portraits[i]);
        }
        return { items, type: 'mixed' };
    }

    // Priority 6: Single landscape (leaves 3 columns gap)
    if (landscapes.length === 1) {
        items.push(landscapes[0]);
        return { items, type: 'landscape' };
    }

    // Priority 7: Remaining portraits (1-3)
    if (portraits.length > 0) {
        const count = Math.min(portraits.length, 3);
        for (let i = 0; i < count; i++) {
            items.push(portraits[i]);
        }
        return { items, type: 'portrait' };
    }

    // No items available
    return { items: [], type: 'mixed' };
}