import { useState } from "react";

const NAV_ITEMS = [
  { key: "companies", label: "Companies" },
  { key: "brands", label: "Brands" },
  { key: "categories", label: "Categories" },
  { key: "products", label: "Products" },
  { key: "product-presentations", label: "Product Presentations" },
  { key: "menus",    label: "Menus"    },
  { key: "customers", label: "Customers" },
  { key: "roles", label: "Roles" },
  { key: "users", label: "Users" },
  { key: "messages", label: "Messages" },
];

export default function Navbar({ page, onNavigate, onLogout }) {
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  function handleNavigate(nextPage) {
    onNavigate(nextPage);
    setMobileMenuOpen(false);
  }

  function handleLogout() {
    onLogout();
    setMobileMenuOpen(false);
  }

  return (
    <header className="bg-blue-700 text-white px-3 sm:px-6 py-2 sm:py-0 shadow-md">
      <div className="flex items-center justify-between gap-3">
        {/* Brand */}
        <span className="text-base sm:text-lg font-bold tracking-widest py-2 sm:py-4 mr-2 sm:mr-8 select-none whitespace-nowrap">
          RetailBase Web
        </span>

        {/* Nav links (desktop) */}
        <nav className="hidden md:flex items-stretch flex-1 gap-1">
          {NAV_ITEMS.map(({ key, label }) => (
            <button
              key={key}
              onClick={() => onNavigate(key)}
              className={`px-5 py-4 text-sm font-semibold transition-colors border-b-2 ${
                page === key
                  ? "border-white text-white"
                  : "border-transparent text-blue-200 hover:text-white hover:border-blue-300"
              }`}
            >
              {label}
            </button>
          ))}
        </nav>

        {/* Hamburger (mobile) */}
        <button
          type="button"
          onClick={() => setMobileMenuOpen((open) => !open)}
          aria-label="Abrir menú"
          aria-expanded={mobileMenuOpen}
          className="md:hidden inline-flex items-center justify-center w-10 h-10 rounded border border-blue-300/60 bg-blue-600/30 hover:bg-blue-600/50 transition-colors"
        >
          <span className="sr-only">Menú</span>
          <svg className="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            {mobileMenuOpen ? (
              <path d="M18 6 6 18M6 6l12 12" />
            ) : (
              <path d="M3 6h18M3 12h18M3 18h18" />
            )}
          </svg>
        </button>

        {/* Logout (desktop) */}
        <button
          onClick={onLogout}
          className="hidden md:inline-flex bg-white text-blue-700 px-3 sm:px-4 py-1.5 rounded font-semibold hover:bg-blue-50 transition-colors text-xs sm:text-sm"
        >
          Log Out
        </button>
      </div>

      {mobileMenuOpen && (
        <div className="md:hidden mt-2 rounded-lg border border-blue-300/40 bg-blue-800/70 backdrop-blur-sm overflow-hidden">
          <nav className="py-1">
            {NAV_ITEMS.map(({ key, label }) => (
              <button
                key={key}
                onClick={() => handleNavigate(key)}
                className={`w-full text-left px-4 py-3 text-sm font-semibold transition-colors ${
                  page === key
                    ? "bg-white text-blue-700"
                    : "text-blue-100 hover:bg-blue-600"
                }`}
              >
                {label}
              </button>
            ))}
          </nav>

          <div className="border-t border-blue-300/40 p-2">
            <button
              onClick={handleLogout}
              className="w-full bg-white text-blue-700 px-4 py-2 rounded font-semibold hover:bg-blue-50 transition-colors text-sm"
            >
              Log Out
            </button>
          </div>
        </div>
      )}
    </header>
  );
}
