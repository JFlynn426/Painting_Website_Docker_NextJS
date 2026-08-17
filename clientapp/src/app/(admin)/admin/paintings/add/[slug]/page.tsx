'use client';

import { use, useState, Fragment } from 'react';
import Link from 'next/link';
import { uploadImage, AddPaintingRequest, ImageUploadResult } from '@/lib/api';
import { addPaintingAction } from '@/actions/painting-actions';
import UploadSpinnerModal from '@/components/UploadSpinnerModal';

interface AddPaintingToCategoryPageProps {
    params: Promise<{ slug: string }>;
}

export default function AddPaintingToCategoryPage({ params }: AddPaintingToCategoryPageProps) {
    const { slug } = use(params);
    const artworkLabel = process.env.NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL || "Painting";
    const artworkLabelPlural = process.env.NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL_PLURAL || "Paintings";
    const artworkLabelLower = artworkLabel.toLowerCase();
    const artworkLabelLowerPlural = artworkLabelPlural.toLowerCase();

    const [title, setTitle] = useState('');
    const [description, setDescription] = useState('');
    const [imageUrl, setImageUrl] = useState('');
    const [thumbnailUrl, setThumbnailUrl] = useState('');
    const [price, setPrice] = useState('');
    const [width, setWidth] = useState('');
    const [height, setHeight] = useState('');
    const [depth, setDepth] = useState('');
    const [year, setYear] = useState('');
    const [isAvailable, setIsAvailable] = useState(true);
    const [isNew, setIsNew] = useState(true);
    const [isLandscape, setIsLandscape] = useState(true);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [isUploading, setIsUploading] = useState(false);
    const [submitError, setSubmitError] = useState<string | null>(null);
    const [submitSuccess, setSubmitSuccess] = useState(false);
    const [errors, setErrors] = useState<Record<string, string>>({});

    const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) {
            // Validate file type - only JPG/JPEG allowed
            const validTypes = ['image/jpeg', 'image/jpg'];
            if (!validTypes.includes(file.type)) {
                setErrors((prev: Record<string, string>) => ({ ...prev, file: 'Please select a JPG/JPEG image file' }));
                return;
            }

            // Validate file size (max 20MB)
            if (file.size > 20 * 1024 * 1024) {
                setErrors((prev: Record<string, string>) => ({ ...prev, file: 'File size must be less than 20MB' }));
                return;
            }

            setIsUploading(true);
            setErrors((prev: Record<string, string>) => {
                const next = { ...prev };
                delete next.file;
                return next;
            });

            try {
                // Show local preview while uploading
                const reader = new FileReader();
                reader.onloadend = () => {
                    const result = reader.result as string;
                    setImageUrl(result);
                };
                reader.readAsDataURL(file);

                // Upload to server - isLandscape detected from JPEG dimensions on backend
                const result: ImageUploadResult = await uploadImage(file);
                setImageUrl(result.highResUrl);
                setThumbnailUrl(result.thumbnailUrl);
                setIsLandscape(result.isLandscape);
            } catch (err) {
                const message = err instanceof Error ? err.message : 'Failed to upload image';
                setErrors((prev: Record<string, string>) => ({ ...prev, file: message }));
                setImageUrl('');
                setThumbnailUrl('');
            } finally {
                setIsUploading(false);
            }
        }
    };

    const validate = (): boolean => {
        const newErrors: Record<string, string> = {};

        // Title is required, max 100 chars
        if (!title.trim()) {
            newErrors.title = 'Title is required';
        } else if (title.length > 100) {
            newErrors.title = 'Title must be 100 characters or less';
        }

        // Description max 500 chars
        if (description && description.length > 500) {
            newErrors.description = 'Description must be 500 characters or less';
        }

        // Image URL is required
        if (!imageUrl.trim()) {
            newErrors.imageUrl = 'Image is required (upload a .jpg file)';
        }

        // Price must be >= 0 with max 2 decimal places
        if (price !== '' && price !== undefined) {
            const priceNum = parseFloat(price);
            if (isNaN(priceNum)) {
                newErrors.price = 'Price must be a valid number';
            } else if (priceNum < 0) {
                newErrors.price = 'Price must be 0 or greater';
            } else if (!Number.isFinite(priceNum * 100) || (priceNum * 100) % 1 !== 0) {
                newErrors.price = 'Price can have at most 2 decimal places';
            }
        }

        // Width must be > 0 with max 2 decimal places
        if (width !== '' && width !== undefined) {
            const widthNum = parseFloat(width);
            if (isNaN(widthNum)) {
                newErrors.width = 'Width must be a valid number';
            } else if (widthNum <= 0) {
                newErrors.width = 'Width must be greater than 0';
            } else if ((widthNum * 100) % 1 !== 0) {
                newErrors.width = 'Width can have at most 2 decimal places';
            }
        }

        // Height must be > 0 with max 2 decimal places
        if (height !== '' && height !== undefined) {
            const heightNum = parseFloat(height);
            if (isNaN(heightNum)) {
                newErrors.height = 'Height must be a valid number';
            } else if (heightNum <= 0) {
                newErrors.height = 'Height must be greater than 0';
            } else if ((heightNum * 100) % 1 !== 0) {
                newErrors.height = 'Height can have at most 2 decimal places';
            }
        }

        // Depth must be > 0 with max 2 decimal places
        if (depth !== '' && depth !== undefined) {
            const depthNum = parseFloat(depth);
            if (isNaN(depthNum)) {
                newErrors.depth = 'Depth must be a valid number';
            } else if (depthNum <= 0) {
                newErrors.depth = 'Depth must be greater than 0';
            } else if ((depthNum * 100) % 1 !== 0) {
                newErrors.depth = 'Depth can have at most 2 decimal places';
            }
        }

        // Year must be between 1900 and 2100
        if (year !== '' && year !== undefined) {
            const yearNum = parseInt(year, 10);
            if (isNaN(yearNum)) {
                newErrors.year = 'Year must be a valid number';
            } else if (yearNum < 1900 || yearNum > 2100) {
                newErrors.year = 'Year must be between 1900 and 2100';
            }
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setSubmitError(null);
        setSubmitSuccess(false);

        if (!validate()) {
            return;
        }

        setIsSubmitting(true);

        const request: AddPaintingRequest = {
            title: title.trim(),
            description: description.trim() || undefined,
            imageUrl: imageUrl.trim(),
            thumbnailUrl: thumbnailUrl.trim() || undefined,
            categorySlug: slug,
            price: price !== '' ? parseFloat(price) : undefined,
            width: width !== '' ? parseFloat(width) : undefined,
            height: height !== '' ? parseFloat(height) : undefined,
            depth: depth !== '' ? parseFloat(depth) : undefined,
            year: year !== '' ? parseInt(year, 10) : undefined,
            isAvailable,
            isNew,
            isLandscape,
        };

        try {
            await addPaintingAction(request);
            setSubmitSuccess(true);
            window.scrollTo(0, 0);
            // Reset form
            setTitle('');
            setDescription('');
            setImageUrl('');
            setThumbnailUrl('');
            setPrice('');
            setWidth('');
            setHeight('');
            setDepth('');
            setYear('');
            setIsAvailable(true);
            setIsNew(true);
        } catch (err) {
            setSubmitError(err instanceof Error ? err.message : `Failed to add ${artworkLabelLower}`);
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <Fragment>
            <div>
                <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">
                    Add {artworkLabel} to: {slug.split('-').map(word => word.charAt(0).toUpperCase() + word.slice(1)).join(' ')}
                </h1>

                <div className="bg-yellow-100 border border-yellow-600 rounded-lg p-4 mb-6">
                    <p className="text-black text-sm">
                        <strong>Note:</strong> Required fields are marked with a red asterisk. Attempt to fill out the same fields for all {artworkLabelLowerPlural}, where possible, to maintain consistency across the site.
                    </p>
                </div>

                {submitSuccess && (
                    <div className="bg-green-200 border border-green-500 rounded-lg p-4 mb-6">
                        <p className="text-black">{artworkLabel} added successfully!</p>
                    </div>
                )}

                {submitError && (
                    <div className="bg-red-200 border border-red-500 rounded-lg p-4 mb-6">
                        <p className="text-black">Error: {submitError}</p>
                    </div>
                )}

                {errors.file && (
                    <div className="bg-red-200 border border-red-500 rounded-lg p-4 mb-6">
                        <p className="text-black">{errors.file}</p>
                    </div>
                )}

                <form onSubmit={handleSubmit} className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 space-y-4">
                    {/* Image Upload */}
                    <div>
                        <label className="block text-sm font-medium text-[var(--foreground)] mb-2">
                            {artworkLabel} Image (.jpg) <span className="text-red-500">*</span>
                        </label>
                        <input
                            type="file"
                            accept=".jpg,.jpeg"
                            onChange={handleFileUpload}
                            disabled={isUploading}
                            className="w-full text-[var(--foreground)] file:mr-4 file:py-2 file:px-4 file:rounded file:border-0 file:text-sm file:font-semibold file:bg-[var(--button-color)] file:text-white hover:file:opacity-90 disabled:opacity-50"
                        />
                        {isUploading && <p className="text-blue-400 text-sm mt-1">Uploading and processing image...</p>}
                        {errors.file && <p className="text-red-500 text-sm mt-1">{errors.file}</p>}
                        {imageUrl && (
                            <div className="mt-2">
                                <img src={imageUrl} alt="Preview" className="max-w-xs rounded" />
                            </div>
                        )}
                    </div>

                    {/* Title */}
                    <div>
                        <label htmlFor="title" className="block text-sm font-medium text-[var(--foreground)] mb-1">
                            Title <span className="text-red-500">*</span>
                        </label>
                        <input
                            type="text"
                            id="title"
                            value={title}
                            onChange={e => setTitle(e.target.value)}
                            maxLength={100}
                            className="w-full px-3 py-2 bg-[var(--background)] text-[var(--foreground)] border border-gray-600 rounded focus:outline-none focus:border-blue-500"
                            placeholder={`Enter ${artworkLabelLower} title (Required)`}
                        />
                        <p className="text-gray-500 text-xs mt-1">{title.length}/100 characters</p>
                        {errors.title && <p className="text-red-500 text-sm mt-1">{errors.title}</p>}
                    </div>

                    {/* Description */}
                    <div>
                        <label htmlFor="description" className="block text-sm font-medium text-[var(--foreground)] mb-1">
                            Description
                        </label>
                        <textarea
                            id="description"
                            value={description}
                            onChange={e => setDescription(e.target.value)}
                            maxLength={500}
                            rows={4}
                            className="w-full px-3 py-2 bg-[var(--background)] text-[var(--foreground)] border border-gray-600 rounded focus:outline-none focus:border-blue-500"
                            placeholder={`Enter ${artworkLabelLower} description (Suggested)`}
                        />
                        <p className="text-gray-500 text-xs mt-1">{description.length}/500 characters</p>
                        {errors.description && <p className="text-red-500 text-sm mt-1">{errors.description}</p>}
                    </div>

                    {/* Dimensions */}
                    <div className="grid grid-cols-3 gap-4">
                        <div>
                            <label htmlFor="width" className="block text-sm font-medium text-[var(--foreground)] mb-1">
                                Width
                            </label>
                            <input
                                type="number"
                                id="width"
                                value={width}
                                onChange={e => setWidth(e.target.value)}
                                step="0.01"
                                min="0.01"
                                className="w-full px-3 py-2 bg-[var(--background)] text-[var(--foreground)] border border-gray-600 rounded focus:outline-none focus:border-blue-500 [appearance:textfield] [&::-webkit-outer-spin-button]:appearance-none [&::-webkit-inner-spin-button]:appearance-none"
                                placeholder="Width (Suggested)"
                            />
                            {errors.width && <p className="text-red-500 text-sm mt-1">{errors.width}</p>}
                        </div>
                        <div>
                            <label htmlFor="height" className="block text-sm font-medium text-[var(--foreground)] mb-1">
                                Height
                            </label>
                            <input
                                type="number"
                                id="height"
                                value={height}
                                onChange={e => setHeight(e.target.value)}
                                step="0.01"
                                min="0.01"
                                className="w-full px-3 py-2 bg-[var(--background)] text-[var(--foreground)] border border-gray-600 rounded focus:outline-none focus:border-blue-500 [appearance:textfield] [&::-webkit-outer-spin-button]:appearance-none [&::-webkit-inner-spin-button]:appearance-none"
                                placeholder="Height (Suggested)"
                            />
                            {errors.height && <p className="text-red-500 text-sm mt-1">{errors.height}</p>}
                        </div>
                        <div>
                            <label htmlFor="depth" className="block text-sm font-medium text-[var(--foreground)] mb-1">
                                Depth
                            </label>
                            <input
                                type="number"
                                id="depth"
                                value={depth}
                                onChange={e => setDepth(e.target.value)}
                                step="0.01"
                                min="0.01"
                                className="w-full px-3 py-2 bg-[var(--background)] text-[var(--foreground)] border border-gray-600 rounded focus:outline-none focus:border-blue-500 [appearance:textfield] [&::-webkit-outer-spin-button]:appearance-none [&::-webkit-inner-spin-button]:appearance-none"
                                placeholder="Depth (Optional)"
                            />
                            {errors.depth && <p className="text-red-500 text-sm mt-1">{errors.depth}</p>}
                        </div>
                    </div>

                    {/* Year and Price */}
                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <label htmlFor="year" className="block text-sm font-medium text-[var(--foreground)] mb-1">
                                Year
                            </label>
                            <input
                                type="number"
                                id="year"
                                value={year}
                                onChange={e => setYear(e.target.value)}
                                min={1900}
                                max={2100}
                                className="w-full px-3 py-2 bg-[var(--background)] text-[var(--foreground)] border border-gray-600 rounded focus:outline-none focus:border-blue-500 [appearance:textfield] [&::-webkit-outer-spin-button]:appearance-none [&::-webkit-inner-spin-button]:appearance-none"
                                placeholder="Year created (Optional)"
                            />
                            {errors.year && <p className="text-red-500 text-sm mt-1">{errors.year}</p>}
                        </div>
                        <div>
                            <label htmlFor="price" className="block text-sm font-medium text-[var(--foreground)] mb-1">
                                Price ($)
                            </label>
                            <input
                                type="number"
                                id="price"
                                value={price}
                                onChange={e => setPrice(e.target.value)}
                                step="0.01"
                                min="0"
                                className="w-full px-3 py-2 bg-[var(--background)] text-[var(--foreground)] border border-gray-600 rounded focus:outline-none focus:border-blue-500 [appearance:textfield] [&::-webkit-outer-spin-button]:appearance-none [&::-webkit-inner-spin-button]:appearance-none"
                                placeholder={`Price (Suggested for available ${artworkLabelLower})`}
                            />
                            {errors.price && <p className="text-red-500 text-sm mt-1">{errors.price}</p>}
                        </div>
                    </div>

                    {/* Is Available */}
                    <div className="flex items-center">
                        <input
                            type="checkbox"
                            id="isAvailable"
                            checked={isAvailable}
                            onChange={e => setIsAvailable(e.target.checked)}
                            className="w-4 h-4 text-blue-600 bg-[var(--background)] border-gray-600 rounded focus:ring-blue-500"
                        />
                        <label htmlFor="isAvailable" className="ml-2 block text-sm font-medium text-[var(--foreground)]">
                            {artworkLabel} is available
                        </label>
                    </div>

                    {/* Add to New Paintings */}
                    <div className="flex items-center">
                        <input
                            type="checkbox"
                            id="isNew"
                            checked={isNew}
                            onChange={e => setIsNew(e.target.checked)}
                            className="w-4 h-4 text-blue-600 bg-[var(--background)] border-gray-600 rounded focus:ring-blue-500"
                        />
                        <label htmlFor="isNew" className="ml-2 block text-sm font-medium text-[var(--foreground)]">
                            Add to New {artworkLabelPlural}
                        </label>
                    </div>

                    {/* Submit */}
                    <div className="flex gap-4 pt-4">
                        <button
                            type="submit"
                            disabled={isSubmitting}
                            className="px-6 py-2 bg-[var(--button-color)] text-white rounded hover:opacity-90 transition-opacity disabled:opacity-50"
                        >
                            {isSubmitting ? 'Adding...' : `Add ${artworkLabel}`}
                        </button>
                        <button
                            type="button"
                            onClick={() => { window.scrollTo(0, 0); window.location.href = '/admin/paintings/add'; }}
                            className="px-6 py-2 bg-gray-600 text-white rounded hover:bg-gray-700 transition-colors text-center"
                        >
                            Cancel
                        </button>
                    </div>
                </form>

                <div className="mt-4">
                    <Link href="/admin/paintings/add" className="text-[var(--title-color)] hover:underline">
                        &larr; Back to Categories
                    </Link>
                </div>
            </div>
            <UploadSpinnerModal isVisible={isUploading} />
        </Fragment>
    );
}
