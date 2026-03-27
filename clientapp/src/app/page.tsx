import ArtCarousel from "../components/ArtCarousel";
import { pageContentData } from "./models/pageContent";

export default async function Home() {
  await new Promise((resolve) => setTimeout(resolve, 1000));

  // Get home page content
  const homeContent = pageContentData.find(p => p.address === 'home');

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
        <p className="text-center max-w-2xl mb-8">
          {homeContent.content}
        </p>
      )}
    </div>
  );
}
