import { useState } from "react";

const NAV_ITEMS = [
  { key: "companies", label: "Companies" },
  { key: "brands", label: "Brands" },
  { key: "categories", label: "Categories" },
  { key: "products", label: "Products" },
  { key: "product-presentations", label: "Product Presentations" },
  { key: "menus", label: "Menus" },
  { key: "customers", label: "Customers" },
  { key: "roles", label: "Roles" },
  { key: "users", label: "Users" },
  { key: "messages", label: "Messages" },
];

export default function Navbar({ page, onNavigate, onLogout }) {
  const [open, setOpen] = useState(false);

  return (
    <>
      {/* Mobile toggle button */}
      <button
        type="button"
        onClick={() => setOpen((o) => !o)}
        aria-label="Toggle sidebar"
        aria-expanded={open}
        className="md:hidden fixed top-3 left-3 z-50 inline-flex items-center justify-center w-10 h-10 rounded bg-green-700 text-white shadow-lg"
      >
        <svg className="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
          {open ? (
            <path d="M18 6 6 18M6 6l12 12" />
          ) : (
            <path d="M3 6h18M3 12h18M3 18h18" />
          )}
        </svg>
      </button>

      {/* Backdrop (mobile) */}
      {open && (
        <div
          className="md:hidden fixed inset-0 z-30 bg-black/40"
          onClick={() => setOpen(false)}
        />
      )}

      {/* Sidebar */}
      <aside
        className={`
          fixed top-0 left-0 z-40 h-full w-60 bg-green-700 text-white flex flex-col shadow-lg
          transition-transform duration-200
          ${open ? "translate-x-0" : "-translate-x-full"}
          md:sticky md:top-0 md:h-screen md:translate-x-0 md:flex-shrink-0
        `}
      >
        {/* Brand */}
        <div className="px-5 py-5 text-lg font-bold tracking-widest select-none border-b border-green-600 whitespace-nowrap">
          RetailBase Web
        </div>

        {/* Nav items */}
        <nav className="flex-1 py-3 overflow-y-auto">
          {NAV_ITEMS.map(({ key, label }) => (
            <button
              key={key}
              onClick={() => { onNavigate(key); setOpen(false); }}
              className={`w-full text-left px-5 py-2.5 text-sm font-semibold transition-colors border-l-4 ${
                page === key
                  ? "border-white bg-green-800 text-white"
                  : "border-transparent text-green-200 hover:text-white hover:bg-green-600"
              }`}
            >
              {label}
            </button>
          ))}
        </nav>

        {/* Logout */}
        <div className="p-4 border-t border-green-600">
          <button
            onClick={() => { onLogout(); setOpen(false); }}
            className="w-full bg-white text-green-700 px-4 py-2 rounded font-semibold hover:bg-green-50 transition-colors text-sm"
          >
            Log Out
          </button>
        </div>
      </aside>
    </>
  );
}
