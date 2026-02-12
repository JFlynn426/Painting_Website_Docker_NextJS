import ArtCarousel from "../components/ArtCarousel";

export default async function Home() {
  await new Promise((resolve) => setTimeout(resolve, 1000));

  return (
    <div className="d-flex flex-column align-items-center justify-content-center p-4 p-sm-2 p-md-1">
      <h1 className="text-3xl mb-8 text-center text-light">Welcome to My Art Gallery</h1>

      <div className="mb-8 w-100">
        <ArtCarousel />
      </div>
    </div>
  );
}


