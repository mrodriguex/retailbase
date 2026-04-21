import { useState, useEffect, useCallback } from "react";
import * as menuService from "../services/menuService";

const PAGE_SIZE_OPTIONS = [5, 10, 20, 50];

const emptyForm = {
  id: 0,
  name: "",
  abreviation: "",
  descripcion: "",
  imagen: "",
  ruta: "",
  claveMenuPadre: "",
  order: 0,
  enabled: true,
};

export default function Menus() {
  const [menus, setMenus] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  // Pagination
  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);

  // Filters
  const [filterEnabled, setFilterEnabled] = useState(undefined);

  // Add / Edit modal
  const [modal, setModal] = useState({ open: false, mode: "add", data: { ...emptyForm } });
  const [saving, setSaving] = useState(false);
  const [modalError, setModalError] = useState("");

  // Delete confirmation
  const [deleteTarget, setDeleteTarget] = useState(null);
  const [deleting, setDeleting] = useState(false);

  // ── Data loading ──────────────────────────────────────────────────────────
  const loadMenus = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const result = await menuService.getAll({
        enabled: filterEnabled,
        pageIndex,
        pageSize,
      });

      if (Array.isArray(result)) {
        setMenus(result);
        setTotal(result.length);
      } else if (result?.items) {
        setMenus(result.items);
        setTotal(result.total ?? result.items.length);
      } else {
        setMenus([]);
        setTotal(0);
      }
    } catch (e) {
      setError(e.message || "Error al cargar los menús.");
    } finally {
      setLoading(false);
    }
  }, [pageIndex, pageSize, filterEnabled]);

  useEffect(() => {
    loadMenus();
  }, [loadMenus]);

  // ── Modal helpers ─────────────────────────────────────────────────────────
  function openAdd() {
    setModal({ open: true, mode: "add", data: { ...emptyForm } });
    setModalError("");
  }

  function openEdit(menu) {
    setModal({ open: true, mode: "edit", data: { ...menu, claveMenuPadre: menu.claveMenuPadre ?? "" } });
    setModalError("");
  }

  function closeModal() {
    setModal((m) => ({ ...m, open: false }));
  }

  function handleFormChange(e) {
    const { name, value, type, checked } = e.target;
    setModal((m) => ({
      ...m,
      data: {
        ...m.data,
        [name]: type === "checkbox" ? checked : type === "number" ? (value === "" ? "" : Number(value)) : value,
      },
    }));
  }

  async function handleSave(e) {
    e.preventDefault();
    setModalError("");
    setSaving(true);
    try {
      const payload = {
        ...modal.data,
        claveMenuPadre: modal.data.claveMenuPadre === "" ? null : Number(modal.data.claveMenuPadre),
      };
      if (modal.mode === "add") {
        await menuService.add(payload);
      } else {
        await menuService.update(payload);
      }
      closeModal();
      loadMenus();
    } catch (e) {
      setModalError(e.message || "Error al guardar el menú.");
    } finally {
      setSaving(false);
    }
  }

  // ── Delete helpers ────────────────────────────────────────────────────────
  async function handleDelete() {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await menuService.remove(deleteTarget.id);
      setDeleteTarget(null);
      if (menus.length === 1 && pageIndex > 1) setPageIndex((p) => p - 1);
      else loadMenus();
    } catch (e) {
      setError(e.message || "Error al eliminar el menú.");
      setDeleteTarget(null);
    } finally {
      setDeleting(false);
    }
  }

  const totalPages = Math.max(1, Math.ceil(total / pageSize));

  // ── Render ────────────────────────────────────────────────────────────────
  return (
    <main className="max-w-7xl mx-auto p-3 sm:p-4 md:p-6">
      {/* ── Toolbar ── */}
      <div className="flex flex-wrap gap-2 sm:gap-3 items-center mb-5">
        <button
          onClick={openAdd}
          className="w-full sm:w-auto bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded font-semibold transition-colors text-sm"
        >
          + New Menú
        </button>

        <select
          value={filterEnabled === undefined ? "" : String(filterEnabled)}
          onChange={(e) => {
            setFilterEnabled(e.target.value === "" ? undefined : e.target.value === "true");
            setPageIndex(1);
          }}
          className="w-full sm:w-auto border border-gray-300 rounded p-2 text-sm bg-white"
        >
          <option value="">All</option>
          <option value="true">Enabled</option>
          <option value="false">Disabled</option>
        </select>

        <select
          value={pageSize}
          onChange={(e) => { setPageSize(Number(e.target.value)); setPageIndex(1); }}
          className="w-full sm:w-auto border border-gray-300 rounded p-2 text-sm bg-white"
        >
          {PAGE_SIZE_OPTIONS.map((n) => (
            <option key={n} value={n}>{n} per page</option>
          ))}
        </select>

        <button
          onClick={loadMenus}
          disabled={loading}
          className="w-full sm:w-auto sm:ml-auto border border-gray-300 bg-white hover:bg-gray-50 disabled:opacity-50 px-4 py-2 rounded text-sm transition-colors"
        >
          ↻ Update
        </button>
      </div>

      {/* ── Error banner ── */}
      {error && (
        <div className="mb-4 p-3 bg-red-100 border border-red-400 text-red-700 rounded text-sm flex justify-between">
          <span>{error}</span>
          <button onClick={() => setError("")} className="font-bold ml-4">×</button>
        </div>
      )}

      {/* ── Table ── */}
      <div className="bg-white rounded-lg shadow overflow-x-auto">
        <table className="w-full text-sm">
          <thead className="bg-gray-50 border-b">
            <tr>
              {["ID", "Name", "Abreviation", "Ruta", "Imagen", "Menú Padre", "Order", "Estatus", "Actions"].map((h) => (
                <th key={h} className="px-4 py-3 text-left font-semibold text-gray-600 whitespace-nowrap">
                  {h}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr>
                <td colSpan={9} className="text-center py-12 text-gray-400">
                  <span className="inline-block animate-pulse">Loading...</span>
                </td>
              </tr>
            ) : menus.length === 0 ? (
              <tr>
                <td colSpan={9} className="text-center py-12 text-gray-400">
                  Sin registros
                </td>
              </tr>
            ) : (
              menus.map((m) => (
                <tr key={m.id} className="border-b last:border-0 hover:bg-gray-50 transition-colors">
                  <td className="px-4 py-3 text-gray-500">{m.id}</td>
                  <td className="px-4 py-3 font-medium text-gray-900">{m.name}</td>
                  <td className="px-4 py-3 text-gray-600">{m.abreviation}</td>
                  <td className="px-4 py-3 text-gray-600 font-mono text-xs">{m.ruta}</td>
                  <td className="px-4 py-3 text-gray-600 max-w-[120px] truncate" title={m.imagen}>
                    {m.imagen || <span className="text-gray-300">—</span>}
                  </td>
                  <td className="px-4 py-3 text-gray-600">
                    {m.claveMenuPadre ?? <span className="text-gray-300">—</span>}
                  </td>
                  <td className="px-4 py-3 text-gray-600">{m.order}</td>
                  <td className="px-4 py-3">
                    <span className={`px-2 py-1 rounded-full text-xs font-semibold ${
                      m.enabled ? "bg-green-100 text-green-700" : "bg-red-100 text-red-700"
                    }`}>
                      {m.enabled ? "Enabled" : "Inenabled"}
                    </span>
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex gap-2">
                      <button
                        onClick={() => openEdit(m)}
                        className="text-blue-600 hover:text-blue-800 text-xs border border-blue-300 px-2 py-1 rounded hover:bg-blue-50 transition-colors"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => setDeleteTarget({ id: m.id, name: m.name })}
                        className="text-red-600 hover:text-red-800 text-xs border border-red-300 px-2 py-1 rounded hover:bg-red-50 transition-colors"
                      >
                        Delete
                      </button>
                    </div>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {/* ── Pagination ── */}
      {total > 0 && (
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 mt-4 text-sm text-gray-600">
          <span>{total} registro{total !== 1 ? "s" : ""} en total</span>
          <div className="flex gap-2 items-center flex-wrap">
            <button
              disabled={pageIndex === 1}
              onClick={() => setPageIndex((p) => Math.max(1, p - 1))}
              className="px-3 py-1 border rounded disabled:opacity-40 hover:bg-gray-100 transition-colors"
            >
              ← Previous
            </button>
            <span className="px-2">Page {pageIndex} of {totalPages}</span>
            <button
              disabled={pageIndex >= totalPages}
              onClick={() => setPageIndex((p) => p + 1)}
              className="px-3 py-1 border rounded disabled:opacity-40 hover:bg-gray-100 transition-colors"
            >
              Next →
            </button>
          </div>
        </div>
      )}

      {/* ── Add / Edit Modal ── */}
      {modal.open && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl w-full max-w-lg">
            <div className="flex items-center justify-between px-6 py-4 border-b">
              <h2 className="text-lg font-bold">
                {modal.mode === "add" ? "New Menú" : "Edit Menú"}
              </h2>
              <button onClick={closeModal} className="text-gray-400 hover:text-gray-600 text-2xl font-bold leading-none">×</button>
            </div>

            <form onSubmit={handleSave} className="p-6 space-y-4">
              {modalError && (
                <div className="p-2 bg-red-100 border border-red-400 text-red-700 rounded text-sm">{modalError}</div>
              )}

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Name <span className="text-red-500">*</span>
                  </label>
                  <input
                    name="name"
                    required
                    value={modal.data.name}
                    onChange={handleFormChange}
                    className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Abreviation</label>
                  <input
                    name="abreviation"
                    value={modal.data.abreviation ?? ""}
                    onChange={handleFormChange}
                    className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Ruta</label>
                  <input
                    name="ruta"
                    value={modal.data.ruta ?? ""}
                    onChange={handleFormChange}
                    placeholder="/ruta/del/menu"
                    className="w-full border border-gray-300 rounded p-2 text-sm font-mono focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Imagen</label>
                  <input
                    name="imagen"
                    value={modal.data.imagen ?? ""}
                    onChange={handleFormChange}
                    placeholder="ícono o URL"
                    className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Menú Padre (ID)</label>
                  <input
                    name="claveMenuPadre"
                    type="number"
                    min={0}
                    value={modal.data.claveMenuPadre ?? ""}
                    onChange={handleFormChange}
                    placeholder="Vacío = raíz"
                    className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Order</label>
                  <input
                    name="order"
                    type="number"
                    min={0}
                    value={modal.data.order ?? 0}
                    onChange={handleFormChange}
                    className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div className="flex items-center gap-2 mt-5 col-span-2">
                  <input
                    id="menu-enabled-check"
                    name="enabled"
                    type="checkbox"
                    checked={modal.data.enabled ?? true}
                    onChange={handleFormChange}
                    className="w-4 h-4 accent-blue-600"
                  />
                  <label htmlFor="menu-enabled-check" className="text-sm font-medium text-gray-700">Enabled</label>
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Descripción</label>
                <textarea
                  name="descripcion"
                  value={modal.data.descripcion ?? ""}
                  onChange={handleFormChange}
                  rows={2}
                  className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
                />
              </div>

              <div className="flex justify-end gap-3 pt-2">
                <button
                  type="button"
                  onClick={closeModal}
                  className="px-4 py-2 border border-gray-300 rounded text-sm hover:bg-gray-50 transition-colors"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={saving}
                  className="px-4 py-2 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-300 text-white rounded text-sm font-semibold transition-colors"
                >
                  {saving ? "Guardando..." : "Guardar"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* ── Delete Confirmation ── */}
      {deleteTarget && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl p-6 w-full max-w-sm">
            <h2 className="text-lg font-bold mb-2">Confirmar eliminación</h2>
            <p className="text-sm text-gray-600 mb-6">
              ¿Estás seguro de que deseas eliminar el menú{" "}
              <strong>{deleteTarget.name || `#${deleteTarget.id}`}</strong>?{" "}
              Esta acción no se puede deshacer.
            </p>
            <div className="flex justify-end gap-3">
              <button
                onClick={() => setDeleteTarget(null)}
                disabled={deleting}
                className="px-4 py-2 border border-gray-300 rounded text-sm hover:bg-gray-50 transition-colors disabled:opacity-50"
              >
                Cancel
              </button>
              <button
                onClick={handleDelete}
                disabled={deleting}
                className="px-4 py-2 bg-red-600 hover:bg-red-700 disabled:bg-red-300 text-white rounded text-sm font-semibold transition-colors"
              >
                {deleting ? "Deleting..." : "Delete"}
              </button>
            </div>
          </div>
        </div>
      )}
    </main>
  );
}
