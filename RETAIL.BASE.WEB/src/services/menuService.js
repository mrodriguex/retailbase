import { apiClient, unwrap } from './apiClient';
import { normalizePaginatedResult } from './pagination';

export async function getAll({ enabled, pageIndex = 1, pageSize = 20 } = {}) {
  const params = { pageIndex, pageSize };
  if (enabled !== undefined) params.enabled = enabled;
  const response = await apiClient.get('/api/v1/Menu/GetAll', { params });
  return normalizePaginatedResult(unwrap(response), { pageIndex, pageSize });
}

export async function getById(idMenu) {
  const response = await apiClient.get('/api/v1/Menu/GetById', {
    params: { idMenu },
  });
  return unwrap(response);
}

export async function add(menu) {
  const response = await apiClient.post('/api/v1/Menu/Add', menu);
  return unwrap(response);
}

export async function update(menu) {
  const response = await apiClient.put('/api/v1/Menu/Update', menu);
  return unwrap(response);
}

export async function remove(idMenu) {
  const response = await apiClient.delete('/api/v1/Menu/Delete', {
    params: { idMenu },
  });
  return unwrap(response);
}

export async function getMenusByUser(idUsuario, idRole) {
  const response = await apiClient.get('/api/v1/Menu/GetMenusByUser', {
    params: { idUsuario, idRole },
  });
  return unwrap(response);
}

export async function getMenusByProfile(idRole) {
  const response = await apiClient.get('/api/v1/Menu/GetMenusByProfile', {
    params: { idRole },
  });
  return unwrap(response);
}
