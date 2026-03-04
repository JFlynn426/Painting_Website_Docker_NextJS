export interface Painting {
    id: string;
    title: string;
    description: string;
    imageUrl: string;
    thumbnailUrl?: string;
    category: string;
    dimensions?: string;
    year?: number;
    price: number;
    isAvailable?: boolean;
}

// Dummy data for all paintings - will be replaced with API data
export const paintingsData: Painting[] = [
    // Landscapes and Cityscapes
    {
        id: 'landscape-1',
        title: 'Aspens',
        description: 'A serene autumn scene featuring golden aspen trees against a mountain backdrop.',
        imageUrl: '/Landscapes and Cityscapes/Aspens.jpg',
        category: 'landscapes',
        dimensions: '24x36 inches',
        year: 2023,
        price: 1200,
        isAvailable: true
    },
    {
        id: 'landscape-2',
        title: 'Bahia Honda',
        description: 'A tropical paradise capturing the essence of the Florida Keys.',
        imageUrl: '/Landscapes and Cityscapes/Bahia Honda .jpg',
        category: 'landscapes',
        dimensions: '30x40 inches',
        year: 2022,
        price: 1500,
        isAvailable: true
    },
    {
        id: 'landscape-3',
        title: 'Baked Goods',
        description: 'A charming street scene featuring a local bakery.',
        imageUrl: '/Landscapes and Cityscapes/BakedGoods.JPG',
        category: 'landscapes',
        dimensions: '18x24 inches',
        year: 2023,
        price: 800,
        isAvailable: false
    },
    {
        id: 'landscape-4',
        title: 'Banyans',
        description: 'Majestic banyan trees creating a natural cathedral.',
        imageUrl: '/Landscapes and Cityscapes/Banyans.jpg',
        category: 'landscapes',
        dimensions: '36x48 inches',
        year: 2022,
        price: 2000,
        isAvailable: true
    },
    {
        id: 'landscape-5',
        title: 'Brady & Duke',
        description: 'A heartwarming portrait of two beloved companions.',
        imageUrl: '/Landscapes and Cityscapes/Brady & Duke.jpg',
        category: 'landscapes',
        dimensions: '20x24 inches',
        year: 2023,
        price: 950,
        isAvailable: true
    },
    {
        id: 'landscape-6',
        title: 'Bridge World',
        description: 'An architectural study of bridges connecting worlds.',
        imageUrl: '/Landscapes and Cityscapes/Bridge world.jpg',
        category: 'landscapes',
        dimensions: '24x30 inches',
        year: 2021,
        price: 1100,
        isAvailable: true
    },
    {
        id: 'landscape-7',
        title: 'Castelvetore',
        description: 'A picturesque Italian village bathed in warm sunlight.',
        imageUrl: '/Landscapes and Cityscapes/Castelvetore .JPG',
        category: 'landscapes',
        dimensions: '20x28 inches',
        year: 2022,
        price: 1300,
        isAvailable: true
    },
    {
        id: 'landscape-8',
        title: 'Cobblestone Way',
        description: 'An old-world charm captured in cobblestone streets.',
        imageUrl: '/Landscapes and Cityscapes/Cobblestone Way.JPG',
        category: 'landscapes',
        dimensions: '16x20 inches',
        year: 2023,
        price: 750,
        isAvailable: false
    },
    {
        id: 'landscape-9',
        title: 'Forest Mystery',
        description: 'Enigmatic forest scene with dappled light filtering through trees.',
        imageUrl: '/Landscapes and Cityscapes/Forest Mystery.jpg',
        category: 'landscapes',
        dimensions: '30x36 inches',
        year: 2022,
        price: 1600,
        isAvailable: true
    },
    {
        id: 'landscape-10',
        title: 'Hydrangea Time',
        description: 'Lush hydrangeas in full bloom creating a garden paradise.',
        imageUrl: '/Landscapes and Cityscapes/Hydrangea Time.jpeg',
        category: 'landscapes',
        dimensions: '24x24 inches',
        year: 2023,
        price: 900,
        isAvailable: true
    },
    {
        id: 'landscape-11',
        title: 'Italian Church',
        description: 'A historic church standing as a testament to faith and artistry.',
        imageUrl: '/Landscapes and Cityscapes/Italian Church.jpg',
        category: 'landscapes',
        dimensions: '28x36 inches',
        year: 2021,
        price: 1400,
        isAvailable: true
    },
    {
        id: 'landscape-12',
        title: 'Mine',
        description: 'An industrial landscape exploring the beauty of working environments.',
        imageUrl: '/Landscapes and Cityscapes/Mine (1).jpg',
        category: 'landscapes',
        dimensions: '32x40 inches',
        year: 2022,
        price: 1700,
        isAvailable: false
    },
    {
        id: 'landscape-13',
        title: 'New Inhabitants',
        description: 'Nature reclaiming space with new life emerging.',
        imageUrl: '/Landscapes and Cityscapes/New Inhabitants.jpeg',
        category: 'landscapes',
        dimensions: '20x24 inches',
        year: 2023,
        price: 850,
        isAvailable: true
    },
    {
        id: 'landscape-14',
        title: 'Rainbow River',
        description: 'A vibrant river scene with colorful reflections.',
        imageUrl: '/Landscapes and Cityscapes/Rainbow River1.jpg',
        category: 'landscapes',
        dimensions: '24x32 inches',
        year: 2022,
        price: 1250,
        isAvailable: true
    },
    {
        id: 'landscape-15',
        title: 'Sedona',
        description: 'The iconic red rocks of Sedona in golden hour light.',
        imageUrl: '/Landscapes and Cityscapes/Sedona.JPG',
        category: 'landscapes',
        dimensions: '30x40 inches',
        year: 2021,
        price: 1800,
        isAvailable: true
    },
    {
        id: 'landscape-16',
        title: 'The Path',
        description: 'A winding path inviting viewers into a contemplative journey.',
        imageUrl: '/Landscapes and Cityscapes/The Path.jpg',
        category: 'landscapes',
        dimensions: '24x30 inches',
        year: 2023,
        price: 1100,
        isAvailable: true
    },
    {
        id: 'landscape-17',
        title: 'The Pond',
        description: 'A tranquil pond reflecting the surrounding nature.',
        imageUrl: '/Landscapes and Cityscapes/The Pond.jpg',
        category: 'landscapes',
        dimensions: '20x28 inches',
        year: 2022,
        price: 950,
        isAvailable: false
    },
    {
        id: 'landscape-18',
        title: 'Window Jigsaw',
        description: 'An abstract view through windows creating a puzzle of perspectives.',
        imageUrl: '/Landscapes and Cityscapes/Window Jigsaw.jpg',
        category: 'landscapes',
        dimensions: '18x24 inches',
        year: 2023,
        price: 800,
        isAvailable: true
    },
    // Seascapes
    {
        id: 'seascape-1',
        title: 'Cloud Creatures',
        description: 'Whimsical cloud formations dancing across the sky above the ocean.',
        imageUrl: '/Seascapes/Cloud Creatures2.jpg',
        category: 'seascapes',
        dimensions: '30x40 inches',
        year: 2023,
        price: 1600,
        isAvailable: true
    },
    {
        id: 'seascape-2',
        title: 'Contemplation',
        description: 'A moment of quiet reflection by the waters edge.',
        imageUrl: '/Seascapes/Contemplation.jpeg',
        category: 'seascapes',
        dimensions: '24x32 inches',
        year: 2022,
        price: 1200,
        isAvailable: true
    },
    {
        id: 'seascape-3',
        title: 'Double Roll',
        description: 'Two waves rolling in perfect harmony toward the shore.',
        imageUrl: '/Seascapes/Double Roll.jpg',
        category: 'seascapes',
        dimensions: '36x48 inches',
        year: 2021,
        price: 2200,
        isAvailable: false
    },
    {
        id: 'seascape-4',
        title: 'Even Cloudy Days are Better at the Beach',
        description: 'Finding beauty in overcast coastal scenes.',
        imageUrl: '/Seascapes/Even Cloudy Days are Better at the Beach.jpeg',
        category: 'seascapes',
        dimensions: '24x30 inches',
        year: 2023,
        price: 1100,
        isAvailable: true
    },
    {
        id: 'seascape-5',
        title: 'Mangrove Village',
        description: 'A coastal village nestled among mangrove trees.',
        imageUrl: '/Seascapes/Mangrove Village.jpg',
        category: 'seascapes',
        dimensions: '28x36 inches',
        year: 2022,
        price: 1400,
        isAvailable: true
    },
    {
        id: 'seascape-6',
        title: 'Morning Glory',
        description: 'The first light of dawn breaking over the ocean.',
        imageUrl: '/Seascapes/Morning Glory.jpg',
        category: 'seascapes',
        dimensions: '30x40 inches',
        year: 2023,
        price: 1700,
        isAvailable: true
    },
    {
        id: 'seascape-7',
        title: 'Pastel Morning',
        description: 'Soft pastel hues painting the early morning sky.',
        imageUrl: '/Seascapes/Pastel Morning.JPG',
        category: 'seascapes',
        dimensions: '24x32 inches',
        year: 2022,
        price: 1250,
        isAvailable: true
    },
    {
        id: 'seascape-8',
        title: 'Rainbow River',
        description: 'Where the river meets the sea in a colorful embrace.',
        imageUrl: '/Seascapes/Rainbow River.jpg',
        category: 'seascapes',
        dimensions: '26x34 inches',
        year: 2021,
        price: 1350,
        isAvailable: false
    },
    {
        id: 'seascape-9',
        title: 'Rowboat Waiting',
        description: 'A solitary rowboat resting on calm waters.',
        imageUrl: '/Seascapes/Rowboat Waiting.jpg',
        category: 'seascapes',
        dimensions: '20x28 inches',
        year: 2023,
        price: 950,
        isAvailable: true
    },
    {
        id: 'seascape-10',
        title: 'Sailboat',
        description: 'A graceful sailboat cutting through the waves.',
        imageUrl: '/Seascapes/Sailboat.jpg',
        category: 'seascapes',
        dimensions: '24x30 inches',
        year: 2022,
        price: 1150,
        isAvailable: true
    },
    {
        id: 'seascape-11',
        title: 'Sailing Sunset',
        description: 'Silhouetted sails against a dramatic sunset.',
        imageUrl: '/Seascapes/Sailing Sunset.jpg',
        category: 'seascapes',
        dimensions: '32x40 inches',
        year: 2021,
        price: 1800,
        isAvailable: true
    },
    {
        id: 'seascape-12',
        title: 'Seeing Red',
        description: 'Bold red tones capturing the intensity of coastal light.',
        imageUrl: '/Seascapes/Seeing Red.jpg',
        category: 'seascapes',
        dimensions: '24x32 inches',
        year: 2023,
        price: 1200,
        isAvailable: true
    },
    {
        id: 'seascape-13',
        title: 'Shore Scene',
        description: 'A peaceful shoreline with gentle waves lapping at the sand.',
        imageUrl: '/Seascapes/Shore scene.JPG',
        category: 'seascapes',
        dimensions: '20x24 inches',
        year: 2022,
        price: 900,
        isAvailable: false
    },
    {
        id: 'seascape-14',
        title: 'Solitude',
        description: 'The peaceful isolation of being alone with the ocean.',
        imageUrl: '/Seascapes/Solitude.jpg',
        category: 'seascapes',
        dimensions: '28x36 inches',
        year: 2023,
        price: 1450,
        isAvailable: true
    },
    {
        id: 'seascape-15',
        title: 'Sunrise Drama',
        description: 'Dramatic clouds and light at the moments of sunrise.',
        imageUrl: '/Seascapes/SunriseDrama.JPG',
        category: 'seascapes',
        dimensions: '30x40 inches',
        year: 2022,
        price: 1650,
        isAvailable: true
    },
    {
        id: 'seascape-16',
        title: 'Wave Blue',
        description: 'The deep blue of ocean waves in motion.',
        imageUrl: '/Seascapes/Wave Blue.jpg',
        category: 'seascapes',
        dimensions: '24x30 inches',
        year: 2021,
        price: 1100,
        isAvailable: true
    },
    {
        id: 'seascape-17',
        title: 'Wind and Water',
        description: 'The dynamic interplay between wind and water.',
        imageUrl: '/Seascapes/Wind and Water1.JPG',
        category: 'seascapes',
        dimensions: '26x34 inches',
        year: 2023,
        price: 1300,
        isAvailable: true
    },
    // Animals
    {
        id: 'animal-1',
        title: 'Abby\'s Horse',
        description: 'A majestic portrait of Abby\'s beloved horse.',
        imageUrl: '/Animals/Abby\'s Horse.jpg',
        category: 'animals',
        dimensions: '30x40 inches',
        year: 2023,
        price: 1800,
        isAvailable: true
    },
    {
        id: 'animal-2',
        title: 'Angelfish & Purple Fan',
        description: 'Vibrant underwater scene featuring colorful angelfish.',
        imageUrl: '/Animals/Angelfish  & Purple Fan.jpg',
        category: 'animals',
        dimensions: '24x32 inches',
        year: 2022,
        price: 1400,
        isAvailable: true
    },
    {
        id: 'animal-3',
        title: 'Cruising',
        description: 'Marine life cruising through the ocean depths.',
        imageUrl: '/Animals/Cruising.JPG',
        category: 'animals',
        dimensions: '20x28 inches',
        year: 2023,
        price: 1100,
        isAvailable: false
    },
    {
        id: 'animal-4',
        title: 'Dyptych Big Yellow',
        description: 'A striking dyptych featuring bold yellow tones.',
        imageUrl: '/Animals/Dyptych Big Yellow.jpg',
        category: 'animals',
        dimensions: '36x48 inches',
        year: 2021,
        price: 2400,
        isAvailable: true
    },
    {
        id: 'animal-5',
        title: 'Dyptych Ocean Butterflies',
        description: 'Two panels capturing the grace of ocean butterflies.',
        imageUrl: '/Animals/Dyptych Ocean butterflies.jpg',
        category: 'animals',
        dimensions: '40x30 inches',
        year: 2022,
        price: 2200,
        isAvailable: true
    },
    {
        id: 'animal-6',
        title: 'Ever Vigilant',
        description: 'A watchful creature ever alert to its surroundings.',
        imageUrl: '/Animals/Ever Vigilant.jpg',
        category: 'animals',
        dimensions: '24x30 inches',
        year: 2023,
        price: 1250,
        isAvailable: true
    },
    {
        id: 'animal-7',
        title: 'Fairy Wrens',
        description: 'Delicate fairy wrens in their natural habitat.',
        imageUrl: '/Animals/Fairy Wrens.jpg',
        category: 'animals',
        dimensions: '18x24 inches',
        year: 2022,
        price: 850,
        isAvailable: true
    },
    {
        id: 'animal-8',
        title: 'Green Turtle',
        description: 'A serene green turtle gliding through turquoise waters.',
        imageUrl: '/Animals/Green Turtle.JPG',
        category: 'animals',
        dimensions: '28x36 inches',
        year: 2021,
        price: 1600,
        isAvailable: false
    },
    {
        id: 'animal-9',
        title: 'Hiding',
        description: 'A playful creature caught in a moment of hiding.',
        imageUrl: '/Animals/Hiding.JPG',
        category: 'animals',
        dimensions: '20x24 inches',
        year: 2023,
        price: 950,
        isAvailable: true
    },
    {
        id: 'animal-10',
        title: 'Leatherback',
        description: 'The magnificent leatherback turtle in its element.',
        imageUrl: '/Animals/Leatherback .jpg',
        category: 'animals',
        dimensions: '32x40 inches',
        year: 2022,
        price: 1750,
        isAvailable: true
    },
    {
        id: 'animal-11',
        title: 'Manatees',
        description: 'Gentle manatees floating in peaceful waters.',
        imageUrl: '/Animals/Manatees.jpg',
        category: 'animals',
        dimensions: '24x32 inches',
        year: 2023,
        price: 1300,
        isAvailable: true
    },
    {
        id: 'animal-12',
        title: 'Meadow',
        description: 'Wild animals grazing in a sunlit meadow.',
        imageUrl: '/Animals/Meadow.JPG',
        category: 'animals',
        dimensions: '30x40 inches',
        year: 2022,
        price: 1550,
        isAvailable: true
    },
    {
        id: 'animal-13',
        title: 'My Green Visitor',
        description: 'An unexpected green visitor bringing life to the scene.',
        imageUrl: '/Animals/My Green Visitor.jpg',
        category: 'animals',
        dimensions: '20x28 inches',
        year: 2021,
        price: 1050,
        isAvailable: false
    },
    {
        id: 'animal-14',
        title: 'Octopus Purple',
        description: 'A mesmerizing purple octopus in all its tentacled glory.',
        imageUrl: '/Animals/OctopusPurple.jpg',
        category: 'animals',
        dimensions: '24x24 inches',
        year: 2023,
        price: 1150,
        isAvailable: true
    },
    {
        id: 'animal-15',
        title: 'Painted Bunting',
        description: 'The brilliantly colored painted bunting in flight.',
        imageUrl: '/Animals/Painted Bunting .jpg',
        category: 'animals',
        dimensions: '16x20 inches',
        year: 2022,
        price: 750,
        isAvailable: true
    },
    {
        id: 'animal-16',
        title: 'Peekaboo',
        description: 'A playful peekaboo moment with a curious creature.',
        imageUrl: '/Animals/Peekaboo.jpg',
        category: 'animals',
        dimensions: '18x24 inches',
        year: 2023,
        price: 850,
        isAvailable: true
    },
    {
        id: 'animal-17',
        title: 'Sam',
        description: 'A portrait of Sam, a beloved companion.',
        imageUrl: '/Animals/Sam.JPG',
        category: 'animals',
        dimensions: '20x24 inches',
        year: 2022,
        price: 900,
        isAvailable: false
    },
    {
        id: 'animal-18',
        title: 'Snail Kite',
        description: 'The elegant snail kite in its natural wetland habitat.',
        imageUrl: '/Animals/Snail Kite .jpg',
        category: 'animals',
        dimensions: '24x30 inches',
        year: 2021,
        price: 1200,
        isAvailable: true
    },
    {
        id: 'animal-19',
        title: 'Snowy Owl',
        description: 'A majestic snowy owl with piercing gaze.',
        imageUrl: '/Animals/SnowyOwl.jpg',
        category: 'animals',
        dimensions: '28x36 inches',
        year: 2023,
        price: 1650,
        isAvailable: true
    },
    {
        id: 'animal-20',
        title: 'Spiny Lobster',
        description: 'A colorful spiny lobster in its underwater world.',
        imageUrl: '/Animals/Spiny Lobster.jpg',
        category: 'animals',
        dimensions: '20x28 inches',
        year: 2022,
        price: 1050,
        isAvailable: true
    },
    {
        id: 'animal-21',
        title: 'Tigers',
        description: 'The power and grace of tigers captured in paint.',
        imageUrl: '/Animals/Tigers.jpg',
        category: 'animals',
        dimensions: '36x48 inches',
        year: 2021,
        price: 2500,
        isAvailable: true
    },
    {
        id: 'animal-22',
        title: 'To the Light',
        description: 'Creatures drawn to the light in a dramatic scene.',
        imageUrl: '/Animals/To the Light.jpg',
        category: 'animals',
        dimensions: '24x32 inches',
        year: 2023,
        price: 1350,
        isAvailable: false
    },
    {
        id: 'animal-23',
        title: 'What\'s for Dinner',
        description: 'A humorous take on the food chain in nature.',
        imageUrl: '/Animals/What\'s for dinner.jpg',
        category: 'animals',
        dimensions: '22x28 inches',
        year: 2022,
        price: 1100,
        isAvailable: true
    },
    {
        id: 'animal-24',
        title: 'Wild White Majesty',
        description: 'The wild white majesty of nature\'s creatures.',
        imageUrl: '/Animals/Wild White Majesty.jpg',
        category: 'animals',
        dimensions: '30x40 inches',
        year: 2021,
        price: 1800,
        isAvailable: true
    },
    {
        id: 'animal-25',
        title: 'ZsaZsa',
        description: 'A charming portrait of ZsaZsa with personality.',
        imageUrl: '/Animals/ZsaZsa.jpeg',
        category: 'animals',
        dimensions: '18x24 inches',
        year: 2023,
        price: 850,
        isAvailable: true
    },
    // Flowers
    {
        id: 'flower-1',
        title: 'Bird of Paradise',
        description: 'The exotic beauty of the bird of paradise flower.',
        imageUrl: '/Flowers/Bird of Paradise.jpg',
        category: 'flowers',
        dimensions: '24x30 inches',
        year: 2023,
        price: 1200,
        isAvailable: true
    },
    {
        id: 'flower-2',
        title: 'Bumblebee',
        description: 'A busy bumblebee collecting nectar from vibrant blooms.',
        imageUrl: '/Flowers/Bumblebee.jpg',
        category: 'flowers',
        dimensions: '20x24 inches',
        year: 2022,
        price: 950,
        isAvailable: true
    },
    {
        id: 'flower-3',
        title: 'Coneflowers',
        description: 'Rustic coneflowers swaying in a summer breeze.',
        imageUrl: '/Flowers/Coneflowers.jpg',
        category: 'flowers',
        dimensions: '24x32 inches',
        year: 2023,
        price: 1150,
        isAvailable: false
    },
    {
        id: 'flower-4',
        title: 'Daffodils',
        description: 'Cheerful daffodils heralding the arrival of spring.',
        imageUrl: '/Flowers/Daffodils.jpg',
        category: 'flowers',
        dimensions: '20x28 inches',
        year: 2022,
        price: 1000,
        isAvailable: true
    },
    {
        id: 'flower-5',
        title: 'Flowers for Dee',
        description: 'A special floral tribute created for Dee.',
        imageUrl: '/Flowers/Flowers for Dee.jpg',
        category: 'flowers',
        dimensions: '24x30 inches',
        year: 2021,
        price: 1250,
        isAvailable: true
    },
    {
        id: 'flower-6',
        title: 'Honey Bee',
        description: 'A honey bee at work among colorful blossoms.',
        imageUrl: '/Flowers/Honey bee.jpeg',
        category: 'flowers',
        dimensions: '18x24 inches',
        year: 2023,
        price: 850,
        isAvailable: true
    },
    {
        id: 'flower-7',
        title: 'Milkweed Hummingbird',
        description: 'A hummingbird feeding on milkweed nectar.',
        imageUrl: '/Flowers/MilkweedHummingbird.jpg',
        category: 'flowers',
        dimensions: '22x28 inches',
        year: 2022,
        price: 1100,
        isAvailable: true
    },
    {
        id: 'flower-8',
        title: 'Monarch Beginnings',
        description: 'The beginning of a monarch butterfly\'s journey.',
        imageUrl: '/Flowers/Monarch Beginnings.JPG',
        category: 'flowers',
        dimensions: '20x24 inches',
        year: 2023,
        price: 950,
        isAvailable: false
    },
    {
        id: 'flower-9',
        title: 'Royal Poinciana',
        description: 'The regal royal poinciana in full tropical bloom.',
        imageUrl: '/Flowers/Royal Poinciana.jpg',
        category: 'flowers',
        dimensions: '28x36 inches',
        year: 2021,
        price: 1500,
        isAvailable: true
    },
    {
        id: 'flower-10',
        title: 'Squash Blossoms',
        description: 'Golden squash blossoms in a garden setting.',
        imageUrl: '/Flowers/Squash blossoms1.JPG',
        category: 'flowers',
        dimensions: '20x24 inches',
        year: 2022,
        price: 900,
        isAvailable: true
    },
    {
        id: 'flower-11',
        title: 'The Bee',
        description: 'A close-up study of a bee in its natural habitat.',
        imageUrl: '/Flowers/The Bee.jpg',
        category: 'flowers',
        dimensions: '16x20 inches',
        year: 2023,
        price: 750,
        isAvailable: true
    },
    {
        id: 'flower-12',
        title: 'Water Lilies',
        description: 'Peaceful water lilies floating on a tranquil pond.',
        imageUrl: '/Flowers/Water lilies.jpg',
        category: 'flowers',
        dimensions: '24x32 inches',
        year: 2022,
        price: 1200,
        isAvailable: true
    }
];

// Helper function to get painting by ID
export function getPaintingById(id: string): Painting | undefined {
    return paintingsData.find(painting => painting.id === id);
}

// Helper function to get paintings by category
export function getPaintingsByCategory(category: string): Painting[] {
    return paintingsData.filter(painting => painting.category === category);
}

// Helper function to get painting slug from filename
export function getPaintingSlugFromFilename(filename: string): string {
    // Convert filename to slug (e.g., "Aspens.jpg" -> "aspens")
    return filename.replace(/\.[^/.]+$/, "").toLowerCase().replace(/[^a-z0-9]+/g, "-").trim();
}

// Helper function to get painting by filename
export function getPaintingByFilename(filename: string, category: string): Painting | undefined {
    const slug = getPaintingSlugFromFilename(filename);
    return paintingsData.find(painting => {
        const fileSlug = getPaintingSlugFromFilename(painting.imageUrl.split('/').pop() || '');
        return fileSlug === slug && painting.category === category;
    });
}