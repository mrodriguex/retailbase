import { apiClient, unwrap } from './apiClient';
import { normalizePaginatedResult } from './pagination';

export async function getAll({ enabled, pageIndex = 1, pageSize = 20 } = {}) {
  const params = { pageIndex, pageSize };
  if (enabled !== undefined) params.enabled = enabled;
  const response = await apiClient.get('/api/v1/MenuItem/GetAll', { params });
  return normalizePaginatedResult(unwrap(response), { pageIndex, pageSize });
}

export async function getById(idMenuItem) {
  const response = await apiClient.get('/api/v1/MenuItem/GetById', {
    params: { idMenuItem },
  });
  return unwrap(response);
}

export async function add(menuItem) {
  const response = await apiClient.post('/api/v1/MenuItem/Add', menuItem);
  return unwrap(response);
}

export async function update(menuItem) {
  const response = await apiClient.put('/api/v1/MenuItem/Update', menuItem);
  return unwrap(response);
}

export async function remove(idMenuItem) {
  const response = await apiClient.delete('/api/v1/MenuItem/Delete', {
    params: { idMenuItem },
  });
  return unwrap(response);
}

export async function getMenuItemsByUser(idUsuario, idRole) {
  const response = await apiClient.get('/api/v1/MenuItem/GetMenuItemsByUser', {
    params: { idUser: idUsuario, idRole },
  });
  return unwrap(response);
}

export async function getMenuItemsByProfile(idRole) {
  const response = await apiClient.get('/api/v1/MenuItem/GetMenuItemsByProfile', {
    params: { idRole },
  });
  return unwrap(response);
}
