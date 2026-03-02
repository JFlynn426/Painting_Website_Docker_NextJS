import ArtCarousel from "../components/ArtCarousel";

export default async function Home() {
  await new Promise((resolve) => setTimeout(resolve, 1000));

  return (
    <div className="flex flex-col items-center justify-center p-4 sm:p-2 md:p-1 text-[var(--foreground)]">
      <h1 className="text-3xl mb-8 text-center">Welcome to My Art Gallery</h1>

      <div className="mb-8 w-full">
        <ArtCarousel />
      </div>
    </div>
  );
}


