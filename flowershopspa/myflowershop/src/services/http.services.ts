import axios, { type AxiosInstance, type AxiosRequestConfig, type AxiosResponse, AxiosError } from 'axios'
import { API_CONFIG } from '@/config/api.configs'

class HttpService {
  private client: AxiosInstance

  constructor() {
    this.client = axios.create({
      baseURL: API_CONFIG.baseURL,
      timeout: API_CONFIG.timeout,
      headers: API_CONFIG.headers
    })

    this.setupInterceptors()
  }

  private setupInterceptors(): void {
    // Request interceptor
    this.client.interceptors.request.use(
      (config) => {
        // Add auth token if available
        const token = localStorage.getItem('auth_token')
        if (token && config.headers) {
          config.headers.Authorization = `Bearer ${token}`
        }
        return config
      },
      (error: AxiosError) => {
        return Promise.reject(error)
      }
    )

    // Response interceptor
    this.client.interceptors.response.use(
      (response: any) => {
        return Promise.resolve(response);
    },
    (error) => {
        if (error.response?.status == 403) {
            console.log("Your do not have permission.");
        }
        if (error?.response?.status == 204) {
            // logout
        }
        if (error?.response?.status == 503
             && error?.response.data?.Data?.IsOrderShare == false
        ) {
            console.log("Service unvailable.");
        }
        // mode maintenance
        return Promise.reject(
        error instanceof Error
            ? error
            : new Error(error.message || 'Request failed')
        );
    },
    )
  }

  /**
   * GET request
   * @param url - API endpoint
   * @param config - Axios config options
   * @returns Promise with response data
   */
  public get<T = any>(url: string, config?: AxiosRequestConfig): Promise<T> {
    return this.client.get<T, T>(url, config)
  }

  /**
   * POST request
   * @param url - API endpoint
   * @param data - Request body
   * @param config - Axios config options
   * @returns Promise with response data
   */
  public post<T = any, D = any>(url: string, data?: D, config?: AxiosRequestConfig): Promise<T> {
    return this.client.post<T, T, D>(url, data, config)
  }

  /**
   * PUT request
   * @param url - API endpoint
   * @param data - Request body
   * @param config - Axios config options
   * @returns Promise with response data
   */
  public put<T = any, D = any>(url: string, data?: D, config?: AxiosRequestConfig): Promise<T> {
    return this.client.put<T, T, D>(url, data, config)
  }

  /**
   * DELETE request
   * @param url - API endpoint
   * @param config - Axios config options
   * @returns Promise with response data
   */
  public delete<T = any>(url: string, config?: AxiosRequestConfig): Promise<T> {
    return this.client.delete<T, T>(url, config)
  }

  /**
   * PATCH request
   * @param url - API endpoint
   * @param data - Request body
   * @param config - Axios config options
   * @returns Promise with response data
   */
  public patch<T = any, D = any>(url: string, data?: D, config?: AxiosRequestConfig): Promise<T> {
    return this.client.patch<T, T, D>(url, data, config)
  }

  /**
   * Update base URL dynamically
   * @param newBaseURL - New base URL
   */
  public setBaseURL(newBaseURL: string): void {
    this.client.defaults.baseURL = newBaseURL
  }

  /**
   * Set authorization token
   * @param token - Auth token
   */
  public setAuthToken(token: string): void {
    this.client.defaults.headers.common['Authorization'] = `Bearer ${token}`
    localStorage.setItem('auth_token', token)
  }

  /**
   * Remove authorization token
   */
  public removeAuthToken(): void {
    delete this.client.defaults.headers.common['Authorization']
    localStorage.removeItem('auth_token')
  }

  /**
   * Get the axios instance for advanced usage
   */
  public getClient(): AxiosInstance {
    return this.client
  }
}

// Export singleton instance
export default new HttpService()