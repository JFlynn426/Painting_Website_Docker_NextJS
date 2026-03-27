export interface PageContent {
    address: string;
    title: string;
    content: string;
}

// Dummy data for page content - will be replaced with API data
export const pageContentData: PageContent[] = [
    {
        address: 'home',
        title: 'Welcome to My Art Gallery',
        content: 'Explore a curated collection of original paintings featuring landscapes, seascapes, animals, and flowers. Each piece is created with passion and attention to detail, capturing the beauty of the natural world.'
    }
];