import { useState, useEffect, useCallback } from "react";
import * as userService from "../services/userService";

const PAGE_SIZE_OPTIONS = [5, 10, 20, 50];

const emptyForm = {
  id: 0,
  name: "",
  abreviation: "",
  descripcion: "",
  userName: "",
  firstName: "",
  lastNameFather: "",
  lastNameMother: "",
  email: "",
  password: "",
  employeeId: 0,
  order: 0,
  enabled: true,
  estatus: true
};

function normalizeUser(raw = {}) {
  return {
    ...raw,
    id: raw.id ?? raw.idUser ?? raw.Id ?? raw.IdUser ?? 0,
    name: raw.name ?? raw.Name ?? "",
    abreviation: raw.abreviation ?? raw.Abreviation ?? "",
    descripcion: raw.descripcion ?? raw.Descripcion ?? "",
    userName: raw.userName ?? raw.UserName ?? "",
    firstName: raw.firstName ?? raw.FirstName ?? "",
    lastNameFather: raw.lastNameFather ?? raw.LastNameFather ?? "",
    lastNameMother: raw.lastNameMother ?? raw.LastNameMother ?? "",
    email: raw.email ?? raw.Email ?? "",
    employeeId: raw.employeeId ?? raw.EmployeeId ?? 0,
    order: raw.order ?? raw.Order ?? 0,
    enabled: raw.enabled ?? raw.Enabled ?? false,
    attempts: raw.attempts ?? raw.Attempts ?? 0,
    avatar: raw.avatar ?? raw.Avatar ?? "",
    roles: raw.roles ?? raw.Roles ?? [],
    companies: raw.companies ?? raw.Companies ?? [],
  };
}

export default function Users() {
  const [users, setUsers] = useState([]);
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

  // Detail drawer
  const [detail, setDetail] = useState(null);
  const [detailLoading, setDetailLoading] = useState(false);

  // Change password modal
  const [pwdModal, setPwdModal] = useState({ open: false, username: "", password: "", confirm: "", saving: false, error: "" });

  // Delete confirmation
  const [deleteTarget, setDeleteTarget] = useState(null);
  const [deleting, setDeleting] = useState(false);

  // ── Data loading ──────────────────────────────────────────────────────────
  const loadUsers = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const result = await userService.getAll({ enabled: filterEnabled, pageIndex, pageSize });
      if (Array.isArray(result)) {
        setUsers(result.map((u) => normalizeUser(u)));
        setTotal(result.length);
      } else if (result?.items) {
        setUsers(result.items.map((u) => normalizeUser(u)));
        setTotal(result.total ?? result.items.length);
      } else {
        setUsers([]);
        setTotal(0);
      }
    } catch (e) {
      setError(e.message || "Error al cargar los users.");
    } finally {
      setLoading(false);
    }
  }, [pageIndex, pageSize, filterEnabled]);

  useEffect(() => { loadUsers(); }, [loadUsers]);

  // ── Detail drawer ─────────────────────────────────────────────────────────
  async function openDetail(u) {
    setDetail(normalizeUser(u));
    setDetailLoading(true);
    try {
      const full = await userService.getById(u.id);
      setDetail(normalizeUser(full));
    } catch { /* fall back to list data */ }
    finally { setDetailLoading(false); }
  }

  // ── Unlock ────────────────────────────────────────────────────────────────
  async function handleUnlock(u) {
    try {
      await userService.unlockUser(u.id);
      loadUsers();
      if (detail?.id === u.id) setDetail((d) => ({ ...d, bloqueado: false, attempts: 0 }));
    } catch (e) {
      setError(e.message || "Error al desbloquear user.");
    }
  }

  // ── Add / Edit modal ──────────────────────────────────────────────────────
  function openAdd() {
    setModal({ open: true, mode: "add", data: { ...emptyForm } });
    setModalError("");
  }

  async function openEdit(u) {
    const fallback = normalizeUser(u);
    let source = fallback;

    try {
      const full = await userService.getById(fallback.id);
      source = normalizeUser(full);
    } catch {
      // Fallback to row data if full payload can't be loaded.
    }

    setModal({
      open: true, mode: "edit",
      data: {
        ...emptyForm,
        ...source,
        userName: source.userName ?? "",
        password: "",
        idUserPorAusencia: source.idUserPorAusencia ?? "",
      },
    });
    setModalError("");
  }

  function closeModal() { setModal((m) => ({ ...m, open: false })); }

  function handleFormChange(e) {
    const { name, value, type, checked } = e.target;
    setModal((m) => ({
      ...m,
      data: {
        ...m.data,
        [name]: type === "checkbox" ? checked
          : type === "number" ? (value === "" ? "" : Number(value))
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
        ...modal.data,
        idUserPorAusencia: modal.data.idUserPorAusencia === "" ? null : Number(modal.data.idUserPorAusencia),
      };
      if (!payload.password) delete payload.password; // skip empty password on edit
      if (modal.mode === "add") {
        await userService.add(payload);
      } else {
        await userService.update(payload);
      }
      closeModal();
      loadUsers();
    } catch (e) {
      setModalError(e.message || "Error al guardar el user.");
    } finally {
      setSaving(false);
    }
  }

  // ── Change password modal ─────────────────────────────────────────────────
  function openPwdModal(u) {
    setPwdModal({ open: true, username: u.userName ?? u.firstName ?? "", password: "", confirm: "", saving: false, error: "" });
  }

  async function handlePwdSave(e) {
    e.preventDefault();
    if (pwdModal.password !== pwdModal.confirm) {
      setPwdModal((m) => ({ ...m, error: "Las passwords no coinciden." }));
      return;
    }
    setPwdModal((m) => ({ ...m, saving: true, error: "" }));
    try {
      await userService.updatePassword(pwdModal.username, pwdModal.password);
      setPwdModal((m) => ({ ...m, open: false }));
    } catch (e) {
      setPwdModal((m) => ({ ...m, saving: false, error: e.message || "Error al cambiar password." }));
    }
  }

  // ── Delete ────────────────────────────────────────────────────────────────
  async function handleDelete() {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await userService.remove(deleteTarget.id);
      setDeleteTarget(null);
      if (users.length === 1 && pageIndex > 1) setPageIndex((p) => p - 1);
      else loadUsers();
    } catch (e) {
      setError(e.message || "Error al eliminar el user.");
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
        <button onClick={openAdd} className="w-full sm:w-auto bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded font-semibold transition-colors text-sm">
          + New User
        </button>

        <select
          value={filterEnabled === undefined ? "" : String(filterEnabled)}
          onChange={(e) => { setFilterEnabled(e.target.value === "" ? undefined : e.target.value === "true"); setPageIndex(1); }}
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
          {PAGE_SIZE_OPTIONS.map((n) => <option key={n} value={n}>{n} per page</option>)}
        </select>

        <button onClick={loadUsers} disabled={loading} className="w-full sm:w-auto sm:ml-auto border border-gray-300 bg-white hover:bg-gray-50 disabled:opacity-50 px-4 py-2 rounded text-sm transition-colors">
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
              {["ID", "Full Name", "User", "Email", "Nº Emp.", "Roles", "Estatus", "Actions"].map((h) => (
                <th key={h} className="px-4 py-3 text-left font-semibold text-gray-600 whitespace-nowrap">{h}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={9} className="text-center py-12 text-gray-400"><span className="animate-pulse">Loading...</span></td></tr>
            ) : users.length === 0 ? (
              <tr><td colSpan={9} className="text-center py-12 text-gray-400">Sin registros</td></tr>
            ) : (
              users.map((u) => (
                <tr key={u.id} className="border-b last:border-0 hover:bg-gray-50 transition-colors">
                  <td className="px-4 py-3 text-gray-500">{u.id}</td>
                  <td className="px-4 py-3">
                    <div className="font-medium text-gray-900">{u.nameCompleto || `${u.name ?? ""}`}</div>
                    <div className="text-xs text-gray-400">{u.lastNameFather} {u.lastNameMother}</div>
                  </td>
                  <td className="px-4 py-3 text-gray-600 font-mono text-xs">{u.userName ?? u.firstName}</td>
                  <td className="px-4 py-3 text-gray-600">{u.email}</td>
                  <td className="px-4 py-3 text-gray-600 text-center">{u.employeeId || <span className="text-gray-300">—</span>}</td>
                  <td className="px-4 py-3">
                    {u.roles?.length > 0
                      ? <span className="bg-purple-100 text-purple-700 text-xs font-semibold px-2 py-1 rounded-full">{u.roles.length}</span>
                      : <span className="text-gray-300">—</span>}
                  </td>
                  <td className="px-4 py-3">
                    <span className={`px-2 py-1 rounded-full text-xs font-semibold ${u.enabled ? "bg-green-100 text-green-700" : "bg-red-100 text-red-700"}`}>
                      {u.enabled ? "Enabled" : "Inenabled"}
                    </span>
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex gap-1 flex-wrap">
                      <button onClick={() => openDetail(u)} className="text-gray-600 hover:text-gray-800 text-xs border border-gray-300 px-2 py-1 rounded hover:bg-gray-50 transition-colors">View</button>
                      <button onClick={() => openEdit(u)} className="text-blue-600 hover:text-blue-800 text-xs border border-blue-300 px-2 py-1 rounded hover:bg-blue-50 transition-colors">Edit</button>
                      <button onClick={() => openPwdModal(u)} className="text-amber-600 hover:text-amber-800 text-xs border border-amber-300 px-2 py-1 rounded hover:bg-amber-50 transition-colors">Clave</button>
                      {u.bloqueado && (
                        <button onClick={() => handleUnlock(u)} className="text-green-600 hover:text-green-800 text-xs border border-green-300 px-2 py-1 rounded hover:bg-green-50 transition-colors">Desbloquear</button>
                      )}
                      <button onClick={() => setDeleteTarget({ id: u.id, name: u.nameCompleto || u.name })} className="text-red-600 hover:text-red-800 text-xs border border-red-300 px-2 py-1 rounded hover:bg-red-50 transition-colors">Delete</button>
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
            <button disabled={pageIndex === 1} onClick={() => setPageIndex((p) => Math.max(1, p - 1))} className="px-3 py-1 border rounded disabled:opacity-40 hover:bg-gray-100 transition-colors">← Previous</button>
            <span className="px-2">Page {pageIndex} of {totalPages}</span>
            <button disabled={pageIndex >= totalPages} onClick={() => setPageIndex((p) => p + 1)} className="px-3 py-1 border rounded disabled:opacity-40 hover:bg-gray-100 transition-colors">Next →</button>
          </div>
        </div>
      )}

      {/* ── Detail Drawer ── */}
      {detail && (
        <div className="fixed inset-0 bg-black/50 flex justify-end z-50">
          <div className="bg-white w-full max-w-md h-full overflow-y-auto shadow-xl flex flex-col">
            <div className="flex items-center justify-between px-6 py-4 border-b bg-gray-50">
              <h2 className="text-lg font-bold">Detalle de User</h2>
              <button onClick={() => setDetail(null)} className="text-gray-400 hover:text-gray-600 text-2xl font-bold leading-none">×</button>
            </div>

            {detailLoading ? (
              <div className="flex-1 flex items-center justify-center text-gray-400 animate-pulse">Loading...</div>
            ) : (
              <div className="p-6 space-y-6 text-sm">
                {/* Avatar + name */}
                <div className="flex items-center gap-4">
                  {detail.avatar ? (
                    <img src={detail.avatar} alt="foto" className="w-16 h-16 rounded-full object-cover border" />
                  ) : (
                    <div className="w-16 h-16 rounded-full bg-blue-100 flex items-center justify-center text-blue-600 text-2xl font-bold select-none">
                      {(detail.name ?? "?")[0]?.toUpperCase()}
                    </div>
                  )}
                  <div>
                    <p className="font-bold text-lg">{detail.nameCompleto || detail.name}</p>
                    <p className="text-gray-400 font-mono">{detail.userName ?? detail.firstName}</p>
                    <p className="text-gray-500">{detail.email}</p>
                  </div>
                </div>

                {/* Flags */}
                <div className="flex gap-2 flex-wrap">
                  <span className={`px-2 py-1 rounded-full text-xs font-semibold ${detail.enabled ? "bg-green-100 text-green-700" : "bg-gray-100 text-gray-500"}`}>{detail.enabled ? "Enabled" : "Inenabled"}</span>
                  {detail.cambioPassword && <span className="px-2 py-1 rounded-full text-xs font-semibold bg-amber-100 text-amber-700">Cambio de clave requerido</span>}
                </div>

                {/* Fields */}
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                  {[
                    ["ID", detail.id],
                    ["Clave de user", detail.userName || "—"],
                    ["Nº Empleado", detail.employeeId || "—"],
                    ["Apellido Paterno", detail.lastNameFather || "—"],
                    ["Apellido Materno", detail.lastNameMother || "—"],
                    ["Abreviation", detail.abreviation || "—"],
                    ["Order", detail.order],
                    ["Intentos fallidos", detail.attempts ?? 0],
                    ["ID Sustituto", detail.idUserPorAusencia ?? "—"],
                  ].map(([label, val]) => (
                    <div key={label}>
                      <p className="text-gray-400 text-xs uppercase tracking-wide mb-0.5">{label}</p>
                      <p className="font-medium">{val}</p>
                    </div>
                  ))}
                  {detail.descripcion && (
                    <div className="col-span-2">
                      <p className="text-gray-400 text-xs uppercase tracking-wide mb-0.5">Descripción</p>
                      <p>{detail.descripcion}</p>
                    </div>
                  )}
                </div>

                {/* Roles */}
                <div>
                  <p className="text-gray-400 text-xs uppercase tracking-wide mb-2">Roles ({detail.roles?.length ?? 0})</p>
                  {detail.roles?.length > 0 ? (
                    <ul className="space-y-1">
                      {detail.roles.map((p) => (
                        <li key={p.id} className="bg-purple-50 rounded px-3 py-2 font-medium text-purple-800">{p.name} <span className="text-purple-400 font-normal text-xs ml-1">({p.abreviation})</span></li>
                      ))}
                    </ul>
                  ) : <p className="text-gray-400">Sin roles asignados.</p>}
                </div>

                {/* Companies */}
                <div>
                  <p className="text-gray-400 text-xs uppercase tracking-wide mb-2">Companies ({detail.companies?.length ?? 0})</p>
                  {detail.companies?.length > 0 ? (
                    <ul className="space-y-1">
                      {detail.companies.map((e) => (
                        <li key={e.id} className="bg-blue-50 rounded px-3 py-2 font-medium text-blue-800">{e.name} <span className="text-blue-400 font-normal text-xs ml-1">({e.abreviation})</span></li>
                      ))}
                    </ul>
                  ) : <p className="text-gray-400">Sin companies asignadas.</p>}
                </div>

                {/* Actions */}
                <div className="flex gap-2 pt-2 flex-wrap">
                  <button onClick={() => { setDetail(null); openEdit(detail); }} className="flex-1 bg-blue-600 hover:bg-blue-700 text-white py-2 rounded text-sm font-semibold transition-colors">Edit</button>
                  <button onClick={() => { openPwdModal(detail); setDetail(null); }} className="flex-1 bg-amber-500 hover:bg-amber-600 text-white py-2 rounded text-sm font-semibold transition-colors">Cambiar Clave</button>
                  {detail.bloqueado && (
                    <button onClick={() => handleUnlock(detail)} className="flex-1 bg-green-600 hover:bg-green-700 text-white py-2 rounded text-sm font-semibold transition-colors">Desbloquear</button>
                  )}
                  <button onClick={() => setDetail(null)} className="flex-1 border border-gray-300 hover:bg-gray-50 py-2 rounded text-sm transition-colors">Cerrar</button>
                </div>
              </div>
            )}
          </div>
        </div>
      )}

      {/* ── Add / Edit Modal ── */}
      {modal.open && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl w-full max-w-2xl max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between px-6 py-4 border-b sticky top-0 bg-white z-10">
              <h2 className="text-lg font-bold">{modal.mode === "add" ? "New User" : "Edit User"}</h2>
              <button onClick={closeModal} className="text-gray-400 hover:text-gray-600 text-2xl font-bold leading-none">×</button>
            </div>

            <form onSubmit={handleSave} className="p-6 space-y-5">
              {modalError && <div className="p-2 bg-red-100 border border-red-400 text-red-700 rounded text-sm">{modalError}</div>}

              {/* Name section */}
              <div>
                <p className="text-xs font-semibold text-gray-400 uppercase tracking-wide mb-3">Datos personales</p>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Name <span className="text-red-500">*</span></label>
                    <input name="name" required value={modal.data.name} onChange={handleFormChange} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Abreviation</label>
                    <input name="abreviation" value={modal.data.abreviation ?? ""} onChange={handleFormChange} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Apellido Paterno</label>
                    <input name="lastNameFather" value={modal.data.lastNameFather ?? ""} onChange={handleFormChange} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Apellido Materno</label>
                    <input name="lastNameMother" value={modal.data.lastNameMother ?? ""} onChange={handleFormChange} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Email</label>
                    <input name="email" type="email" value={modal.data.email ?? ""} onChange={handleFormChange} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Nº Empleado</label>
                    <input name="employeeId" type="number" min={0} value={modal.data.employeeId ?? 0} onChange={handleFormChange} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                  </div>
                </div>
              </div>

              {/* Account section */}
              <div>
                <p className="text-xs font-semibold text-gray-400 uppercase tracking-wide mb-3">Cuenta</p>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Clave de user <span className="text-red-500">*</span></label>
                    <input name="userName" required value={modal.data.userName ?? ""} onChange={handleFormChange} className="w-full border border-gray-300 rounded p-2 text-sm font-mono focus:outline-none focus:ring-2 focus:ring-blue-500" />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Name de user</label>
                    <input name="firstName" value={modal.data.firstName ?? ""} onChange={handleFormChange} className="w-full border border-gray-300 rounded p-2 text-sm font-mono focus:outline-none focus:ring-2 focus:ring-blue-500" />
                  </div>
                  {modal.mode === "add" && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Password <span className="text-red-500">*</span>
                    </label>
                    <input name="password" type="password" required value={modal.data.password ?? ""} onChange={handleFormChange} autoComplete="new-password" className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                  </div>
                  )}
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Order</label>
                    <input name="order" type="number" min={0} value={modal.data.order ?? 0} onChange={handleFormChange} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">ID User por Ausencia</label>
                    <input name="idUserPorAusencia" type="number" min={0} value={modal.data.idUserPorAusencia ?? ""} onChange={handleFormChange} placeholder="Vacío = ninguno" className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                  </div>
                </div>

                {/* Flags */}
                <div className="flex flex-wrap gap-6 mt-4">
                  {[
                    { name: "enabled", label: "Enabled" },
                    { name: "cambioPassword", label: "Forzar cambio de clave" },
                  ].map(({ name, label }) => (
                    <div key={name} className="flex items-center gap-2">
                      <input id={`flag-${name}`} name={name} type="checkbox" checked={modal.data[name] ?? false} onChange={handleFormChange} className="w-4 h-4 accent-blue-600" />
                      <label htmlFor={`flag-${name}`} className="text-sm font-medium text-gray-700">{label}</label>
                    </div>
                  ))}
                </div>
              </div>

              {/* Descripción */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Descripción</label>
                <textarea name="descripcion" value={modal.data.descripcion ?? ""} onChange={handleFormChange} rows={2} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none" />
              </div>

              <div className="flex justify-end gap-3 pt-2">
                <button type="button" onClick={closeModal} className="px-4 py-2 border border-gray-300 rounded text-sm hover:bg-gray-50 transition-colors">Cancel</button>
                <button type="submit" disabled={saving} className="px-4 py-2 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-300 text-white rounded text-sm font-semibold transition-colors">
                  {saving ? "Guardando..." : "Guardar"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* ── Change Password Modal ── */}
      {pwdModal.open && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl w-full max-w-sm">
            <div className="flex items-center justify-between px-6 py-4 border-b">
              <h2 className="text-lg font-bold">Cambiar Password</h2>
              <button onClick={() => setPwdModal((m) => ({ ...m, open: false }))} className="text-gray-400 hover:text-gray-600 text-2xl font-bold leading-none">×</button>
            </div>
            <form onSubmit={handlePwdSave} className="p-6 space-y-4">
              {pwdModal.error && <div className="p-2 bg-red-100 border border-red-400 text-red-700 rounded text-sm">{pwdModal.error}</div>}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">User</label>
                <input value={pwdModal.username} readOnly className="w-full border border-gray-200 bg-gray-50 rounded p-2 text-sm font-mono text-gray-500" />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">New password <span className="text-red-500">*</span></label>
                <input type="password" required autoComplete="new-password" value={pwdModal.password} onChange={(e) => setPwdModal((m) => ({ ...m, password: e.target.value }))} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Confirmar password <span className="text-red-500">*</span></label>
                <input type="password" required value={pwdModal.confirm} onChange={(e) => setPwdModal((m) => ({ ...m, confirm: e.target.value }))} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
              </div>
              <div className="flex justify-end gap-3 pt-2">
                <button type="button" onClick={() => setPwdModal((m) => ({ ...m, open: false }))} className="px-4 py-2 border border-gray-300 rounded text-sm hover:bg-gray-50 transition-colors">Cancel</button>
                <button type="submit" disabled={pwdModal.saving} className="px-4 py-2 bg-amber-500 hover:bg-amber-600 disabled:bg-amber-300 text-white rounded text-sm font-semibold transition-colors">
                  {pwdModal.saving ? "Guardando..." : "Cambiar Clave"}
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
              ¿Estás seguro de que deseas eliminar al user <strong>{deleteTarget.name || `#${deleteTarget.id}`}</strong>? Esta acción no se puede deshacer.
            </p>
            <div className="flex justify-end gap-3">
              <button onClick={() => setDeleteTarget(null)} disabled={deleting} className="px-4 py-2 border border-gray-300 rounded text-sm hover:bg-gray-50 transition-colors disabled:opacity-50">Cancel</button>
              <button onClick={handleDelete} disabled={deleting} className="px-4 py-2 bg-red-600 hover:bg-red-700 disabled:bg-red-300 text-white rounded text-sm font-semibold transition-colors">
                {deleting ? "Deleting..." : "Delete"}
              </button>
            </div>
          </div>
        </div>
      )}
    </main>
  );
}
