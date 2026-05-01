import { apiClient, unwrap } from './apiClient';
import { normalizePaginatedResult } from './pagination';

export async function getAll({ enabled, pageIndex = 1, pageSize = 20 } = {}) {
  const params = { pageIndex, pageSize };
  if (enabled !== undefined) params.enabled = enabled;
  const response = await apiClient.get('/api/v1/ProductPresentation/GetAll', { params });
  return normalizePaginatedResult(unwrap(response), { pageIndex, pageSize });
}

export async function getById(idProductPresentation) {
  const response = await apiClient.get('/api/v1/ProductPresentation/GetById', {
    params: { id: idProductPresentation },
  });
  return unwrap(response);
}

export async function add(productPresentation) {
  const response = await apiClient.post('/api/v1/ProductPresentation/Add', productPresentation);
  return unwrap(response);
}

export async function update(productPresentation) {
  const response = await apiClient.put('/api/v1/ProductPresentation/Update', productPresentation);
  return unwrap(response);
}

export async function remove(idProductPresentation) {
  const response = await apiClient.delete('/api/v1/ProductPresentation/Delete', {
    params: { id: idProductPresentation },
  });
  return unwrap(response);
}
