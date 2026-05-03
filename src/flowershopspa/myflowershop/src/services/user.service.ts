import httpService from '@/services/http.services'
import type {
  UserResponseDto,
  PagedResultDto,
  UpdateUserDto,
  UpdateUserStatusDto,
  OrderResponseDto
} from '@/models/user'

class UserService {
  private readonly BASE = '/api/Admin/Users'

  /**
   * Get paginated list of users with optional status filter
   */
  async getUsers(
    page: number,
    pageSize: number,
    status?: string
  ): Promise<PagedResultDto<UserResponseDto>> {
    let url = `${this.BASE}?page=${page}&pageSize=${pageSize}`
    if (status) {
      url += `&status=${encodeURIComponent(status)}`
    }
    const res = await httpService.get<PagedResultDto<UserResponseDto>>(url)
    return (res as any).data ?? res
  }

  /**
   * Approve or suspend a user by updating their status
   */
  async updateUserStatus(id: number, status: string): Promise<UserResponseDto> {
    const payload: UpdateUserStatusDto = { status }
    const res = await httpService.put<UserResponseDto>(
      `${this.BASE}/${id}/status`,
      payload
    )
    return (res as any).data ?? res
  }

  /**
   * Update user profile fields (name, phone, deliveryAddress)
   */
  async updateUser(id: number, data: UpdateUserDto): Promise<UserResponseDto> {
    const res = await httpService.put<UserResponseDto>(`${this.BASE}/${id}`, data)
    return (res as any).data ?? res
  }

  /**
   * Fetch order history for a given user
   */
  async getUserOrders(id: number): Promise<OrderResponseDto[]> {
    const res = await httpService.get<OrderResponseDto[]>(`${this.BASE}/${id}/orders`)
    return (res as any).data ?? res
  }
}

export const userService = new UserService()
