const required = (value, name) => {
    if (!value || value.trim() === '') {
        throw new Error(`${name} is required.`);
    }
    return value;
};

const parseBool = (value, defaultValue = false) => {
    if (value === undefined || value === null) {
        return defaultValue;
    }
    if (typeof value === 'boolean') {
        return value;
    }
    if (typeof value === 'string') {
        const lowerValue = value.toLowerCase();
        if (lowerValue === 'true' || lowerValue === '1') {
            return true;
        }
        if (lowerValue === 'false' || lowerValue === '0') {
            return false;
        }
    }
    throw new Error(`Invalid boolean value: ${value}`);
};

export const env = {
  apiBaseUrl: required(import.meta.env.VITE_API_BASE_URL, "VITE_API_BASE_URL"),
  signalrHubUrl: required(import.meta.env.VITE_SIGNALR_HUB_URL, "VITE_SIGNALR_HUB_URL"),
  signalrSkipNegotiation: parseBool(import.meta.env.VITE_SIGNALR_SKIP_NEGOTIATION, false),
};
