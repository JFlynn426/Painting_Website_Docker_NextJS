import ArtCarousel from "../../components/ArtCarousel";
import { getPageContent } from "../../lib/api";
import { renderParagraphs } from "../../lib/sanitization";
export const dynamic = "force-dynamic";

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

      {/* Content below carousel with consistent styling */}
      {homeContent && (
        <div className="max-w-[50rem] mb-8" style={{ textAlign: 'left', maxWidth: '800px', margin: '0 auto', marginBottom: '3rem' }}>
          <div dangerouslySetInnerHTML={{ __html: renderParagraphs(homeContent.content) }} />
        </div>
      )}
    </div>
  );
}
