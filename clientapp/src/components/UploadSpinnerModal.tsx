'use client';

interface UploadSpinnerModalProps {
    isVisible: boolean;
}

/**
 * Modal overlay that displays a spinner while images are uploading.
 * Freezes the entire page to prevent user interaction during upload.
 */
export default function UploadSpinnerModal({ isVisible }: UploadSpinnerModalProps) {
    if (!isVisible) return null;

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50 backdrop-blur-sm">
            <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-8 flex flex-col items-center gap-4 shadow-xl">
                {/* Spinner */}
                <div className="w-12 h-12 border-4 border-blue-600 border-t-transparent rounded-full animate-spin" />
                <p className="text-gray-200 text-lg font-medium">Uploading image...</p>
            </div>
        </div>
    );
}
