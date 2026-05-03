export const API_CONFIG = {
  baseURL: import.meta.env.VITE_API_BASE_URL || 'https://localhost:7204/',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
}