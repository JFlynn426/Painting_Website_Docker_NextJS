export interface PaintingCategory {
    id: string;
    name: string;
    slug: string;
    description?: string;
}

export const paintingCategories: PaintingCategory[] = [
    {
        id: 'landscapes-cityscapes',
        name: 'Landscapes & Cityscapes',
        slug: 'landscapes-and-cityscapes',
        description: 'Beautiful landscapes and urban cityscapes'
    },
    {
        id: 'seascapes',
        name: 'Seascapes',
        slug: 'seascapes',
        description: 'Ocean and coastal scenes'
    },
    {
        id: 'animals-people',
        name: 'Animals & People',
        slug: 'animals-and-people',
        description: 'Portraits and animal paintings'
    },
    {
        id: 'flowers',
        name: 'Flowers',
        slug: 'flowers',
        description: 'Botanical and floral compositions'
    }
];