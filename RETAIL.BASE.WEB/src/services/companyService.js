import { apiClient, unwrap } from './apiClient';
import { normalizePaginatedResult } from './pagination';

export async function getAll({ enabled, pageIndex = 1, pageSize = 20 } = {}) {
  const params = { pageIndex, pageSize };
  if (enabled !== undefined) params.enabled = enabled;
  const response = await apiClient.get('/api/v1/Company/GetAll', { params });
  return normalizePaginatedResult(unwrap(response), { pageIndex, pageSize });
}

export async function getCompaniesByUser(idUsuario) {
  const response = await apiClient.get('/api/v1/Company/GetCompaniesByUser', {
    params: { idUser: idUsuario },
  });
  return unwrap(response);
}

export async function assignCompanyToUser(idUser, idCompany) {
  const response = await apiClient.post('/api/v1/Company/AssignCompanyToUser', null, {
    params: { idUser, idCompany },
  });
  return unwrap(response);
}

export async function removeCompanyFromUser(idUser, idCompany) {
  const response = await apiClient.post('/api/v1/Company/RemoveCompanyFromUser', null, {
    params: { idUser, idCompany },
  });
  return unwrap(response);
}

export async function getById(idCompany) {
  const response = await apiClient.get('/api/v1/Company/GetById', {
    params: { idCompany },
  });
  return unwrap(response);
}

export async function add(company) {
  const response = await apiClient.post('/api/v1/Company/Add', company);
  return unwrap(response);
}

export async function update(company) {
  const response = await apiClient.put('/api/v1/Company/Update', company);
  return unwrap(response);
}

export async function remove(idCompany) {
  const response = await apiClient.delete('/api/v1/Company/Delete', {
    params: { idCompany },
  });
  return unwrap(response);
}
