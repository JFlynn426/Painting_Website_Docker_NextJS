import NavBar from "@/components/NavBar";
import Footer from "@/components/Footer";

export default function PublicLayout({
    children,
}: Readonly<{
    children: React.ReactNode;
}>) {
    return (
        <div className="flex flex-col min-h-screen">
            <NavBar />
            <main className="py-4 flex-1">
                {children}
            </main>
            <Footer />
        </div>
    );
}
