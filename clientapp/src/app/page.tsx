
import Image from 'next/image';

export default async function Home() {
  await new Promise((resolve) => setTimeout(resolve, 1000));

  return (
    <div className="d-flex flex-column align-items-center justify-content-center p-4 p-sm-2 p-md-1 min-vh-100">
      <h1 className="text-3xl font-bold mb-8 text-center text-light">Welcome to My Art Gallery</h1>

      <div className="mb-8 d-flex justify-content-center w-100">
        <Image
          src="/Turtle_Painting.jpg"
          alt="Turtle Painting"
          width={800}
          height={600}
          className="rounded-lg shadow-xl w-100 h-auto object-contain"
          style={{ maxHeight: "75vh", objectFit: "contain" }}
          priority={true}
        />
      </div>
    </div>
  );
}


