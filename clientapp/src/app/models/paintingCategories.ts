export interface PaintingCategory {
    id: string;
    name: string;
    slug: string;
    description?: string;
    paintingCount?: number;
}

export const paintingCategories: PaintingCategory[] = [
    {
        id: 'landscapes-cityscapes',
        name: 'Landscapes & Cityscapes',
        slug: 'landscapes',
        description: 'Beautiful landscapes and urban cityscapes',
        paintingCount: 0
    },
    {
        id: 'seascapes',
        name: 'Seascapes',
        slug: 'seascapes',
        description: 'Ocean and coastal scenes', // Updated description to match suggested edit
        paintingCount: 0
    },
    {
        id: 'animals-people',
        name: 'Animals & People',
        slug: 'animals',
        description: 'Portraits and animal paintings',
        paintingCount: 0
    },
    {
        id: 'flowers',
        name: 'Flowers',
        slug: 'flowers',
        description: 'Botanical and floral compositions',
        paintingCount: 0
    }
];