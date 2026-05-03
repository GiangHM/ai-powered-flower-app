import type { AxiosResponse } from 'axios'
import httpService from './http.services'
import type {
  RegisterDto,
  LoginDto,
  ConfirmEmailDto,
  AuthResponseDto
} from '@/models/auth/auth'

class AuthService {
  private readonly AUTH_BASE_PATH = '/api/Auth'

  /**
   * Register a new customer
   * @param data - Registration data
   * @returns Promise with auth response (JWT token)
   */
  async register(data: RegisterDto): Promise<AuthResponseDto> {
    const response = await httpService.post<AuthResponseDto>(
      `${this.AUTH_BASE_PATH}/register`,
      data
    )
    return response
  }

  /**
   * Login with email and password
   * @param data - Login credentials
   * @returns Promise with auth response (JWT token)
   */
  async login(data: LoginDto): Promise<AxiosResponse<AuthResponseDto>> {
    const response = await httpService.post<AxiosResponse<AuthResponseDto>>(
      `${this.AUTH_BASE_PATH}/login`,
      data
    )
    return response
  }

  /**
   * Confirm email with verification token
   * @param data - Confirmation token
   * @returns Promise with success message
   */
  async confirmEmail(data: ConfirmEmailDto): Promise<void> {
    await httpService.post(`${this.AUTH_BASE_PATH}/confirm-email`, data)
  }

  /**
   * Logout user by clearing auth token
   */
  logout(): void {
    httpService.removeAuthToken()
  }

  /**
   * Set authentication token
   * @param token - JWT token
   */
  setToken(token: string): void {
    httpService.setAuthToken(token)
  }

  /**
   * Get current auth token from localStorage
   * @returns JWT token or null
   */
  getToken(): string | null {
    return localStorage.getItem('auth_token')
  }

  /**
   * Check if user is authenticated
   * @returns boolean indicating if user has valid token
   */
  isAuthenticated(): boolean {
    const token = this.getToken()
    if (!token) return false

    // Parse JWT to check expiration (basic check without full validation)
    try {
      const tokenPayload = token.split('.')[1]
      if (!tokenPayload) return false
      const payload = JSON.parse(atob(tokenPayload))
      const exp = payload.exp * 1000 // Convert to milliseconds
      return Date.now() < exp
    } catch {
      return false
    }
  }
  isAdmin(): boolean {
    const token = this.getToken()
    if (!token) return false

    try {
      const tokenPayload = token.split('.')[1]
      if (!tokenPayload) return false
      const payload = JSON.parse(atob(tokenPayload))
      const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
      return role === 'Admin'
    } catch {
      return false
    }
  }
}

export default new AuthService()
