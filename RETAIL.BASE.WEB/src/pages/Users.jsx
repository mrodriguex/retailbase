import { useState, useEffect, useCallback } from "react";
import * as userService from "../services/userService";
import * as roleService from "../services/roleService";
import * as companyService from "../services/companyService";
import avatar from "../assets/avatar.png";

const PAGE_SIZE_OPTIONS = [5, 10, 20, 50];

const emptyForm = {
  id: 0,
  name: "",
  abbreviation: "",
  description: "",
  userName: "",
  firstName: "",
  lastNameFather: "",
  lastNameMother: "",
  email: "",
  password: "",
  employeeId: 0,
  order: 0,
  enabled: true,
};

function normalizeUser(raw = {}) {
  return {
    ...raw,
    id: raw.id ?? raw.idUser ?? raw.Id ?? raw.IdUser ?? 0,
    name: raw.name ?? raw.Name ?? "",
    abbreviation: raw.abbreviation ?? raw.abreviation ?? raw.Abbreviation ?? raw.Abreviation ?? "",
    description: raw.description ?? raw.descripcion ?? raw.Description ?? raw.Descripcion ?? "",
    userName: raw.userName ?? raw.UserName ?? "",
    firstName: raw.firstName ?? raw.FirstName ?? "",
    lastNameFather: raw.lastNameFather ?? raw.LastNameFather ?? "",
    lastNameMother: raw.lastNameMother ?? raw.LastNameMother ?? "",
    email: raw.email ?? raw.Email ?? "",
    employeeId: raw.employeeId ?? raw.EmployeeId ?? 0,
    order: raw.order ?? raw.Order ?? 0,
    enabled: raw.enabled ?? raw.Enabled ?? false,
    attempts: raw.attempts ?? raw.Attempts ?? 0,
    avatar: avatar,
    roles: raw.roles ?? raw.Roles ?? [],
    companys: raw.companys ?? raw.companies ?? raw.Companys ?? raw.Companies ?? [],
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

  // Roles modal
  const [rolesModal, setRolesModal] = useState({
    open: false, userId: null, userName: "",
    userRoles: [], allRoles: [],
    loading: false, actionLoading: false,
    selectedRoleId: "", error: "",
  });

  // Companies modal
  const [companiesModal, setCompaniesModal] = useState({
    open: false, userId: null, userName: "",
    userCompanies: [], allCompanies: [],
    loading: false, actionLoading: false,
    selectedCompanyId: "", error: "",
  });

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
      if (detail?.id === u.id) setDetail((d) => ({ ...d, attempts: 0 }));
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
        id: modal.data.id,
        name: modal.data.name,
        abbreviation: modal.data.abbreviation,
        description: modal.data.description,
        userName: modal.data.userName,
        firstName: modal.data.firstName,
        lastNameFather: modal.data.lastNameFather,
        lastNameMother: modal.data.lastNameMother,
        email: modal.data.email,
        employeeId: modal.data.employeeId,
        order: modal.data.order,
        enabled: modal.data.enabled,
        ...(modal.data.password ? { password: modal.data.password } : {}),
      };
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

  // ── Role management ───────────────────────────────────────────────────────
  async function openRolesModal(u) {
    setRolesModal({ open: true, userId: u.id, userName: u.name, userRoles: [], allRoles: [], loading: true, actionLoading: false, selectedRoleId: "", error: "" });
    try {
      const [userRoles, allResult] = await Promise.all([
        roleService.getUserProfiles(u.id),
        roleService.getAll({ pageSize: 9999 }),
      ]);
      const normalizedRoles = Array.isArray(userRoles) ? userRoles : [];
      setRolesModal(m => ({
        ...m,
        loading: false,
        userRoles: normalizedRoles,
        allRoles: allResult?.items ?? (Array.isArray(allResult) ? allResult : []),
      }));
      setUsers(prev => prev.map(usr => usr.id === u.id ? { ...usr, roles: normalizedRoles } : usr));
    } catch (e) {
      setRolesModal(m => ({ ...m, loading: false, error: e.message || "Error loading roles." }));
    }
  }

  async function handleAssignRole(idRole) {
    setRolesModal(m => ({ ...m, actionLoading: true, error: "" }));
    try {
      await roleService.assignProfileToUser(rolesModal.userId, idRole);
      const userRoles = await roleService.getUserProfiles(rolesModal.userId);
      const normalizedRoles = Array.isArray(userRoles) ? userRoles : [];
      setRolesModal(m => ({ ...m, actionLoading: false, selectedRoleId: "", userRoles: normalizedRoles }));
      setUsers(prev => prev.map(u => u.id === rolesModal.userId ? { ...u, roles: normalizedRoles } : u));
    } catch (e) {
      const message = e.response?.data?.error || e.message;
      setRolesModal(m => ({ ...m, actionLoading: false, error: message }));
    }
  }

  async function handleRemoveRole(idRole) {
    setRolesModal(m => ({ ...m, actionLoading: true, error: "" }));
    try {
      await roleService.removeProfileFromUser(rolesModal.userId, idRole);
      const userRoles = await roleService.getUserProfiles(rolesModal.userId);
      const normalizedRoles = Array.isArray(userRoles) ? userRoles : [];
      setRolesModal(m => ({ ...m, actionLoading: false, userRoles: normalizedRoles }));
      setUsers(prev => prev.map(u => u.id === rolesModal.userId ? { ...u, roles: normalizedRoles } : u));
    } catch (e) {
      const message = e.response?.data?.error || e.message;
      setRolesModal(m => ({ ...m, actionLoading: false, error: message }));
    }
  }

  const totalPages = Math.max(1, Math.ceil(total / pageSize));

  // ── Company management ───────────────────────────────────────────────
  async function openCompaniesModal(u) {
    setCompaniesModal({ open: true, userId: u.id, userName: u.name, userCompanies: [], allCompanies: [], loading: true, actionLoading: false, selectedCompanyId: "", error: "" });
    try {
      const [userCompanies, allResult] = await Promise.all([
        companyService.getCompaniesByUser(u.id),
        companyService.getAll({ pageSize: 9999 }),
      ]);
      const normalizedCompanies = Array.isArray(userCompanies) ? userCompanies : (userCompanies?.items ?? []);
      setCompaniesModal(m => ({
        ...m,
        loading: false,
        userCompanies: normalizedCompanies,
        allCompanies: allResult?.items ?? (Array.isArray(allResult) ? allResult : []),
      }));
      setUsers(prev => prev.map(usr => usr.id === u.id ? { ...usr, companys: normalizedCompanies } : usr));
    } catch (e) {
      setCompaniesModal(m => ({ ...m, loading: false, error: e.message || "Error loading companies." }));
    }
  }

  async function handleAssignCompany(idCompany) {
    setCompaniesModal(m => ({ ...m, actionLoading: true, error: "" }));
    try {
      await companyService.assignCompanyToUser(companiesModal.userId, idCompany);
      const userCompanies = await companyService.getCompaniesByUser(companiesModal.userId);
      const normalizedCompanies = Array.isArray(userCompanies) ? userCompanies : (userCompanies?.items ?? []);
      setCompaniesModal(m => ({ ...m, actionLoading: false, selectedCompanyId: "", userCompanies: normalizedCompanies }));
      setUsers(prev => prev.map(u => u.id === companiesModal.userId ? { ...u, companys: normalizedCompanies } : u));
    } catch (e) {
      const message = e.response?.data?.error || e.message;
      setCompaniesModal(m => ({ ...m, actionLoading: false, error: message }));
    }
  }

  async function handleRemoveCompany(idCompany) {
    setCompaniesModal(m => ({ ...m, actionLoading: true, error: "" }));
    try {
      await companyService.removeCompanyFromUser(companiesModal.userId, idCompany);
      const userCompanies = await companyService.getCompaniesByUser(companiesModal.userId);
      const normalizedCompanies = Array.isArray(userCompanies) ? userCompanies : (userCompanies?.items ?? []);
      setCompaniesModal(m => ({ ...m, actionLoading: false, userCompanies: normalizedCompanies }));
      setUsers(prev => prev.map(u => u.id === companiesModal.userId ? { ...u, companys: normalizedCompanies } : u));
    } catch (e) {
      const message = e.response?.data?.error || e.message;
      setCompaniesModal(m => ({ ...m, actionLoading: false, error: message }));
    }
  }

  // ── Render ────────────────────────────────────────────────────────────────
  return (
    <main className="max-w-7xl mx-auto p-3 sm:p-4 md:p-6">

      {/* ── Toolbar ── */}
      <div className="flex flex-wrap gap-2 sm:gap-3 items-center mb-5">
        <button onClick={openAdd} className="w-full sm:w-auto bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded font-semibold transition-colors text-sm">
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
              {["ID", "Full Name", "User", "Email", "Nº Emp.", "Roles", "Companies", "Estatus", "Actions"].map((h) => (
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
                    <div className="font-medium text-gray-900">{u.name}</div>
                    <div className="text-xs text-gray-400">{u.lastNameFather} {u.lastNameMother}</div>
                  </td>
                  <td className="px-4 py-3 text-gray-600 font-mono text-xs">{u.userName ?? u.firstName}</td>
                  <td className="px-4 py-3 text-gray-600">{u.email}</td>
                  <td className="px-4 py-3 text-gray-600 text-center">{u.employeeId || <span className="text-gray-300">—</span>}</td>
                  <td className="px-4 py-3">
                    <button
                      onClick={() => openRolesModal(u)}
                      className={`text-xs font-semibold px-2 py-1 rounded-full transition-colors ${
                        u.roles?.length > 0
                          ? "bg-purple-100 text-purple-700 hover:bg-purple-200"
                          : "bg-gray-100 text-gray-400 hover:bg-gray-200"
                      }`}
                    >
                      {u.roles?.length ?? 0}
                    </button>
                  </td>
                  <td className="px-4 py-3">
                    <button
                      onClick={() => openCompaniesModal(u)}
                      className={`text-xs font-semibold px-2 py-1 rounded-full transition-colors ${
                        u.companys?.length > 0
                          ? "bg-green-100 text-green-700 hover:bg-green-200"
                          : "bg-gray-100 text-gray-400 hover:bg-gray-200"
                      }`}
                    >
                      {u.companys?.length ?? 0}
                    </button>
                  </td>
                  <td className="px-4 py-3">
                    <span className={`px-2 py-1 rounded-full text-xs font-semibold ${u.enabled ? "bg-green-100 text-green-700" : "bg-red-100 text-red-700"}`}>
                      {u.enabled ? "Enabled" : "Inenabled"}
                    </span>
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex gap-1 flex-wrap">
                      <button onClick={() => openDetail(u)} className="text-gray-600 hover:text-gray-800 text-xs border border-gray-300 px-2 py-1 rounded hover:bg-gray-50 transition-colors">View</button>
                      <button onClick={() => openEdit(u)} className="text-green-600 hover:text-green-800 text-xs border border-green-300 px-2 py-1 rounded hover:bg-green-50 transition-colors">Edit</button>
                      <button onClick={() => openPwdModal(u)} className="text-amber-600 hover:text-amber-800 text-xs border border-amber-300 px-2 py-1 rounded hover:bg-amber-50 transition-colors">Pass</button>
                      <button onClick={() => openRolesModal(u)} className="text-purple-600 hover:text-purple-800 text-xs border border-purple-300 px-2 py-1 rounded hover:bg-purple-50 transition-colors">Roles</button>
                      <button onClick={() => openCompaniesModal(u)} className="text-green-600 hover:text-green-800 text-xs border border-green-300 px-2 py-1 rounded hover:bg-green-50 transition-colors">Companies</button>
                      {u.attempts > 0 && (
                        <button onClick={() => handleUnlock(u)} className="text-green-600 hover:text-green-800 text-xs border border-green-300 px-2 py-1 rounded hover:bg-green-50 transition-colors">Desbloquear</button>
                      )}
                      <button onClick={() => setDeleteTarget({ id: u.id, name: u.name })} className="text-red-600 hover:text-red-800 text-xs border border-red-300 px-2 py-1 rounded hover:bg-red-50 transition-colors">Delete</button>
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
              <h2 className="text-lg font-bold">User Details</h2>
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
                    <img src={avatar} alt="default avatar" className="w-16 h-16 rounded-full object-cover border" />
                  )}
                  <div>
                    <p className="font-bold text-lg">{detail.name}</p>
                    <p className="text-gray-400 font-mono">{detail.userName ?? detail.firstName}</p>
                    <p className="text-gray-500">{detail.email}</p>
                  </div>
                </div>

                {/* Flags */}
                <div className="flex gap-2 flex-wrap">
                  <span className={`px-2 py-1 rounded-full text-xs font-semibold ${detail.enabled ? "bg-green-100 text-green-700" : "bg-gray-100 text-gray-500"}`}>{detail.enabled ? "Enabled" : "Inenabled"}</span>
                  {/* No cambioPassword flag in API model */}
                </div>

                {/* Fields */}
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                  {[
                    ["ID", detail.id],
                    ["Password de user", detail.userName || "—"],
                    ["Nº Empleado", detail.employeeId || "—"],
                    ["Apellido Paterno", detail.lastNameFather || "—"],
                    ["Apellido Materno", detail.lastNameMother || "—"],
                    ["Abbreviation", detail.abbreviation || "—"],
                    ["Order", detail.order],
                    ["Failed Attempts", detail.attempts ?? 0],
                  ].map(([label, val]) => (
                    <div key={label}>
                      <p className="text-gray-400 text-xs uppercase tracking-wide mb-0.5">{label}</p>
                      <p className="font-medium">{val}</p>
                    </div>
                  ))}
                  {detail.description && (
                    <div className="col-span-2">
                      <p className="text-gray-400 text-xs uppercase tracking-wide mb-0.5">Description</p>
                      <p>{detail.description}</p>
                    </div>
                  )}
                </div>

                {/* Roles */}
                <div>
                  <p className="text-gray-400 text-xs uppercase tracking-wide mb-2">Roles ({detail.roles?.length ?? 0})</p>
                  {detail.roles?.length > 0 ? (
                    <ul className="space-y-1">
                      {detail.roles.map((p) => (
                        <li key={p.id} className="bg-purple-50 rounded px-3 py-2 font-medium text-purple-800">{p.name} <span className="text-purple-400 font-normal text-xs ml-1">({p.abbreviation})</span></li>
                      ))}
                    </ul>
                  ) : <p className="text-gray-400">Sin roles asignados.</p>}
                </div>

                {/* Companies */}
                <div>
                  <p className="text-gray-400 text-xs uppercase tracking-wide mb-2">Companies ({detail.companys?.length ?? 0})</p>
                  {detail.companys?.length > 0 ? (
                    <ul className="space-y-1">
                      {detail.companys.map((e) => (
                        <li key={e.id} className="bg-green-50 rounded px-3 py-2 font-medium text-green-800">{e.name} <span className="text-green-400 font-normal text-xs ml-1">({e.abbreviation})</span></li>
                      ))}
                    </ul>
                  ) : <p className="text-gray-400">Sin companies asignadas.</p>}
                </div>

                {/* Actions */}
                <div className="flex gap-2 pt-2 flex-wrap">
                  <button onClick={() => { setDetail(null); openEdit(detail); }} className="flex-1 bg-green-600 hover:bg-green-700 text-white py-2 rounded text-sm font-semibold transition-colors">Edit</button>
                  <button onClick={() => { setDetail(null); openRolesModal(detail); }} className="flex-1 bg-purple-600 hover:bg-purple-700 text-white py-2 rounded text-sm font-semibold transition-colors">Roles</button>
                  <button onClick={() => { setDetail(null); openCompaniesModal(detail); }} className="flex-1 bg-green-600 hover:bg-green-700 text-white py-2 rounded text-sm font-semibold transition-colors">Companies</button>
                  <button onClick={() => { openPwdModal(detail); setDetail(null); }} className="flex-1 bg-amber-500 hover:bg-amber-600 text-white py-2 rounded text-sm font-semibold transition-colors">Pass</button>
                  {detail.attempts > 0 && (
                    <button onClick={() => handleUnlock(detail)} className="flex-1 bg-green-600 hover:bg-green-700 text-white py-2 rounded text-sm font-semibold transition-colors">Unlock</button>
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
                <p className="text-xs font-semibold text-gray-400 uppercase tracking-wide mb-3">Personal Data</p>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Name <span className="text-red-500">*</span></label>
                    <input name="name" required value={modal.data.name} onChange={handleFormChange} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Abbreviation</label>
                    <input name="abbreviation" value={modal.data.abbreviation ?? ""} onChange={handleFormChange} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Last Name (Father)</label>
                    <input name="lastNameFather" value={modal.data.lastNameFather ?? ""} onChange={handleFormChange} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Last Name (Mother)</label>
                    <input name="lastNameMother" value={modal.data.lastNameMother ?? ""} onChange={handleFormChange} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Email</label>
                    <input name="email" type="email" value={modal.data.email ?? ""} onChange={handleFormChange} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Nº Empleado</label>
                    <input name="employeeId" type="number" min={0} value={modal.data.employeeId ?? 0} onChange={handleFormChange} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" />
                  </div>
                </div>
              </div>

              {/* Account section */}
              <div>
                <p className="text-xs font-semibold text-gray-400 uppercase tracking-wide mb-3">Cuenta</p>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">User Password <span className="text-red-500">*</span></label>
                    <input name="userName" required value={modal.data.userName ?? ""} onChange={handleFormChange} className="w-full border border-gray-300 rounded p-2 text-sm font-mono focus:outline-none focus:ring-2 focus:ring-green-500" />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">User Name</label>
                    <input name="firstName" value={modal.data.firstName ?? ""} onChange={handleFormChange} className="w-full border border-gray-300 rounded p-2 text-sm font-mono focus:outline-none focus:ring-2 focus:ring-green-500" />
                  </div>
                  {modal.mode === "add" && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Password <span className="text-red-500">*</span>
                    </label>
                    <input name="password" type="password" required value={modal.data.password ?? ""} onChange={handleFormChange} autoComplete="new-password" className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" />
                  </div>
                  )}
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Order</label>
                    <input name="order" type="number" min={0} value={modal.data.order ?? 0} onChange={handleFormChange} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" />
                  </div>
                </div>

                {/* Flags */}
                <div className="flex flex-wrap gap-6 mt-4">
                  <div className="flex items-center gap-2">
                    <input id="flag-enabled" name="enabled" type="checkbox" checked={modal.data.enabled ?? false} onChange={handleFormChange} className="w-4 h-4 accent-green-600" />
                    <label htmlFor="flag-enabled" className="text-sm font-medium text-gray-700">Enabled</label>
                  </div>
                </div>
              </div>

              {/* Descripción */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Description</label>
                <textarea name="description" value={modal.data.description ?? ""} onChange={handleFormChange} rows={2} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500 resize-none" />
              </div>

              <div className="flex justify-end gap-3 pt-2">
                <button type="button" onClick={closeModal} className="px-4 py-2 border border-gray-300 rounded text-sm hover:bg-gray-50 transition-colors">Cancel</button>
                <button type="submit" disabled={saving} className="px-4 py-2 bg-green-600 hover:bg-green-700 disabled:bg-green-300 text-white rounded text-sm font-semibold transition-colors">
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
              <h2 className="text-lg font-bold">Change Password</h2>
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
                <input type="password" required autoComplete="new-password" value={pwdModal.password} onChange={(e) => setPwdModal((m) => ({ ...m, password: e.target.value }))} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Confirm password <span className="text-red-500">*</span></label>
                <input type="password" required value={pwdModal.confirm} onChange={(e) => setPwdModal((m) => ({ ...m, confirm: e.target.value }))} className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" />
              </div>
              <div className="flex justify-end gap-3 pt-2">
                <button type="button" onClick={() => setPwdModal((m) => ({ ...m, open: false }))} className="px-4 py-2 border border-gray-300 rounded text-sm hover:bg-gray-50 transition-colors">Cancel</button>
                <button type="submit" disabled={pwdModal.saving} className="px-4 py-2 bg-amber-500 hover:bg-amber-600 disabled:bg-amber-300 text-white rounded text-sm font-semibold transition-colors">
                  {pwdModal.saving ? "Guardando..." : "Change Password"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* ── Companies Modal ── */}
      {companiesModal.open && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl w-full max-w-md">
            <div className="flex items-center justify-between px-6 py-4 border-b">
              <h2 className="text-lg font-bold">Companies — {companiesModal.userName}</h2>
              <button onClick={() => setCompaniesModal(m => ({ ...m, open: false }))} className="text-gray-400 hover:text-gray-600 text-2xl font-bold leading-none">×</button>
            </div>
            <div className="p-6 space-y-4">
              {companiesModal.error && (
                <div className="p-2 bg-red-100 border border-red-400 text-red-700 rounded text-sm flex justify-between">
                  <span>{companiesModal.error}</span>
                  <button onClick={() => setCompaniesModal(m => ({ ...m, error: "" }))} className="font-bold ml-4">×</button>
                </div>
              )}

              {/* Assign new company */}
              <div className="flex gap-2">
                <select
                  value={companiesModal.selectedCompanyId}
                  onChange={(e) => setCompaniesModal(m => ({ ...m, selectedCompanyId: e.target.value }))}
                  disabled={companiesModal.loading || companiesModal.actionLoading}
                  className="flex-1 border border-gray-300 rounded p-2 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-green-500 disabled:opacity-50"
                >
                  <option value="">— Select company to assign —</option>
                  {companiesModal.allCompanies
                    .filter(c => !companiesModal.userCompanies.some(uc => uc.id === c.id))
                    .map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                </select>
                <button
                  onClick={() => companiesModal.selectedCompanyId && handleAssignCompany(Number(companiesModal.selectedCompanyId))}
                  disabled={!companiesModal.selectedCompanyId || companiesModal.actionLoading || companiesModal.loading}
                  className="px-4 py-2 bg-green-600 hover:bg-green-700 disabled:bg-green-300 text-white rounded text-sm font-semibold transition-colors whitespace-nowrap"
                >
                  {companiesModal.actionLoading ? "..." : "Assign"}
                </button>
              </div>

              {/* Current companies list */}
              <div>
                <p className="text-xs font-semibold text-gray-400 uppercase tracking-wide mb-2">
                  Assigned companies ({companiesModal.userCompanies.length})
                </p>
                {companiesModal.loading ? (
                  <div className="py-6 text-center text-gray-400 animate-pulse">Loading...</div>
                ) : companiesModal.userCompanies.length === 0 ? (
                  <p className="text-gray-400 text-sm">No companies assigned.</p>
                ) : (
                  <ul className="space-y-2">
                    {companiesModal.userCompanies.map(company => (
                      <li key={company.id} className="flex items-center justify-between bg-green-50 rounded px-3 py-2">
                        <span className="font-medium text-green-800 text-sm">
                          {company.name}
                          {company.abbreviation && <span className="text-green-400 font-normal text-xs ml-2">({company.abbreviation})</span>}
                        </span>
                        <button
                          onClick={() => handleRemoveCompany(company.id)}
                          disabled={companiesModal.actionLoading}
                          className="text-red-500 hover:text-red-700 text-xs border border-red-300 px-2 py-0.5 rounded hover:bg-red-50 transition-colors disabled:opacity-50 ml-3"
                        >
                          Remove
                        </button>
                      </li>
                    ))}
                  </ul>
                )}
              </div>

              <div className="flex justify-end pt-2">
                <button onClick={() => setCompaniesModal(m => ({ ...m, open: false }))} className="px-4 py-2 border border-gray-300 rounded text-sm hover:bg-gray-50 transition-colors">Close</button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* ── Roles Modal ── */}
      {rolesModal.open && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl w-full max-w-md">
            <div className="flex items-center justify-between px-6 py-4 border-b">
              <h2 className="text-lg font-bold">Roles — {rolesModal.userName}</h2>
              <button onClick={() => setRolesModal(m => ({ ...m, open: false }))} className="text-gray-400 hover:text-gray-600 text-2xl font-bold leading-none">×</button>
            </div>
            <div className="p-6 space-y-4">
              {rolesModal.error && (
                <div className="p-2 bg-red-100 border border-red-400 text-red-700 rounded text-sm flex justify-between">
                  <span>{rolesModal.error}</span>
                  <button onClick={() => setRolesModal(m => ({ ...m, error: "" }))} className="font-bold ml-4">×</button>
                </div>
              )}

              {/* Assign new role */}
              <div className="flex gap-2">
                <select
                  value={rolesModal.selectedRoleId}
                  onChange={(e) => setRolesModal(m => ({ ...m, selectedRoleId: e.target.value }))}
                  disabled={rolesModal.loading || rolesModal.actionLoading}
                  className="flex-1 border border-gray-300 rounded p-2 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-purple-500 disabled:opacity-50"
                >
                  <option value="">— Select role to assign —</option>
                  {rolesModal.allRoles
                    .filter(r => !rolesModal.userRoles.some(ur => ur.id === r.id))
                    .map(r => <option key={r.id} value={r.id}>{r.name}</option>)}
                </select>
                <button
                  onClick={() => rolesModal.selectedRoleId && handleAssignRole(Number(rolesModal.selectedRoleId))}
                  disabled={!rolesModal.selectedRoleId || rolesModal.actionLoading || rolesModal.loading}
                  className="px-4 py-2 bg-purple-600 hover:bg-purple-700 disabled:bg-purple-300 text-white rounded text-sm font-semibold transition-colors whitespace-nowrap"
                >
                  {rolesModal.actionLoading ? "..." : "Assign"}
                </button>
              </div>

              {/* Current roles list */}
              <div>
                <p className="text-xs font-semibold text-gray-400 uppercase tracking-wide mb-2">
                  Assigned roles ({rolesModal.userRoles.length})
                </p>
                {rolesModal.loading ? (
                  <div className="py-6 text-center text-gray-400 animate-pulse">Loading...</div>
                ) : rolesModal.userRoles.length === 0 ? (
                  <p className="text-gray-400 text-sm">No roles assigned.</p>
                ) : (
                  <ul className="space-y-2">
                    {rolesModal.userRoles.map(role => (
                      <li key={role.id} className="flex items-center justify-between bg-purple-50 rounded px-3 py-2">
                        <span className="font-medium text-purple-800 text-sm">
                          {role.name}
                          {role.abbreviation && <span className="text-purple-400 font-normal text-xs ml-2">({role.abbreviation})</span>}
                        </span>
                        <button
                          onClick={() => handleRemoveRole(role.id)}
                          disabled={rolesModal.actionLoading}
                          className="text-red-500 hover:text-red-700 text-xs border border-red-300 px-2 py-0.5 rounded hover:bg-red-50 transition-colors disabled:opacity-50 ml-3"
                        >
                          Remove
                        </button>
                      </li>
                    ))}
                  </ul>
                )}
              </div>

              <div className="flex justify-end pt-2">
                <button onClick={() => setRolesModal(m => ({ ...m, open: false }))} className="px-4 py-2 border border-gray-300 rounded text-sm hover:bg-gray-50 transition-colors">Close</button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* ── Delete Confirmation ── */}
      {deleteTarget && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl p-6 w-full max-w-sm">
            <h2 className="text-lg font-bold mb-2">Confirm eliminación</h2>
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
