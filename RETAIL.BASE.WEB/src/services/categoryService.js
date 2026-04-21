import { apiClient, unwrap } from './apiClient';
import { normalizePaginatedResult } from './pagination';

export async function getAll({ enabled, pageIndex = 1, pageSize = 20 } = {}) {
  const params = { pageIndex, pageSize };
  if (enabled !== undefined) params.enabled = enabled;
  const response = await apiClient.get('/api/v1/Category/GetAll', { params });
  return normalizePaginatedResult(unwrap(response), { pageIndex, pageSize });
}

export async function getById(idCategory) {
  const response = await apiClient.get('/api/v1/Category/GetById', {
    params: { idCategory },
  });
  return unwrap(response);
}

export async function add(category) {
  const response = await apiClient.post('/api/v1/Category/Add', category);
  return unwrap(response);
}

export async function update(category) {
  const response = await apiClient.put('/api/v1/Category/Update', category);
  return unwrap(response);
}

export async function remove(idCategory) {
  const response = await apiClient.delete('/api/v1/Category/Delete', {
    params: { idCategory },
  });
  return unwrap(response);
}
