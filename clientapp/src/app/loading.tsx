export default function Loading() {
    return (
        <div className="flex flex-col items-center justify-center min-vh-100">
            <div className="animate-spin rounded-full h-60 w-60 border-t-4 border-b-4 border-blue-500"></div>
            <p className="mt-8 text-4xl font-bold text-white">Loading...</p>
        </div>
    );
}