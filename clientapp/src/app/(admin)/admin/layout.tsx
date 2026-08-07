import AdminHeader from "@/components/AdminHeader";
import AdminFooter from "@/components/AdminFooter";
import AuthGuard from "@/components/AuthGuard";

export default function AdminLayout({
    children,
}: Readonly<{
    children: React.ReactNode;
}>) {
    return (
        <div className="min-h-screen flex flex-col">
            <AdminHeader />
            <main className="flex-1 container mx-auto p-6">
                <AuthGuard>
                    {children}
                </AuthGuard>
            </main>
            <AdminFooter />
        </div>
    );
}
