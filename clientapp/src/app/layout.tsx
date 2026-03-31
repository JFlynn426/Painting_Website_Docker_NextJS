import type { Metadata } from "next";
import localFont from "next/font/local";
import "./globals.css";
import NavBar from "../components/NavBar";
import Footer from "../components/Footer";

const font = localFont({
  src: [
    {
      path: "../../public/fonts/Manjari-Bold.ttf",
    },
  ],
  variable: "--font-local",
  display: "swap",
});

export const metadata: Metadata = {
  title: "Gloria Gronowicz Fine Art",
  description: "Gloria Gronowicz is a talented artist known for her captivating paintings that blend realism with a touch of surrealism. Her work often features vibrant colors and intricate details, drawing viewers into a world of imagination and emotion. With a unique style that combines traditional techniques with contemporary themes, Gloria's art has garnered acclaim for its ability to evoke deep feelings and spark thoughtful reflection. Whether depicting serene landscapes or thought-provoking portraits, Gloria Gronowicz's paintings are a testament to her artistic vision and skill.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className={`${font.variable} bg-[var(--background)] text-[var(--foreground)]`}>
        <NavBar />
        <main className="py-4">
          {children}
        </main>
        <Footer />
      </body>
    </html>
  );
}
