import { useState, useEffect, useCallback } from "react";
import * as roleService from "../services/roleService";

const PAGE_SIZE_OPTIONS = [5, 10, 20, 50];

const emptyForm = {
  id: 0,
  name: "",
  abbreviation: "",
  description: "",
  order: 0,
  enabled: true,
};

export default function Roles() {
  const [roles, setRoles] = useState([]);
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

  // Detail drawer (shows associated menus & users)
  const [detail, setDetail] = useState(null); // full Role object
  const [detailLoading, setDetailLoading] = useState(false);

  // Delete confirmation
  const [deleteTarget, setDeleteTarget] = useState(null);
  const [deleting, setDeleting] = useState(false);

  // ── Data loading ──────────────────────────────────────────────────────────
  const loadRoles = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const result = await roleService.getAll({
        enabled: filterEnabled,
        pageIndex,
        pageSize,
      });

      if (Array.isArray(result)) {
        setRoles(result);
        setTotal(result.length);
      } else if (result?.items) {
        setRoles(result.items);
        setTotal(result.total ?? result.items.length);
      } else {
        setRoles([]);
        setTotal(0);
      }
    } catch (e) {
      setError(e.message || "Error al cargar los roles.");
    } finally {
      setLoading(false);
    }
  }, [pageIndex, pageSize, filterEnabled]);

  useEffect(() => {
    loadRoles();
  }, [loadRoles]);

  // ── Detail helpers ────────────────────────────────────────────────────────
  async function openDetail(role) {
    setDetail({ ...role });
    setDetailLoading(true);
    try {
      const full = await roleService.getById(role.id);
      setDetail(full);
    } catch {
      // fall back to list data if detail call fails
    } finally {
      setDetailLoading(false);
    }
  }

  // ── Modal helpers ─────────────────────────────────────────────────────────
  function openAdd() {
    setModal({ open: true, mode: "add", data: { ...emptyForm } });
    setModalError("");
  }

  function openEdit(role) {
    setModal({ open: true, mode: "edit", data: { ...role } });
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
        [name]:
          type === "checkbox"
            ? checked
            : type === "number"
            ? value === "" ? "" : Number(value)
            : value,
      },
    }));
  }

  async function handleSave(e) {
    e.preventDefault();
    setModalError("");
    setSaving(true);
    try {
      const payload = {
        id: modal.data.id,
        name: modal.data.name,
        abbreviation: modal.data.abbreviation,
        description: modal.data.description,
        order: modal.data.order,
        enabled: modal.data.enabled,
      };
      if (modal.mode === "add") {
        await roleService.add(payload);
      } else {
        await roleService.update(payload);
      }
      closeModal();
      loadRoles();
    } catch (e) {
      setModalError(e.message || "Error al guardar el role.");
    } finally {
      setSaving(false);
    }
  }

  // ── Delete helpers ────────────────────────────────────────────────────────
  async function handleDelete() {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await roleService.remove(deleteTarget.id);
      setDeleteTarget(null);
      if (roles.length === 1 && pageIndex > 1) setPageIndex((p) => p - 1);
      else loadRoles();
    } catch (e) {
      setError(e.message || "Error al eliminar el role.");
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
          className="w-full sm:w-auto bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded font-semibold transition-colors text-sm"
        >
          + New Role
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
          onClick={loadRoles}
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
              {["ID", "Name", "Abbreviation", "Description", "Order", "Users", "Menu Items", "Status", "Actions"].map((h) => (
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
            ) : roles.length === 0 ? (
              <tr>
                <td colSpan={9} className="text-center py-12 text-gray-400">
                  Sin registros
                </td>
              </tr>
            ) : (
              roles.map((p) => (
                <tr key={p.id} className="border-b last:border-0 hover:bg-gray-50 transition-colors">
                  <td className="px-4 py-3 text-gray-500">{p.id}</td>
                  <td className="px-4 py-3 font-medium text-gray-900">{p.name}</td>
                  <td className="px-4 py-3 text-gray-600">{p.abbreviation}</td>
                  <td className="px-4 py-3 text-gray-500 max-w-[200px] truncate" title={p.description}>
                    {p.description || <span className="text-gray-300">—</span>}
                  </td>
                  <td className="px-4 py-3 text-gray-600">{p.order}</td>
                  <td className="px-4 py-3">
                    {p.users?.length > 0 ? (
                      <span className="bg-green-100 text-green-700 text-xs font-semibold px-2 py-1 rounded-full">
                        {p.users.length}
                      </span>
                    ) : (
                      <span className="text-gray-300">—</span>
                    )}
                  </td>
                  <td className="px-4 py-3">
                    {p.menuItems?.length > 0 ? (
                      <span className="bg-purple-100 text-purple-700 text-xs font-semibold px-2 py-1 rounded-full">
                        {p.menuItems.length}
                      </span>
                    ) : (
                      <span className="text-gray-300">—</span>
                    )}
                  </td>
                  <td className="px-4 py-3">
                    <span className={`px-2 py-1 rounded-full text-xs font-semibold ${
                      p.enabled ? "bg-green-100 text-green-700" : "bg-red-100 text-red-700"
                    }`}>
                      {p.enabled ? "Enabled" : "Inenabled"}
                    </span>
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex gap-2">
                      <button
                        onClick={() => openDetail(p)}
                        className="text-gray-600 hover:text-gray-800 text-xs border border-gray-300 px-2 py-1 rounded hover:bg-gray-50 transition-colors"
                      >
                        View
                      </button>
                      <button
                        onClick={() => openEdit(p)}
                        className="text-green-600 hover:text-green-800 text-xs border border-green-300 px-2 py-1 rounded hover:bg-green-50 transition-colors"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => setDeleteTarget({ id: p.id, name: p.name })}
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

      {/* ── Detail drawer ── */}
      {detail && (
        <div className="fixed inset-0 bg-black/50 flex justify-end z-50">
          <div className="bg-white w-full max-w-md h-full overflow-y-auto shadow-xl flex flex-col">
            <div className="flex items-center justify-between px-6 py-4 border-b bg-gray-50">
              <h2 className="text-lg font-bold">Role Details</h2>
              <button
                onClick={() => setDetail(null)}
                className="text-gray-400 hover:text-gray-600 text-2xl font-bold leading-none"
              >
                ×
              </button>
            </div>

            {detailLoading ? (
              <div className="flex-1 flex items-center justify-center text-gray-400 animate-pulse">
                Loading...
              </div>
            ) : (
              <div className="p-6 space-y-6">
                {/* Basic info */}
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 text-sm">
                  <div>
                    <p className="text-gray-400 text-xs uppercase tracking-wide mb-1">ID</p>
                    <p className="font-medium">{detail.id}</p>
                  </div>
                  <div>
                    <p className="text-gray-400 text-xs uppercase tracking-wide mb-1">Abbreviation</p>
                    <p className="font-medium">{detail.abbreviation || "—"}</p>
                  </div>
                  <div className="col-span-2">
                    <p className="text-gray-400 text-xs uppercase tracking-wide mb-1">Name</p>
                    <p className="font-semibold text-base">{detail.name}</p>
                  </div>
                  <div className="col-span-2">
                    <p className="text-gray-400 text-xs uppercase tracking-wide mb-1">Description</p>
                    <p>{detail.description || "—"}</p>
                  </div>
                  <div>
                    <p className="text-gray-400 text-xs uppercase tracking-wide mb-1">Order</p>
                    <p>{detail.order}</p>
                  </div>
                  <div>
                    <p className="text-gray-400 text-xs uppercase tracking-wide mb-1">Estatus</p>
                    <span className={`px-2 py-1 rounded-full text-xs font-semibold ${
                      detail.enabled ? "bg-green-100 text-green-700" : "bg-red-100 text-red-700"
                    }`}>
                      {detail.enabled ? "Enabled" : "Inenabled"}
                    </span>
                  </div>
                </div>

                {/* Menus */}
                <div>
                  <p className="text-gray-400 text-xs uppercase tracking-wide mb-2">
                    Menu Items ({detail.menuItems?.length ?? 0})
                  </p>
                  {detail.menuItems?.length > 0 ? (
                    <ul className="space-y-1">
                      {detail.menuItems.map((m) => (
                        <li key={m.id} className="flex items-center gap-2 text-sm bg-purple-50 rounded px-3 py-2">
                          <span className="font-mono text-purple-700 text-xs">{m.path || "—"}</span>
                          <span className="font-medium text-gray-700">{m.name}</span>
                        </li>
                      ))}
                    </ul>
                  ) : (
                    <p className="text-sm text-gray-400">Sin menús asignados.</p>
                  )}
                </div>

                {/* Users */}
                <div>
                  <p className="text-gray-400 text-xs uppercase tracking-wide mb-2">
                    Users ({detail.users?.length ?? 0})
                  </p>
                  {detail.users?.length > 0 ? (
                    <ul className="space-y-1">
                      {detail.users.map((u) => (
                        <li key={u.id} className="flex items-center gap-2 text-sm bg-green-50 rounded px-3 py-2">
                          <span className="font-medium text-green-800">{u.name}</span>
                          <span className="text-gray-400 text-xs ml-auto">{u.email}</span>
                        </li>
                      ))}
                    </ul>
                  ) : (
                    <p className="text-sm text-gray-400">Sin usuarios asignados.</p>
                  )}
                </div>

                <div className="pt-2 flex gap-3">
                  <button
                    onClick={() => { setDetail(null); openEdit(detail); }}
                    className="flex-1 bg-green-600 hover:bg-green-700 text-white py-2 rounded text-sm font-semibold transition-colors"
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => setDetail(null)}
                    className="flex-1 border border-gray-300 hover:bg-gray-50 py-2 rounded text-sm transition-colors"
                  >
                    Cerrar
                  </button>
                </div>
              </div>
            )}
          </div>
        </div>
      )}

      {/* ── Add / Edit Modal ── */}
      {modal.open && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl w-full max-w-md">
            <div className="flex items-center justify-between px-6 py-4 border-b">
              <h2 className="text-lg font-bold">
                {modal.mode === "add" ? "New Role" : "Edit Role"}
              </h2>
              <button onClick={closeModal} className="text-gray-400 hover:text-gray-600 text-2xl font-bold leading-none">
                ×
              </button>
            </div>

            <form onSubmit={handleSave} className="p-6 space-y-4">
              {modalError && (
                <div className="p-2 bg-red-100 border border-red-400 text-red-700 rounded text-sm">
                  {modalError}
                </div>
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
                    className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Abbreviation</label>
                  <input
                    name="abbreviation"
                    value={modal.data.abbreviation ?? ""}
                    onChange={handleFormChange}
                    className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
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
                    className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
                  />
                </div>

                <div className="flex items-center gap-2 mt-6">
                  <input
                    id="role-enabled-check"
                    name="enabled"
                    type="checkbox"
                    checked={modal.data.enabled ?? true}
                    onChange={handleFormChange}
                    className="w-4 h-4 accent-green-600"
                  />
                  <label htmlFor="role-enabled-check" className="text-sm font-medium text-gray-700">
                    Enabled
                  </label>
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Description</label>
                <textarea
                  name="description"
                  value={modal.data.description ?? ""}
                  onChange={handleFormChange}
                  rows={3}
                  className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500 resize-none"
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
                  className="px-4 py-2 bg-green-600 hover:bg-green-700 disabled:bg-green-300 text-white rounded text-sm font-semibold transition-colors"
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
            <h2 className="text-lg font-bold mb-2">Confirm eliminación</h2>
            <p className="text-sm text-gray-600 mb-6">
              ¿Estás seguro de que deseas eliminar el role{" "}
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
