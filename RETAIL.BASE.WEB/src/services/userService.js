import { apiClient, unwrap } from './apiClient';
import { normalizePaginatedResult } from './pagination';

export async function getAll({ enabled, pageIndex = 1, pageSize = 20 } = {}) {
  const params = { pageIndex, pageSize };
  if (enabled !== undefined) params.enabled = enabled;
  const response = await apiClient.get('/api/v1/User/GetAll', { params });
  return normalizePaginatedResult(unwrap(response), { pageIndex, pageSize });
}

export async function getById(idUser) {
  const response = await apiClient.get('/api/v1/User/GetById', {
    params: { idUser },
  });
  return unwrap(response);
}

export async function exists(idUser) {
  const response = await apiClient.get('/api/v1/User/Exists', {
    params: { idUser },
  });
  return unwrap(response);
}

export async function add(user) {
  const response = await apiClient.post('/api/v1/User/Add', user);
  return unwrap(response);
}

export async function update(user) {
  const response = await apiClient.put('/api/v1/User/Update', user);
  return unwrap(response);
}

export async function remove(idUser) {
  const response = await apiClient.delete('/api/v1/User/Delete', {
    params: { idUser },
  });
  return unwrap(response);
}

export async function unlockUser(idUser) {
  const response = await apiClient.put('/api/v1/User/UnlockUser', idUser);
  return unwrap(response);
}

export async function updatePassword(username, password) {
  const response = await apiClient.put('/api/v1/User/UpdatePassword', {
    username: String(username),
    password,
  });
  return unwrap(response);
}
