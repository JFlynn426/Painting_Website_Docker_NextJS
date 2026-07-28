import type { Metadata } from "next";
import localFont from "next/font/local";
import "./globals.css";

const font = localFont({
  src: [
    {
      path: "../../public/fonts/Manjari-Thin.ttf",
    },
  ],
  variable: "--font-local",
  display: "swap",
});

// Site configuration from environment variables (build-time)
const siteName = process.env.NEXT_PUBLIC_SITE_NAME || "Gloria Gronowicz Fine Art";
const siteDescription = process.env.NEXT_PUBLIC_SITE_DESCRIPTION || "Gloria Gronowicz is an oil painter who combines fine art with conservation. As a former Ph.D. scientist she seeks to tell the story of different species and their varied habitats, particularly in South Florida. Through color and light she creates a vignette of life in nature.";

// CSS theme configuration from environment variables (build-time)
const cssBackground = process.env.NEXT_PUBLIC_CSS_BACKGROUND || "#3d3d3d";
const cssForeground = process.env.NEXT_PUBLIC_CSS_FOREGROUND || "#ffffff";
const cssNavbarFooterBg = process.env.NEXT_PUBLIC_CSS_NAVBAR_FOOTER_BG || "#2d2d2d";
const cssTitleColor = process.env.NEXT_PUBLIC_CSS_TITLE_COLOR || "#66b3ff";
const cssButtonColor = process.env.NEXT_PUBLIC_CSS_BUTTON_COLOR || "#1e3a8a";

export const metadata: Metadata = {
  title: siteName,
  description: siteDescription,
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className={`${font.variable} bg-[var(--background)] text-[var(--foreground)]`}>
        <style dangerouslySetInnerHTML={{
          __html: `
            /* Override CSS variables from environment configuration */
            :root {
              --background: ${cssBackground};
              --foreground: ${cssForeground};
              --navbar-footer-bg: ${cssNavbarFooterBg};
              --title-color: ${cssTitleColor};
              --button-color: ${cssButtonColor};
            }
            @media (prefers-color-scheme: dark) {
              :root {
                --background: ${cssBackground};
                --foreground: ${cssForeground};
                --navbar-footer-bg: ${cssNavbarFooterBg};
                --title-color: ${cssTitleColor};
                --button-color: ${cssButtonColor};
              }
            }
            strong, b {
              -webkit-text-stroke: 0.25px var(--foreground);
            }
          `
        }} />
        {children}
      </body>
    </html>
  );
}
