import { apiClient, unwrap } from './apiClient';
import { normalizePaginatedResult } from './pagination';

export async function getAll({ enabled, pageIndex = 1, pageSize = 20 } = {}) {
  const params = { pageIndex, pageSize };
  if (enabled !== undefined) params.enabled = enabled;
  const response = await apiClient.get('/api/v1/Brand/GetAll', { params });
  return normalizePaginatedResult(unwrap(response), { pageIndex, pageSize });
}

export async function getById(idBrand) {
  const response = await apiClient.get('/api/v1/Brand/GetById', {
    params: { id: idBrand },
  });
  return unwrap(response);
}

export async function add(brand) {
  const response = await apiClient.post('/api/v1/Brand/Add', brand);
  return unwrap(response);
}

export async function update(brand) {
  const response = await apiClient.put('/api/v1/Brand/Update', brand);
  return unwrap(response);
}

export async function remove(idBrand) {
  const response = await apiClient.delete('/api/v1/Brand/Delete', {
    params: { id: idBrand },
  });
  return unwrap(response);
}
