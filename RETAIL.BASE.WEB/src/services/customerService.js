import { apiClient, unwrap } from './apiClient';
import { normalizePaginatedResult } from './pagination';

export async function getAll({ enabled, pageIndex = 1, pageSize = 20 } = {}) {
  const params = { pageIndex, pageSize };
  if (enabled !== undefined) params.enabled = enabled;
  const response = await apiClient.get('/api/v1/Customer/GetAll', { params });
  return normalizePaginatedResult(unwrap(response), { pageIndex, pageSize });
}

export async function getById(idCustomer) {
  const response = await apiClient.get('/api/v1/Customer/GetById', {
    params: { idCustomer },
  });
  return unwrap(response);
}

export async function add(customer) {
  const response = await apiClient.post('/api/v1/Customer/Add', customer);
  return unwrap(response);
}

export async function update(customer) {
  const response = await apiClient.put('/api/v1/Customer/Update', customer);
  return unwrap(response);
}

export async function remove(idCustomer) {
  const response = await apiClient.delete('/api/v1/Customer/Delete', {
    params: { idCustomer },
  });
  return unwrap(response);
}
