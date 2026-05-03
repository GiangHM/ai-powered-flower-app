export interface RegisterDto {
  name: string
  email: string
  password: string
  phone: string
  deliveryAddress: string
}

export interface LoginDto {
  email: string
  password: string
}

export interface ConfirmEmailDto {
  token: string
}

export interface AuthResponseDto {
  token: string
  expiresAt: string
}

export interface User {
  id: number
  name: string
  email: string
  phone: string
  deliveryAddress: string
  status: 'Pending' | 'Active' | 'Inactive'
  emailConfirmed: boolean
}
