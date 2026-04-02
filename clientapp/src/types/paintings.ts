/**
 * Painting interface - matches ServerApp.Application.DTOs.PaintingDto
 */
export interface Painting {
    id: string;
    slug: string;
    title: string;
    description?: string;
    imageUrl: string;
    thumbnailUrl?: string;
    categorySlug: string;
    width?: number;
    height?: number;
    depth?: number;
    year?: number;
    price?: number;
    isAvailable: boolean;
    isNew: boolean;
}

/**
 * Painting category interface - matches ServerApp.Application.DTOs.PaintingCategoryDto
 */
export interface PaintingCategory {
    id: string;
    slug: string;
    name: string;
    description?: string;
}

/**
 * Painting category with paintings - matches ServerApp.Application.DTOs.PaintingCategoryWithPaintingsDto
 */
export interface PaintingCategoryWithPaintings extends PaintingCategory {
    paintings: Painting[];
}