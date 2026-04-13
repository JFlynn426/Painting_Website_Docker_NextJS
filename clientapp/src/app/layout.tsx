import type { Metadata } from "next";
import localFont from "next/font/local";
import "./globals.css";
import NavBar from "../components/NavBar";
import Footer from "../components/Footer";

const font = localFont({
  src: [
    {
      path: "../../public/fonts/Manjari-Thin.ttf",
    },
  ],
  variable: "--font-local",
  display: "optional",
});

export const metadata: Metadata = {
  title: "Gloria Gronowicz Fine Art",
  description: "Gloria Gronowicz is an oil painter who combines fine art with conservation. As a former Ph.D. scientist she seeks to tell the story of different species and their varied habitats, particularly in South Florida. Through color and light she creates a vignette of life in nature.",
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
