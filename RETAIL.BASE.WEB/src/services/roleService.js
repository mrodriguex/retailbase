import { apiClient, unwrap } from './apiClient';
import { normalizePaginatedResult } from './pagination';

export async function getAll({ enabled, pageIndex = 1, pageSize = 20 } = {}) {
  const params = { pageIndex, pageSize };
  if (enabled !== undefined) params.enabled = enabled;
  const response = await apiClient.get('/api/v1/Role/GetAll', { params });
  return normalizePaginatedResult(unwrap(response), { pageIndex, pageSize });
}

export async function getById(idRole) {
  const response = await apiClient.get('/api/v1/Role/GetById', {
    params: { idRole },
  });
  return unwrap(response);
}

export async function add(role) {
  const response = await apiClient.post('/api/v1/Role/Add', role);
  return unwrap(response);
}

export async function update(role) {
  const response = await apiClient.put('/api/v1/Role/Update', role);
  return unwrap(response);
}

export async function remove(idRole) {
  const response = await apiClient.delete('/api/v1/Role/Delete', {
    params: { idRole },
  });
  return unwrap(response);
}

export async function getUserProfiles(idUsuario) {
  const response = await apiClient.get('/api/v1/Role/GetUserProfiles', {
    params: { idUsuario },
  });
  return unwrap(response);
}
