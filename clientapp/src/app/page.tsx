import ArtCarousel from "../components/ArtCarousel";
import { getPageContent } from "../lib/api";
import { sanitizeHtml } from "../lib/sanitization";

export default async function Home() {
  // Get home page content from API
  const homeContent = await getPageContent('home');

  return (
    <div className="flex flex-col items-center justify-center p-4 sm:p-2 md:p-1 text-[var(--foreground)]">
      {/* Title from page content */}
      {homeContent && (
        <h1 className="text-3xl mb-8 text-center">{homeContent.title}</h1>
      )}

      <div className="mb-8 w-full">
        <ArtCarousel />
      </div>

      {/* Content below carousel */}
      {homeContent && (
        <div className="text-center max-w-2xl mb-8" dangerouslySetInnerHTML={{ __html: sanitizeHtml(homeContent.content) }} />
      )}
    </div>
  );
}
