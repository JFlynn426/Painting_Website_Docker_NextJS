"use client";

import { paintingCategories } from "@/app/models/paintingCategories";
import Image from "next/image";
import { use } from "react";
import styles from "./page.module.css";

interface CategoryPageProps {
    params: Promise<{
        category: string;
    }>;
}

export default function CategoryPage({ params }: CategoryPageProps) {
    const { category } = use(params);

    // Find the category from the model
    const categoryData = paintingCategories.find(cat => cat.slug === category);

    // Get images from the corresponding folder
    const folderName = getFolderNameFromSlug(category);
    const images = getImagesForCategory(folderName);

    if (!categoryData) {
        return (
            <div className="container">
                <h1>Category Not Found</h1>
            </div>
        );
    }

    return (
        <div className={styles.container}>
            <h1 className={styles.categoryTitle}>{categoryData.name}</h1>
            {categoryData.description && (
                <p className={styles.description}>{categoryData.description}</p>
            )}

            {images.length > 0 ? (
                <div className={styles.masonryGrid}>
                    {images.map((image, index) => (
                        <div key={index} className={styles.imageWrapper}>
                            <Image
                                src={`/${folderName}/${image}`}
                                alt={`${categoryData.name} - ${image}`}
                                width={400}
                                height={400}
                                className={styles.paintingImage}
                                priority={index < 3}
                            />
                        </div>
                    ))}
                </div>
            ) : (
                <p className={styles.noImages}>No images available for this category.</p>
            )}
        </div>
    );
}

// Map slugs to folder names in public directory
function getFolderNameFromSlug(slug: string): string {
    const slugToFolderMap: Record<string, string> = {
        'landscapes': 'Landscapes and Cityscapes',
        'seascapes': 'Seascapes',
        'animals': 'Animals',
        'flowers': 'Flowers',
    };
    return slugToFolderMap[slug] || slug;
}

// Get list of images for a category folder
function getImagesForCategory(folderName: string): string[] {
    // Define images for each category based on the public folder structure
    const imagesByCategory: Record<string, string[]> = {
        'Landscapes and Cityscapes': [
            'Aspens.jpg',
            'Bahia Honda .jpg',
            'BakedGoods.JPG',
            'Banyans.jpg',
            'Brady & Duke.jpg',
            'Bridge world.jpg',
            'Castelvetore .JPG',
            'Cobblestone Way.JPG',
            'Forest Mystery.jpg',
            'Hydrangea Time.jpeg',
            'Italian Church.jpg',
            'Mine (1).jpg',
            'New Inhabitants.jpeg',
            'Rainbow River1.jpg',
            'Sedona.JPG',
            'The Path.jpg',
            'The Pond.jpg',
            'Window Jigsaw.jpg',
        ],
        'Seascapes': [
            'Cloud Creatures2.jpg',
            'Contemplation.jpeg',
            'Double Roll.jpg',
            'Even Cloudy Days are Better at the Beach.jpeg',
            'Mangrove Village.jpg',
            'Morning Glory.jpg',
            'Pastel Morning.JPG',
            'Rainbow River.jpg',
            'Rowboat Waiting.jpg',
            'Sailboat.jpg',
            'Sailing Sunset.jpg',
            'Seeing Red.jpg',
            'Shore scene.JPG',
            'Solitude.jpg',
            'SunriseDrama.JPG',
            'Wave Blue.jpg',
            'Wind and Water1.JPG',
        ],
        'Animals': [
            "Abby's Horse.jpg",
            'Angelfish  & Purple Fan.jpg',
            'Cruising.JPG',
            'Dyptych Big Yellow.jpg',
            'Dyptych Ocean butterflies.jpg',
            'Ever Vigilant.jpg',
            'Fairy Wrens.jpg',
            'Green Turtle.JPG',
            'Hiding.JPG',
            'Leatherback .jpg',
            'Manatees.jpg',
            'Meadow.JPG',
            'My Green Visitor.jpg',
            'OctopusPurple.jpg',
            'Painted Bunting .jpg',
            'Peekaboo.jpg',
            'Sam.JPG',
            'Snail Kite .jpg',
            'SnowyOwl.jpg',
            'Spiny Lobster.jpg',
            'Tigers.jpg',
            "To the Light.jpg",
            "What's for dinner.jpg",
            'Wild White Majesty.jpg',
            'ZsaZsa.jpeg',
        ],
        'Flowers': [
            'Bird of Paradise.jpg',
            'Bumblebee.jpg',
            'Coneflowers.jpg',
            'Daffodils.jpg',
            'Flowers for Dee.jpg',
            'Honey bee.jpeg',
            'MilkweedHummingbird.jpg',
            'Monarch Beginnings.JPG',
            'Royal Poinciana.jpg',
            'Squash blossoms1.JPG',
            'The Bee.jpg',
            'Water lilies.jpg',
        ],
    };

    return imagesByCategory[folderName] || [];
}