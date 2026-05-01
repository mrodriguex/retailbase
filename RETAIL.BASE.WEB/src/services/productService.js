import { apiClient, unwrap } from './apiClient';
import { normalizePaginatedResult } from './pagination';

export async function getAll({ enabled, pageIndex = 1, pageSize = 20 } = {}) {
  const params = { pageIndex, pageSize };
  if (enabled !== undefined) params.enabled = enabled;
  const response = await apiClient.get('/api/v1/Product/GetAll', { params });
  return normalizePaginatedResult(unwrap(response), { pageIndex, pageSize });
}

export async function getById(idProduct) {
  const response = await apiClient.get('/api/v1/Product/GetById', {
    params: { id: idProduct },
  });
  return unwrap(response);
}

export async function add(product) {
  const response = await apiClient.post('/api/v1/Product/Add', product);
  return unwrap(response);
}

export async function update(product) {
  const response = await apiClient.put('/api/v1/Product/Update', product);
  return unwrap(response);
}

export async function remove(idProduct) {
  const response = await apiClient.delete('/api/v1/Product/Delete', {
    params: { id: idProduct },
  });
  return unwrap(response);
}
