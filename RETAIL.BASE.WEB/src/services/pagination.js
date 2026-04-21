function asArray(value) {
  if (Array.isArray(value)) return value;
  if (Array.isArray(value?.$values)) return value.$values;
  return [];
}

/**
 * Normalizes list payloads from multiple backend response shapes.
 * Supports:
 * - Array payloads
 * - { items, total }
 * - { data, totalCount, pageIndex, pageSize }
 */
export function normalizePaginatedResult(payload, defaults = {}) {
  const defaultPageIndex = defaults.pageIndex ?? 1;
  const defaultPageSize = defaults.pageSize ?? 20;

  if (Array.isArray(payload)) {
    return {
      items: payload,
      total: payload.length,
      pageIndex: defaultPageIndex,
      pageSize: defaultPageSize,
    };
  }

  const nestedData = payload?.data;
  const items =
    asArray(payload?.items).length > 0
      ? asArray(payload.items)
      : asArray(nestedData?.data).length > 0
      ? asArray(nestedData.data)
      : asArray(payload?.data).length > 0
      ? asArray(payload.data)
      : asArray(payload?.$values);

  const totalRaw =
    payload?.totalCount ??
    payload?.total ??
    nestedData?.totalCount ??
    nestedData?.total ??
    items.length;

  const pageIndexRaw = payload?.pageIndex ?? nestedData?.pageIndex ?? defaultPageIndex;
  const pageSizeRaw = payload?.pageSize ?? nestedData?.pageSize ?? defaultPageSize;

  return {
    items,
    total: Number.isFinite(totalRaw) ? Number(totalRaw) : items.length,
    pageIndex: Number.isFinite(pageIndexRaw) ? Number(pageIndexRaw) : defaultPageIndex,
    pageSize: Number.isFinite(pageSizeRaw) ? Number(pageSizeRaw) : defaultPageSize,
  };
}