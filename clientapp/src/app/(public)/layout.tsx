import NavBar from "@/components/NavBar";
import Footer from "@/components/Footer";

export default function PublicLayout({
    children,
}: Readonly<{
    children: React.ReactNode;
}>) {
    return (
        <>
            <NavBar />
            <main className="py-4">
                {children}
            </main>
            <Footer />
        </>
    );
}
